using Facepunch;
using Oxide.Core;
using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SamSite : ContainerIOEntity
{
	public Animator pitchAnimator;

	public GameObject yaw;

	public GameObject pitch;

	public GameObject gear;

	public Transform eyePoint;

	public float gearEpislonDegrees = 20f;

	public float turnSpeed = 1f;

	public float clientLerpSpeed = 1f;

	public Vector3 currentAimDir = Vector3.forward;

	public Vector3 targetAimDir = Vector3.forward;

	public BaseCombatEntity currentTarget;

	public float scanRadius = 350f;

	public GameObjectRef projectileTest;

	public GameObjectRef muzzleFlashTest;

	public bool staticRespawn;

	public ItemDefinition ammoType;

	[ServerVar(Help="targetmode, 1 = all air vehicles, 0 = only hot air ballons")]
	public static bool alltarget;

	[ServerVar(Help="how long until static sam sites auto repair")]
	public static float staticrepairseconds;

	public SoundDefinition yawMovementLoopDef;

	public float yawGainLerp = 8f;

	public float yawGainMovementSpeedMult = 0.1f;

	public SoundDefinition pitchMovementLoopDef;

	public float pitchGainLerp = 10f;

	public float pitchGainMovementSpeedMult = 0.5f;

	private Item ammoItem;

	public float lockOnTime;

	public float lastTargetVisibleTime;

	public Transform[] tubes;

	private int currentTubeIndex;

	private int firedCount;

	public float nextBurstTime;

	static SamSite()
	{
		SamSite.alltarget = false;
		SamSite.staticrepairseconds = 1200f;
	}

	public SamSite()
	{
	}

	public override int ConsumptionAmount()
	{
		return 25;
	}

	public override void Die(HitInfo info = null)
	{
		if (!this.staticRespawn)
		{
			base.Die(info);
			return;
		}
		this.currentTarget = null;
		Quaternion quaternion = Quaternion.LookRotation(this.currentAimDir, Vector3.up);
		quaternion = Quaternion.Euler(0f, quaternion.eulerAngles.y, 0f);
		this.currentAimDir = quaternion * Vector3.forward;
		base.Invoke(new Action(this.SelfHeal), SamSite.staticrepairseconds);
		this.lifestate = BaseCombatEntity.LifeState.Dead;
		base.health = 0f;
		base.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
	}

	public void EnsureReloaded()
	{
		if (!this.HasAmmo())
		{
			this.Reload();
		}
	}

	public Vector3 EntityCenterPoint(BaseEntity ent)
	{
		return ent.transform.TransformPoint(ent.bounds.center);
	}

	public void FireProjectile(Vector3 origin, Vector3 direction, BaseCombatEntity target)
	{
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.projectileTest.resourcePath, origin, Quaternion.LookRotation(direction, Vector3.up), true);
		if (baseEntity == null)
		{
			return;
		}
		baseEntity.creatorEntity = this;
		ServerProjectile component = baseEntity.GetComponent<ServerProjectile>();
		if (component)
		{
			component.InitializeVelocity(this.GetInheritedProjectileVelocity() + (direction * component.speed));
		}
		baseEntity.Spawn();
	}

	public void FixedUpdate()
	{
		Vector3 worldVelocity;
		Vector3 vector3 = this.currentAimDir;
		if (this.currentTarget != null && this.IsPowered())
		{
			float component = this.projectileTest.Get().GetComponent<ServerProjectile>().speed;
			Vector3 vector31 = this.EntityCenterPoint(this.currentTarget);
			float single = Vector3.Distance(vector31, this.eyePoint.transform.position);
			float single1 = single / component;
			Vector3 worldVelocity1 = vector31 + (this.currentTarget.GetWorldVelocity() * single1);
			single1 = Vector3.Distance(worldVelocity1, this.eyePoint.transform.position) / component;
			worldVelocity1 = vector31 + (this.currentTarget.GetWorldVelocity() * single1);
			if (this.currentTarget.GetWorldVelocity().magnitude > 0.1f)
			{
				float single2 = Mathf.Sin(Time.time * 3f) * (1f + single1 * 0.5f);
				worldVelocity = this.currentTarget.GetWorldVelocity();
				worldVelocity1 = worldVelocity1 + (worldVelocity.normalized * single2);
			}
			worldVelocity = worldVelocity1 - this.eyePoint.transform.position;
			this.currentAimDir = worldVelocity.normalized;
			if (single > this.scanRadius)
			{
				this.currentTarget = null;
			}
		}
		Quaternion quaternion = Quaternion.LookRotation(this.currentAimDir, base.transform.up);
		Vector3 vector32 = quaternion.eulerAngles;
		vector32 = BaseMountable.ConvertVector(vector32);
		float single3 = Mathf.InverseLerp(0f, 90f, -vector32.x);
		float single4 = Mathf.Lerp(15f, -75f, single3);
		Quaternion quaternion1 = Quaternion.Euler(0f, vector32.y, 0f);
		this.yaw.transform.localRotation = quaternion1;
		Quaternion quaternion2 = this.pitch.transform.localRotation;
		float single5 = quaternion2.eulerAngles.x;
		quaternion2 = this.pitch.transform.localRotation;
		Quaternion quaternion3 = Quaternion.Euler(single5, quaternion2.eulerAngles.y, single4);
		this.pitch.transform.localRotation = quaternion3;
		if (this.currentAimDir != vector3)
		{
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
	}

	public Vector3 GetAimDir()
	{
		return this.currentAimDir;
	}

	public virtual bool HasAmmo()
	{
		if (this.staticRespawn)
		{
			return true;
		}
		if (this.ammoItem == null || this.ammoItem.amount <= 0)
		{
			return false;
		}
		return this.ammoItem.parent == this.inventory;
	}

	public bool HasValidTarget()
	{
		return this.currentTarget != null;
	}

	public override bool IsPowered()
	{
		if (this.staticRespawn)
		{
			return true;
		}
		return base.HasFlag(BaseEntity.Flags.Reserved8);
	}

	public bool IsReloading()
	{
		return base.IsInvoking(new Action(this.Reload));
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
	}

	public void Reload()
	{
		if (this.staticRespawn)
		{
			return;
		}
		for (int i = 0; i < this.inventory.itemList.Count; i++)
		{
			Item item = this.inventory.itemList[i];
			if (item != null && item.info.itemid == this.ammoType.itemid && item.amount > 0)
			{
				this.ammoItem = item;
				return;
			}
		}
		this.ammoItem = null;
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.samSite = Pool.Get<SAMSite>();
		info.msg.samSite.aimDir = this.GetAimDir();
	}

	public void SelfHeal()
	{
		this.lifestate = BaseCombatEntity.LifeState.Alive;
		base.health = this.startHealth;
		base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRandomized(new Action(this.TargetScan), 1f, 3f, 1f);
		this.currentAimDir = base.transform.forward;
	}

	public void TargetScan()
	{
		if (!this.IsPowered())
		{
			this.lastTargetVisibleTime = 0f;
			return;
		}
		if (Time.time > this.lastTargetVisibleTime + 3f)
		{
			this.currentTarget = null;
		}
		if (this.HasValidTarget())
		{
			return;
		}
		if (this.IsDead())
		{
			return;
		}
		List<BaseCombatEntity> list = Pool.GetList<BaseCombatEntity>();
		Vis.Entities<BaseCombatEntity>(this.eyePoint.transform.position, this.scanRadius, list, 8192, QueryTriggerInteraction.Ignore);
		BaseCombatEntity baseCombatEntity = null;
		foreach (BaseCombatEntity baseCombatEntity1 in list)
		{
			if (this.EntityCenterPoint(baseCombatEntity1).y < this.eyePoint.transform.position.y || !baseCombatEntity1.IsVisible(this.eyePoint.transform.position, this.scanRadius * 2f))
			{
				continue;
			}
			if ((SamSite.alltarget || baseCombatEntity1.GetComponent<HotAirBalloon>() ? false : !baseCombatEntity1.GetComponent<MiniCopter>()))
			{
				continue;
			}
			baseCombatEntity = baseCombatEntity1;
		}
		if (baseCombatEntity != null && this.currentTarget != baseCombatEntity)
		{
			this.lockOnTime = Time.time + 0.5f;
		}
		this.currentTarget = baseCombatEntity;
		if (this.currentTarget != null)
		{
			this.lastTargetVisibleTime = Time.time;
		}
		Pool.FreeList<BaseCombatEntity>(ref list);
		if (this.currentTarget == null)
		{
			base.CancelInvoke(new Action(this.WeaponTick));
			return;
		}
		base.InvokeRandomized(new Action(this.WeaponTick), 0f, 0.5f, 0.2f);
	}

	public void WeaponTick()
	{
		if (this.IsDead())
		{
			return;
		}
		if (Time.time < this.lockOnTime)
		{
			return;
		}
		if (Time.time < this.nextBurstTime)
		{
			return;
		}
		if (!this.IsPowered())
		{
			this.firedCount = 0;
			return;
		}
		if (this.firedCount >= 6)
		{
			this.nextBurstTime = Time.time + 5f;
			this.firedCount = 0;
			return;
		}
		this.EnsureReloaded();
		if (!this.HasAmmo())
		{
			return;
		}
		if (Interface.CallHook("CanSamSiteShoot", this) != null)
		{
			return;
		}
		if (!this.staticRespawn && this.ammoItem != null)
		{
			this.ammoItem.UseItem(1);
		}
		this.firedCount++;
		this.FireProjectile(this.tubes[this.currentTubeIndex].position, this.currentAimDir, this.currentTarget);
		int num = this.currentTubeIndex + 1;
		Effect.server.Run(this.muzzleFlashTest.resourcePath, this, StringPool.Get(string.Concat("Tube ", num.ToString())), Vector3.zero, Vector3.up, null, false);
		this.currentTubeIndex++;
		if (this.currentTubeIndex >= (int)this.tubes.Length)
		{
			this.currentTubeIndex = 0;
		}
	}
}