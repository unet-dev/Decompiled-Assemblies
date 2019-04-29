using Rust;
using System;
using System.Linq;
using UnityEngine;

public class TriggerHurt : TriggerBase
{
	public float DamagePerSecond = 1f;

	public float DamageTickRate = 4f;

	public DamageType damageType;

	public TriggerHurt()
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
		base.InvokeRepeating(new Action(this.OnTick), 0f, 1f / this.DamageTickRate);
	}

	private void OnTick()
	{
		BaseEntity baseEntity = base.gameObject.ToBaseEntity();
		if (this.entityContents == null)
		{
			return;
		}
		BaseEntity[] array = this.entityContents.ToArray<BaseEntity>();
		for (int i = 0; i < (int)array.Length; i++)
		{
			BaseEntity baseEntity1 = array[i];
			if (baseEntity1.IsValid())
			{
				BaseCombatEntity baseCombatEntity = baseEntity1 as BaseCombatEntity;
				if (baseCombatEntity != null)
				{
					baseCombatEntity.Hurt(this.DamagePerSecond * (1f / this.DamageTickRate), this.damageType, baseEntity, true);
				}
			}
		}
	}
}