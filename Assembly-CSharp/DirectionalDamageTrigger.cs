using Rust;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DirectionalDamageTrigger : TriggerBase
{
	public float repeatRate = 1f;

	public List<DamageTypeEntry> damageType;

	public GameObjectRef attackEffect;

	public DirectionalDamageTrigger()
	{
	}

	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		if (!(baseEntity is BaseCombatEntity))
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	internal override void OnEmpty()
	{
		base.CancelInvoke(new Action(this.OnTick));
	}

	internal override void OnObjects()
	{
		base.InvokeRepeating(new Action(this.OnTick), this.repeatRate, this.repeatRate);
	}

	private void OnTick()
	{
		if (this.attackEffect.isValid)
		{
			Effect.server.Run(this.attackEffect.resourcePath, base.transform.position, Vector3.up, null, false);
		}
		if (this.entityContents == null)
		{
			return;
		}
		BaseEntity[] array = this.entityContents.ToArray<BaseEntity>();
		for (int i = 0; i < (int)array.Length; i++)
		{
			BaseEntity baseEntity = array[i];
			if (baseEntity.IsValid())
			{
				BaseCombatEntity baseCombatEntity = baseEntity as BaseCombatEntity;
				if (baseCombatEntity != null)
				{
					HitInfo hitInfo = new HitInfo();
					hitInfo.damageTypes.Add(this.damageType);
					hitInfo.DoHitEffects = true;
					hitInfo.DidHit = true;
					hitInfo.PointStart = base.transform.position;
					hitInfo.PointEnd = baseCombatEntity.transform.position;
					baseCombatEntity.Hurt(hitInfo);
				}
			}
		}
	}
}