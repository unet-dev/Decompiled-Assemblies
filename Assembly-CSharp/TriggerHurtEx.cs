using Rust;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TriggerHurtEx : TriggerBase, IServerComponent
{
	public float repeatRate = 0.1f;

	[Header("On Enter")]
	public List<DamageTypeEntry> damageOnEnter;

	public GameObjectRef effectOnEnter;

	public TriggerHurtEx.HurtType hurtTypeOnEnter;

	[Header("On Timer (damage per second)")]
	public List<DamageTypeEntry> damageOnTimer;

	public GameObjectRef effectOnTimer;

	public TriggerHurtEx.HurtType hurtTypeOnTimer;

	[Header("On Move (damage per meter)")]
	public List<DamageTypeEntry> damageOnMove;

	public GameObjectRef effectOnMove;

	public TriggerHurtEx.HurtType hurtTypeOnMove;

	[Header("On Leave")]
	public List<DamageTypeEntry> damageOnLeave;

	public GameObjectRef effectOnLeave;

	public TriggerHurtEx.HurtType hurtTypeOnLeave;

	public bool damageEnabled = true;

	internal Dictionary<BaseEntity, TriggerHurtEx.EntityTriggerInfo> entityInfo;

	internal List<BaseEntity> entityAddList;

	internal List<BaseEntity> entityLeaveList;

	public TriggerHurtEx()
	{
	}

	internal void DoDamage(BaseEntity ent, TriggerHurtEx.HurtType type, List<DamageTypeEntry> damage, GameObjectRef effect, float multiply = 1f)
	{
		if (!this.damageEnabled)
		{
			return;
		}
		using (TimeWarning timeWarning = TimeWarning.New("TriggerHurtEx.DoDamage", 0.1f))
		{
			if (damage != null && damage.Count > 0)
			{
				BaseCombatEntity baseCombatEntity = ent as BaseCombatEntity;
				if (baseCombatEntity)
				{
					HitInfo hitInfo = new HitInfo();
					hitInfo.damageTypes.Add(damage);
					hitInfo.damageTypes.ScaleAll(multiply);
					hitInfo.DoHitEffects = true;
					hitInfo.DidHit = true;
					hitInfo.Initiator = base.gameObject.ToBaseEntity();
					hitInfo.PointStart = base.transform.position;
					hitInfo.PointEnd = baseCombatEntity.transform.position;
					if (type != TriggerHurtEx.HurtType.Simple)
					{
						baseCombatEntity.OnAttacked(hitInfo);
					}
					else
					{
						baseCombatEntity.Hurt(hitInfo);
					}
				}
			}
			if (effect.isValid)
			{
				Effect.server.Run(effect.resourcePath, ent, StringPool.closest, base.transform.position, Vector3.up, null, false);
			}
		}
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

	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		if (ent == null)
		{
			return;
		}
		if (this.entityAddList == null)
		{
			this.entityAddList = new List<BaseEntity>();
		}
		this.entityAddList.Add(ent);
		base.Invoke(new Action(this.ProcessQueues), 0.1f);
	}

	internal override void OnEntityLeave(BaseEntity ent)
	{
		base.OnEntityLeave(ent);
		if (ent == null)
		{
			return;
		}
		if (this.entityLeaveList == null)
		{
			this.entityLeaveList = new List<BaseEntity>();
		}
		this.entityLeaveList.Add(ent);
		base.Invoke(new Action(this.ProcessQueues), 0.1f);
	}

	internal override void OnObjects()
	{
		base.InvokeRepeating(new Action(this.OnTick), this.repeatRate, this.repeatRate);
	}

	private void OnTick()
	{
		this.ProcessQueues();
		if (this.entityInfo != null)
		{
			KeyValuePair<BaseEntity, TriggerHurtEx.EntityTriggerInfo>[] array = this.entityInfo.ToArray<KeyValuePair<BaseEntity, TriggerHurtEx.EntityTriggerInfo>>();
			for (int i = 0; i < (int)array.Length; i++)
			{
				KeyValuePair<BaseEntity, TriggerHurtEx.EntityTriggerInfo> keyValuePair = array[i];
				if (keyValuePair.Key.IsValid())
				{
					Vector3 key = keyValuePair.Key.transform.position;
					float value = (key - keyValuePair.Value.lastPosition).magnitude;
					if (value > 0.01f)
					{
						keyValuePair.Value.lastPosition = key;
						this.DoDamage(keyValuePair.Key, this.hurtTypeOnMove, this.damageOnMove, this.effectOnMove, value);
					}
					this.DoDamage(keyValuePair.Key, this.hurtTypeOnTimer, this.damageOnTimer, this.effectOnTimer, this.repeatRate);
				}
			}
		}
	}

	private void ProcessQueues()
	{
		if (this.entityAddList != null)
		{
			foreach (BaseEntity baseEntity in this.entityAddList)
			{
				if (!baseEntity.IsValid())
				{
					continue;
				}
				this.DoDamage(baseEntity, this.hurtTypeOnEnter, this.damageOnEnter, this.effectOnEnter, 1f);
				if (this.entityInfo == null)
				{
					this.entityInfo = new Dictionary<BaseEntity, TriggerHurtEx.EntityTriggerInfo>();
				}
				if (this.entityInfo.ContainsKey(baseEntity))
				{
					continue;
				}
				this.entityInfo.Add(baseEntity, new TriggerHurtEx.EntityTriggerInfo()
				{
					lastPosition = baseEntity.transform.position
				});
			}
			this.entityAddList = null;
		}
		if (this.entityLeaveList != null)
		{
			foreach (BaseEntity baseEntity1 in this.entityLeaveList)
			{
				if (!baseEntity1.IsValid())
				{
					continue;
				}
				this.DoDamage(baseEntity1, this.hurtTypeOnLeave, this.damageOnLeave, this.effectOnLeave, 1f);
				if (this.entityInfo == null)
				{
					continue;
				}
				this.entityInfo.Remove(baseEntity1);
				if (this.entityInfo.Count != 0)
				{
					continue;
				}
				this.entityInfo = null;
			}
			this.entityLeaveList.Clear();
		}
	}

	public class EntityTriggerInfo
	{
		public Vector3 lastPosition;

		public EntityTriggerInfo()
		{
		}
	}

	public enum HurtType
	{
		Simple,
		IncludeBleedingAndScreenShake
	}
}