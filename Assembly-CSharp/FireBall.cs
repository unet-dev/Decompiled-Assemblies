using Facepunch;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : BaseEntity, ISplashable
{
	public float lifeTimeMin = 20f;

	public float lifeTimeMax = 40f;

	public ParticleSystem[] movementSystems;

	public ParticleSystem[] restingSystems;

	[NonSerialized]
	public float generation;

	public GameObjectRef spreadSubEntity;

	public float tickRate = 0.5f;

	public float damagePerSecond = 2f;

	public float radius = 0.5f;

	public int waterToExtinguish = 200;

	public bool canMerge;

	public LayerMask AttackLayers = 1219701521;

	private Vector3 lastPos = Vector3.zero;

	private float deathTime;

	private int wetness;

	public FireBall()
	{
	}

	public void AddLife(float amountToAdd)
	{
		float single = Mathf.Clamp(this.GetDeathTime() + amountToAdd, 0f, this.MaxLifeTime());
		base.Invoke(new Action(this.Extinguish), single);
		this.deathTime = single;
	}

	public bool CanMerge()
	{
		if (!this.canMerge)
		{
			return false;
		}
		return this.TimeLeft() < this.MaxLifeTime() * 0.8f;
	}

	public void DoRadialDamage()
	{
		List<Collider> list = Pool.GetList<Collider>();
		Vector3 vector3 = base.transform.position + new Vector3(0f, this.radius * 0.75f, 0f);
		Vis.Colliders<Collider>(vector3, this.radius, list, this.AttackLayers, QueryTriggerInteraction.Collide);
		HitInfo hitInfo = new HitInfo()
		{
			DoHitEffects = true,
			DidHit = true,
			HitBone = 0,
			Initiator = (this.creatorEntity == null ? base.gameObject.ToBaseEntity() : this.creatorEntity),
			PointStart = base.transform.position
		};
		foreach (Collider collider in list)
		{
			if (collider.isTrigger && (collider.gameObject.layer == 29 || collider.gameObject.layer == 18))
			{
				continue;
			}
			BaseCombatEntity baseEntity = collider.gameObject.ToBaseEntity() as BaseCombatEntity;
			if (baseEntity == null || !baseEntity.isServer || !baseEntity.IsAlive() || !baseEntity.IsVisible(vector3, Single.PositiveInfinity))
			{
				continue;
			}
			if (baseEntity is BasePlayer)
			{
				Effect.server.Run("assets/bundled/prefabs/fx/impacts/additive/fire.prefab", baseEntity, 0, new Vector3(0f, 1f, 0f), Vector3.up, null, false);
			}
			hitInfo.PointEnd = baseEntity.transform.position;
			hitInfo.HitPositionWorld = baseEntity.transform.position;
			hitInfo.damageTypes.Set(DamageType.Heat, this.damagePerSecond * this.tickRate);
			baseEntity.OnAttacked(hitInfo);
		}
		Pool.FreeList<Collider>(ref list);
	}

	public int DoSplash(ItemDefinition splashType, int amount)
	{
		this.wetness += amount;
		return amount;
	}

	public void Extinguish()
	{
		base.CancelInvoke(new Action(this.Extinguish));
		if (!base.IsDestroyed)
		{
			base.Kill(BaseNetworkable.DestroyMode.None);
		}
	}

	public float GetDeathTime()
	{
		return this.deathTime;
	}

	public bool IsResting()
	{
		return base.HasFlag(BaseEntity.Flags.OnFire);
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
	}

	public float MaxLifeTime()
	{
		return this.lifeTimeMax * 2.5f;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRepeating(new Action(this.Think), UnityEngine.Random.Range(0f, 1f), this.tickRate);
		float single = UnityEngine.Random.Range(this.lifeTimeMin, this.lifeTimeMax);
		float single1 = single * UnityEngine.Random.Range(0.9f, 1.1f);
		base.Invoke(new Action(this.Extinguish), single1);
		base.Invoke(new Action(this.TryToSpread), single * UnityEngine.Random.Range(0.3f, 0.5f));
		this.deathTime = Time.realtimeSinceStartup + single1;
	}

	public void SetGeneration(int gen)
	{
		this.generation = (float)gen;
	}

	public void SetResting(bool isResting)
	{
		if (isResting != this.IsResting() & isResting && this.CanMerge())
		{
			List<Collider> list = Pool.GetList<Collider>();
			Vis.Colliders<Collider>(base.transform.position, 0.5f, list, 512, QueryTriggerInteraction.Collide);
			foreach (Collider collider in list)
			{
				BaseEntity baseEntity = collider.gameObject.ToBaseEntity();
				if (!baseEntity)
				{
					continue;
				}
				FireBall server = baseEntity.ToServer<FireBall>();
				if (!server || !server.CanMerge() || !(server != this))
				{
					continue;
				}
				server.Invoke(new Action(this.Extinguish), 1f);
				server.canMerge = false;
				this.AddLife(server.TimeLeft() * 0.25f);
			}
			Pool.FreeList<Collider>(ref list);
		}
		base.SetFlag(BaseEntity.Flags.OnFire, isResting, false, true);
	}

	public void Think()
	{
		if (!base.isServer)
		{
			return;
		}
		this.SetResting(Vector3.Distance(this.lastPos, base.transform.localPosition) < 0.25f);
		this.lastPos = base.transform.localPosition;
		if (this.IsResting())
		{
			this.DoRadialDamage();
		}
		if (this.WaterFactor() > 0.5f)
		{
			this.Extinguish();
		}
		if (this.wetness > this.waterToExtinguish)
		{
			this.Extinguish();
		}
	}

	public float TimeLeft()
	{
		float single = this.deathTime - Time.realtimeSinceStartup;
		if (single < 0f)
		{
			single = 0f;
		}
		return single;
	}

	public void TryToSpread()
	{
		if (UnityEngine.Random.Range(0f, 1f) >= 0.9f - this.generation * 0.1f)
		{
			return;
		}
		if (this.spreadSubEntity.isValid)
		{
			GameManager gameManager = GameManager.server;
			string str = this.spreadSubEntity.resourcePath;
			Vector3 vector3 = new Vector3();
			Quaternion quaternion = new Quaternion();
			BaseEntity baseEntity = gameManager.CreateEntity(str, vector3, quaternion, true);
			if (baseEntity)
			{
				baseEntity.transform.position = base.transform.position + (Vector3.up * 0.25f);
				baseEntity.Spawn();
				Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(45f, Vector3.up, true);
				baseEntity.creatorEntity = (this.creatorEntity == null ? baseEntity : this.creatorEntity);
				baseEntity.SetVelocity(modifiedAimConeDirection * UnityEngine.Random.Range(5f, 8f));
				baseEntity.SendMessage("SetGeneration", this.generation + 1f);
			}
		}
	}

	public bool wantsSplash(ItemDefinition splashType, int amount)
	{
		return !base.IsDestroyed;
	}
}