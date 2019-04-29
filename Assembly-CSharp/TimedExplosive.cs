using Rust;
using Rust.Ai;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TimedExplosive : BaseEntity
{
	public float timerAmountMin = 10f;

	public float timerAmountMax = 20f;

	public float minExplosionRadius;

	public float explosionRadius = 10f;

	public bool canStick;

	public bool onlyDamageParent;

	public GameObjectRef explosionEffect;

	public GameObjectRef stickEffect;

	public GameObjectRef bounceEffect;

	public bool explosionUsesForward;

	public bool waterCausesExplosion;

	public List<DamageTypeEntry> damageTypes = new List<DamageTypeEntry>();

	[NonSerialized]
	private float lastBounceTime;

	public TimedExplosive()
	{
	}

	public virtual bool CanStickTo(BaseEntity entity)
	{
		return entity.GetComponent<DecorDeployable>() == null;
	}

	private void DoBounceEffect()
	{
		if (!this.bounceEffect.isValid)
		{
			return;
		}
		if (Time.time - this.lastBounceTime < 0.2f)
		{
			return;
		}
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component && component.velocity.magnitude < 1f)
		{
			return;
		}
		if (this.bounceEffect.isValid)
		{
			Effect.server.Run(this.bounceEffect.resourcePath, base.transform.position, Vector3.up, null, true);
		}
		this.lastBounceTime = Time.time;
	}

	private void DoCollisionStick(Collision collision, BaseEntity ent)
	{
		this.DoStick(collision.contacts[0].point, collision.contacts[0].normal, ent);
	}

	public void DoStick(Vector3 position, Vector3 normal, BaseEntity ent)
	{
		if (ent == null)
		{
			return;
		}
		if (ent is TimedExplosive)
		{
			if (!ent.HasParent())
			{
				return;
			}
			position = ent.transform.position;
			ent = ent.parentEntity.Get(true);
		}
		this.SetMotionEnabled(false);
		this.SetCollisionEnabled(false);
		if (base.HasChild(ent))
		{
			return;
		}
		base.transform.position = position;
		base.transform.rotation = Quaternion.LookRotation(normal, base.transform.up);
		base.SetParent(ent, StringPool.closest, true, false);
		if (this.stickEffect.isValid)
		{
			Effect.server.Run(this.stickEffect.resourcePath, base.transform.position, Vector3.up, null, true);
		}
		base.ReceiveCollisionMessages(false);
	}

	public virtual void Explode()
	{
		BaseCombatEntity i;
		Sensation sensation;
		base.GetComponent<Collider>().enabled = false;
		if (this.explosionEffect.isValid)
		{
			Effect.server.Run(this.explosionEffect.resourcePath, base.PivotPoint(), (this.explosionUsesForward ? base.transform.forward : Vector3.up), null, true);
		}
		if (this.damageTypes.Count > 0)
		{
			if (!this.onlyDamageParent)
			{
				DamageUtil.RadiusDamage(this.creatorEntity, base.LookupPrefab(), base.CenterPoint(), this.minExplosionRadius, this.explosionRadius, this.damageTypes, 1075980544, true);
				if (this.creatorEntity != null && this.damageTypes != null)
				{
					float single = 0f;
					foreach (DamageTypeEntry damageType in this.damageTypes)
					{
						single += damageType.amount;
					}
					sensation = new Sensation()
					{
						Type = SensationType.Explosion,
						Position = this.creatorEntity.transform.position,
						Radius = this.explosionRadius * 17f,
						DamagePotential = single,
						InitiatorPlayer = this.creatorEntity as BasePlayer,
						Initiator = this.creatorEntity
					};
					Sense.Stimulate(sensation);
				}
			}
			else
			{
				DamageUtil.RadiusDamage(this.creatorEntity, base.LookupPrefab(), base.CenterPoint(), this.minExplosionRadius, this.explosionRadius, this.damageTypes, 141568, true);
				BaseEntity parentEntity = base.GetParentEntity();
				for (i = parentEntity as BaseCombatEntity; i == null && parentEntity != null && parentEntity.HasParent(); i = parentEntity as BaseCombatEntity)
				{
					parentEntity = parentEntity.GetParentEntity();
				}
				if (i)
				{
					HitInfo hitInfo = new HitInfo()
					{
						Initiator = this.creatorEntity,
						WeaponPrefab = base.LookupPrefab()
					};
					hitInfo.damageTypes.Add(this.damageTypes);
					i.Hurt(hitInfo);
				}
				if (this.creatorEntity != null && this.damageTypes != null)
				{
					float single1 = 0f;
					foreach (DamageTypeEntry damageTypeEntry in this.damageTypes)
					{
						single1 += damageTypeEntry.amount;
					}
					sensation = new Sensation()
					{
						Type = SensationType.Explosion,
						Position = this.creatorEntity.transform.position,
						Radius = this.explosionRadius * 17f,
						DamagePotential = single1,
						InitiatorPlayer = this.creatorEntity as BasePlayer,
						Initiator = this.creatorEntity
					};
					Sense.Stimulate(sensation);
				}
			}
		}
		if (base.IsDestroyed)
		{
			return;
		}
		base.Kill(BaseNetworkable.DestroyMode.Gib);
	}

	public virtual float GetRandomTimerTime()
	{
		return UnityEngine.Random.Range(this.timerAmountMin, this.timerAmountMax);
	}

	public bool IsStuck()
	{
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component && !component.isKinematic)
		{
			return false;
		}
		Collider collider = base.GetComponent<Collider>();
		if (collider && collider.enabled)
		{
			return false;
		}
		return this.parentEntity.IsValid(true);
	}

	public override void OnCollision(Collision collision, BaseEntity hitEntity)
	{
		if (this.canStick && !this.IsStuck())
		{
			bool flag = true;
			if (hitEntity)
			{
				flag = this.CanStickTo(hitEntity);
				if (!flag)
				{
					Collider component = base.GetComponent<Collider>();
					if (collision.collider != null && component != null)
					{
						Physics.IgnoreCollision(collision.collider, component);
					}
				}
			}
			if (flag)
			{
				this.DoCollisionStick(collision, hitEntity);
			}
		}
		this.DoBounceEffect();
	}

	internal override void OnParentRemoved()
	{
		this.UnStick();
	}

	public override bool PhysicsDriven()
	{
		return true;
	}

	public virtual void ProjectileImpact(RaycastHit info)
	{
		this.Explode();
	}

	public override void ServerInit()
	{
		this.lastBounceTime = Time.time;
		base.ServerInit();
		this.SetFuse(this.GetRandomTimerTime());
		base.ReceiveCollisionMessages(true);
		if (this.waterCausesExplosion)
		{
			base.InvokeRepeating(new Action(this.WaterCheck), 0f, 0.5f);
		}
	}

	public virtual void SetCollisionEnabled(bool wantsCollision)
	{
		Collider component = base.GetComponent<Collider>();
		if (component.enabled != wantsCollision)
		{
			component.enabled = wantsCollision;
		}
	}

	public virtual void SetFuse(float fuseLength)
	{
		if (base.isServer)
		{
			TimedExplosive timedExplosive = this;
			base.Invoke(new Action(timedExplosive.Explode), fuseLength);
		}
	}

	public virtual void SetMotionEnabled(bool wantsMotion)
	{
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component)
		{
			component.useGravity = wantsMotion;
			component.isKinematic = !wantsMotion;
		}
	}

	private void UnStick()
	{
		if (!base.GetParentEntity())
		{
			return;
		}
		base.SetParent(null, true, true);
		this.SetMotionEnabled(true);
		this.SetCollisionEnabled(true);
		base.ReceiveCollisionMessages(true);
	}

	public void WaterCheck()
	{
		if (this.waterCausesExplosion && this.WaterFactor() >= 0.5f)
		{
			this.Explode();
		}
	}
}