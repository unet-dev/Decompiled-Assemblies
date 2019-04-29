using Facepunch;
using Network;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GunTrap : StorageContainer
{
	public GameObjectRef gun_fire_effect;

	public GameObjectRef bulletEffect;

	public GameObjectRef triggeredEffect;

	public Transform muzzlePos;

	public Transform eyeTransform;

	public int numPellets = 15;

	public int aimCone = 30;

	public float sensorRadius = 1.25f;

	public ItemDefinition ammoType;

	public TargetTrigger trigger;

	public GunTrap()
	{
	}

	public bool CheckTrigger()
	{
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		HashSet<BaseEntity> baseEntities = this.trigger.entityContents;
		bool flag = false;
		if (baseEntities != null)
		{
			foreach (BaseEntity baseEntity in baseEntities)
			{
				BasePlayer component = baseEntity.GetComponent<BasePlayer>();
				if (component.IsSleeping() || !component.IsAlive() || component.IsBuildingAuthed())
				{
					continue;
				}
				list.Clear();
				Vector3 vector3 = component.eyes.position;
				Vector3 eyePosition = this.GetEyePosition() - component.eyes.position;
				GamePhysics.TraceAll(new Ray(vector3, eyePosition.normalized), 0f, list, 9f, 1218519297, QueryTriggerInteraction.UseGlobal);
				int num = 0;
				while (num < list.Count)
				{
					BaseEntity entity = list[num].GetEntity();
					if (!(entity != null) || !(entity == this) && !entity.EqualNetID(this))
					{
						if (!(entity != null) || entity.ShouldBlockProjectiles())
						{
							break;
						}
						num++;
					}
					else
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					continue;
				}
				Pool.FreeList<RaycastHit>(ref list);
				return flag;
			}
		}
		Pool.FreeList<RaycastHit>(ref list);
		return flag;
	}

	public void FireBullet()
	{
		float single = 10f;
		Vector3 vector3 = this.muzzlePos.transform.position - (this.muzzlePos.forward * 0.25f);
		Vector3 vector31 = this.muzzlePos.transform.forward;
		Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection((float)this.aimCone, vector31, true);
		Vector3 vector32 = vector3 + (modifiedAimConeDirection * 300f);
		base.ClientRPC<Vector3>(null, "CLIENT_FireGun", vector32);
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		GamePhysics.TraceAll(new Ray(vector3, modifiedAimConeDirection), 0.1f, list, 300f, 1219701521, QueryTriggerInteraction.UseGlobal);
		for (int i = 0; i < list.Count; i++)
		{
			RaycastHit item = list[i];
			BaseEntity entity = item.GetEntity();
			if (!(entity != null) || !(entity == this) && !entity.EqualNetID(this))
			{
				if (entity is BaseCombatEntity)
				{
					HitInfo hitInfo = new HitInfo(this, entity, DamageType.Bullet, single, item.point);
					entity.OnAttacked(hitInfo);
					if (entity is BasePlayer || entity is BaseNpc)
					{
						Effect.server.ImpactEffect(new HitInfo()
						{
							HitPositionWorld = item.point,
							HitNormalWorld = -item.normal,
							HitMaterial = StringPool.Get("Flesh")
						});
					}
				}
				if (!(entity != null) || entity.ShouldBlockProjectiles())
				{
					vector32 = item.point;
					return;
				}
			}
		}
	}

	public void FireWeapon()
	{
		if (!this.UseAmmo())
		{
			return;
		}
		Effect.server.Run(this.gun_fire_effect.resourcePath, this, StringPool.Get(this.muzzlePos.gameObject.name), Vector3.zero, Vector3.zero, null, false);
		for (int i = 0; i < this.numPellets; i++)
		{
			this.FireBullet();
		}
	}

	public Vector3 GetEyePosition()
	{
		return this.eyeTransform.position;
	}

	public bool IsTriggered()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning timeWarning = TimeWarning.New("GunTrap.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRandomized(new Action(this.TriggerCheck), UnityEngine.Random.Range(0f, 1f), 0.5f, 0.1f);
	}

	public void TriggerCheck()
	{
		if (this.CheckTrigger())
		{
			this.FireWeapon();
		}
	}

	public bool UseAmmo()
	{
		bool flag;
		List<Item>.Enumerator enumerator = this.inventory.itemList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Item current = enumerator.Current;
				if (!(current.info == this.ammoType) || current.amount <= 0)
				{
					continue;
				}
				current.UseItem(1);
				flag = true;
				return flag;
			}
			return false;
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		return flag;
	}

	public static class GunTrapFlags
	{
		public const BaseEntity.Flags Triggered = BaseEntity.Flags.Reserved1;
	}
}