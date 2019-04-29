using Facepunch;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemModProjectileRadialDamage : ItemModProjectileMod
{
	public float radius = 0.5f;

	public DamageTypeEntry damage;

	public GameObjectRef effect;

	public bool ignoreHitObject = true;

	public ItemModProjectileRadialDamage()
	{
	}

	public override void ServerProjectileHit(HitInfo info)
	{
		if (this.effect.isValid)
		{
			Effect.server.Run(this.effect.resourcePath, info.HitPositionWorld, info.HitNormalWorld, null, false);
		}
		List<BaseCombatEntity> list = Pool.GetList<BaseCombatEntity>();
		List<BaseCombatEntity> baseCombatEntities = Pool.GetList<BaseCombatEntity>();
		Vis.Entities<BaseCombatEntity>(info.HitPositionWorld, this.radius, baseCombatEntities, 1236478737, QueryTriggerInteraction.Collide);
		foreach (BaseCombatEntity baseCombatEntity in baseCombatEntities)
		{
			if (!baseCombatEntity.isServer || list.Contains(baseCombatEntity) || baseCombatEntity == info.HitEntity && this.ignoreHitObject)
			{
				continue;
			}
			float single = Vector3.Distance(baseCombatEntity.ClosestPoint(info.HitPositionWorld), info.HitPositionWorld) / this.radius;
			if (single > 1f)
			{
				continue;
			}
			float single1 = 1f - single;
			if (!baseCombatEntity.IsVisible(info.HitPositionWorld + (info.HitNormalWorld * 0.1f), Single.PositiveInfinity))
			{
				continue;
			}
			list.Add(baseCombatEntity);
			baseCombatEntity.OnAttacked(new HitInfo(info.Initiator, baseCombatEntity, this.damage.type, this.damage.amount * single1));
		}
		Pool.FreeList<BaseCombatEntity>(ref list);
		Pool.FreeList<BaseCombatEntity>(ref baseCombatEntities);
	}
}