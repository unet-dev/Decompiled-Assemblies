using Oxide.Core;
using Rust;
using System;
using UnityEngine;

public class CH47HelicopterAIController : CH47Helicopter
{
	public GameObjectRef scientistPrefab;

	public GameObjectRef dismountablePrefab;

	public GameObjectRef weakDismountablePrefab;

	public float maxTiltAngle = 0.3f;

	public float AiAltitudeForce = 10000f;

	public GameObjectRef lockedCratePrefab;

	public const BaseEntity.Flags Flag_Damaged = BaseEntity.Flags.Reserved7;

	public const BaseEntity.Flags Flag_NearDeath = BaseEntity.Flags.OnFire;

	public const BaseEntity.Flags Flag_DropDoorOpen = BaseEntity.Flags.Reserved8;

	public GameObject triggerHurt;

	public int numCrates = 1;

	private bool shouldLand;

	public Vector3 landingTarget;

	public bool aimDirOverride;

	public Vector3 _aimDirection = Vector3.forward;

	public Vector3 _moveTarget = Vector3.zero;

	public int lastAltitudeCheckFrame;

	public float altOverride;

	public float currentDesiredAltitude;

	private bool altitudeProtection = true;

	public float hoverHeight = 30f;

	public CH47HelicopterAIController()
	{
	}

	public override void AttemptMount(BasePlayer player)
	{
		if (Interface.CallHook("CanUseHelicopter", player, this) != null)
		{
			return;
		}
		base.AttemptMount(player);
	}

	public void CalculateDesiredAltitude()
	{
		this.CalculateOverrideAltitude();
		if (this.altOverride > this.currentDesiredAltitude)
		{
			this.currentDesiredAltitude = this.altOverride;
			return;
		}
		this.currentDesiredAltitude = Mathf.MoveTowards(this.currentDesiredAltitude, this.altOverride, Time.fixedDeltaTime * 5f);
	}

	public float CalculateOverrideAltitude()
	{
		RaycastHit raycastHit;
		RaycastHit raycastHit1;
		if (Time.frameCount == this.lastAltitudeCheckFrame)
		{
			return this.altOverride;
		}
		this.lastAltitudeCheckFrame = Time.frameCount;
		float moveTarget = this.GetMoveTarget().y;
		float single = Mathf.Max(TerrainMeta.WaterMap.GetHeight(this.GetMoveTarget()), TerrainMeta.HeightMap.GetHeight(this.GetMoveTarget()));
		float single1 = Mathf.Max(moveTarget, single + this.hoverHeight);
		if (this.altitudeProtection)
		{
			Vector3 vector3 = (this.rigidBody.velocity.magnitude < 0.1f ? base.transform.forward : this.rigidBody.velocity.normalized);
			Vector3 vector31 = Vector3.Cross(Vector3.Cross(base.transform.up, vector3), Vector3.up) + (Vector3.down * 0.3f);
			Vector3 vector32 = vector31.normalized;
			if (Physics.SphereCast(base.transform.position - (vector32 * 20f), 20f, vector32, out raycastHit, 75f, 1218511105) && Physics.SphereCast(raycastHit.point + (Vector3.up * 200f), 20f, Vector3.down, out raycastHit1, 200f, 1218511105))
			{
				single1 = raycastHit1.point.y + this.hoverHeight;
			}
		}
		this.altOverride = single1;
		return this.altOverride;
	}

	public void CancelAnger()
	{
		if (base.SecondsSinceAttacked > 120f)
		{
			this.UnHostile();
			base.CancelInvoke(new Action(this.UnHostile));
		}
	}

	public bool CanDropCrate()
	{
		object obj = Interface.CallHook("CanHelicopterDropCrate", this);
		if (obj as bool)
		{
			return (bool)obj;
		}
		return this.numCrates > 0;
	}

	public void ClearLandingTarget()
	{
		this.shouldLand = false;
	}

	public void DelayedKill()
	{
		BaseVehicle.MountPointInfo[] mountPointInfoArray = this.mountPoints;
		for (int i = 0; i < (int)mountPointInfoArray.Length; i++)
		{
			BaseVehicle.MountPointInfo mountPointInfo = mountPointInfoArray[i];
			if (mountPointInfo.mountable != null)
			{
				BasePlayer mounted = mountPointInfo.mountable.GetMounted();
				if (mounted && mounted.transform != null && !mounted.IsDestroyed && !mounted.IsDead() && mounted.IsNpc)
				{
					mounted.Kill(BaseNetworkable.DestroyMode.None);
				}
			}
		}
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	public override void DestroyShared()
	{
		if (base.isServer)
		{
			BaseVehicle.MountPointInfo[] mountPointInfoArray = this.mountPoints;
			for (int i = 0; i < (int)mountPointInfoArray.Length; i++)
			{
				BaseVehicle.MountPointInfo mountPointInfo = mountPointInfoArray[i];
				if (mountPointInfo.mountable != null)
				{
					BasePlayer mounted = mountPointInfo.mountable.GetMounted();
					if (mounted && mounted.transform != null && !mounted.IsDestroyed && !mounted.IsDead() && mounted.IsNpc)
					{
						mounted.Kill(BaseNetworkable.DestroyMode.None);
					}
				}
			}
		}
		base.DestroyShared();
	}

	public override void DismountAllPlayers()
	{
		BaseVehicle.MountPointInfo[] mountPointInfoArray = this.mountPoints;
		for (int i = 0; i < (int)mountPointInfoArray.Length; i++)
		{
			BaseVehicle.MountPointInfo mountPointInfo = mountPointInfoArray[i];
			if (mountPointInfo.mountable != null)
			{
				BasePlayer mounted = mountPointInfo.mountable.GetMounted();
				if (mounted)
				{
					mounted.Hurt(10000f, DamageType.Explosion, this, false);
				}
			}
		}
	}

	public void DropCrate()
	{
		if (this.numCrates <= 0)
		{
			return;
		}
		Vector3 vector3 = base.transform.position + (Vector3.down * 5f);
		Quaternion quaternion = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.lockedCratePrefab.resourcePath, vector3, quaternion, true);
		if (baseEntity)
		{
			Interface.CallHook("OnHelicopterDropCrate", this);
			baseEntity.SendMessage("SetWasDropped");
			baseEntity.Spawn();
		}
		this.numCrates--;
	}

	public void EnableFacingOverride(bool enabled)
	{
		this.aimDirOverride = enabled;
	}

	public Vector3 GetAimDirectionOverride()
	{
		return this._aimDirection;
	}

	public Vector3 GetMoveTarget()
	{
		return this._moveTarget;
	}

	public Vector3 GetPosition()
	{
		return base.transform.position;
	}

	public void InitiateAnger()
	{
		base.CancelInvoke(new Action(this.UnHostile));
		base.Invoke(new Action(this.UnHostile), 120f);
		BaseVehicle.MountPointInfo[] mountPointInfoArray = this.mountPoints;
		for (int i = 0; i < (int)mountPointInfoArray.Length; i++)
		{
			BaseVehicle.MountPointInfo mountPointInfo = mountPointInfoArray[i];
			if (mountPointInfo.mountable != null)
			{
				BasePlayer mounted = mountPointInfo.mountable.GetMounted();
				if (mounted)
				{
					NPCPlayerApex nPCPlayerApex = mounted as NPCPlayerApex;
					if (nPCPlayerApex)
					{
						nPCPlayerApex.Stats.Hostility = 1f;
						nPCPlayerApex.Stats.Defensiveness = 1f;
					}
				}
			}
		}
	}

	public void MaintainAIAltutide()
	{
		Vector3 vector3 = base.transform.position + this.rigidBody.velocity;
		float single = this.currentDesiredAltitude;
		float single1 = vector3.y;
		float single2 = Mathf.Abs(single - single1);
		bool flag = single > single1;
		float single3 = Mathf.InverseLerp(0f, 10f, single2) * this.AiAltitudeForce * (flag ? 1f : -1f);
		this.rigidBody.AddForce(Vector3.up * single3, ForceMode.Force);
	}

	public override void MounteeTookDamage(BasePlayer mountee, HitInfo info)
	{
		this.InitiateAnger();
	}

	public override void OnAttacked(HitInfo info)
	{
		if (Interface.CallHook("OnHelicopterAttacked", this) != null)
		{
			return;
		}
		base.OnAttacked(info);
		this.InitiateAnger();
		base.SetFlag(BaseEntity.Flags.Reserved7, base.healthFraction <= 0.8f, false, true);
		base.SetFlag(BaseEntity.Flags.OnFire, base.healthFraction <= 0.33f, false, true);
	}

	public override void OnKilled(HitInfo info)
	{
		if (Interface.CallHook("OnHelicopterKilled", this) != null)
		{
			return;
		}
		if (!this.OutOfCrates())
		{
			this.DropCrate();
		}
		base.OnKilled(info);
	}

	public bool OutOfCrates()
	{
		object obj = Interface.CallHook("OnHelicopterOutOfCrates", this);
		if (obj as bool)
		{
			return (bool)obj;
		}
		return this.numCrates <= 0;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		base.Invoke(new Action(this.SpawnScientists), 0.25f);
		this.SetMoveTarget(base.transform.position);
	}

	public void SetAimDirection(Vector3 dir)
	{
		this._aimDirection = dir;
	}

	public void SetAltitudeProtection(bool on)
	{
		this.altitudeProtection = on;
	}

	public override void SetDefaultInputState()
	{
		this.currentInputState.Reset();
		Vector3 moveTarget = this.GetMoveTarget();
		Vector3 vector3 = Vector3.Cross(base.transform.right, Vector3.up);
		Vector3 vector31 = Vector3.Cross(Vector3.up, vector3);
		float single = -Vector3.Dot(Vector3.up, base.transform.right);
		float single1 = Vector3.Dot(Vector3.up, base.transform.forward);
		float single2 = Vector3Ex.Distance2D(base.transform.position, moveTarget);
		float single3 = base.transform.position.y;
		float single4 = this.currentDesiredAltitude;
		Vector3 vector32 = base.transform.position + (base.transform.forward * 10f);
		vector32.y = single4;
		Vector3 vector33 = Vector3Ex.Direction2D(moveTarget, base.transform.position);
		float single5 = -Vector3.Dot(vector33, vector31);
		float single6 = Vector3.Dot(vector33, vector3);
		float single7 = Mathf.InverseLerp(0f, 25f, single2);
		if (single6 <= 0f)
		{
			float single8 = 1f - Mathf.InverseLerp(0f, this.maxTiltAngle, single1);
			this.currentInputState.pitch = 1f * single6 * single8 * single7;
		}
		else
		{
			float single9 = Mathf.InverseLerp(-this.maxTiltAngle, 0f, single1);
			this.currentInputState.pitch = 1f * single6 * single9 * single7;
		}
		if (single5 <= 0f)
		{
			float single10 = 1f - Mathf.InverseLerp(0f, this.maxTiltAngle, single);
			this.currentInputState.roll = 1f * single5 * single10 * single7;
		}
		else
		{
			float single11 = Mathf.InverseLerp(-this.maxTiltAngle, 0f, single);
			this.currentInputState.roll = 1f * single5 * single11 * single7;
		}
		float single12 = Mathf.Abs(single4 - single3);
		float single13 = 1f - Mathf.InverseLerp(10f, 30f, single12);
		this.currentInputState.pitch *= single13;
		this.currentInputState.roll *= single13;
		float single14 = this.maxTiltAngle;
		float single15 = Mathf.InverseLerp(0f + Mathf.Abs(this.currentInputState.pitch) * single14, single14 + Mathf.Abs(this.currentInputState.pitch) * single14, Mathf.Abs(single1));
		BaseHelicopterVehicle.HelicopterInputState helicopterInputState = this.currentInputState;
		helicopterInputState.pitch = helicopterInputState.pitch + single15 * (single1 < 0f ? -1f : 1f);
		float single16 = Mathf.InverseLerp(0f + Mathf.Abs(this.currentInputState.roll) * single14, single14 + Mathf.Abs(this.currentInputState.roll) * single14, Mathf.Abs(single));
		BaseHelicopterVehicle.HelicopterInputState helicopterInputState1 = this.currentInputState;
		helicopterInputState1.roll = helicopterInputState1.roll + single16 * (single < 0f ? -1f : 1f);
		if (this.aimDirOverride || single2 > 30f)
		{
			Vector3 vector34 = (this.aimDirOverride ? this.GetAimDirectionOverride() : Vector3Ex.Direction2D(this.GetMoveTarget(), base.transform.position));
			Vector3 vector35 = (this.aimDirOverride ? this.GetAimDirectionOverride() : Vector3Ex.Direction2D(this.GetMoveTarget(), base.transform.position));
			float single17 = Vector3.Dot(vector31, vector34);
			float single18 = Vector3.Angle(vector3, vector35);
			float single19 = Mathf.InverseLerp(0f, 70f, Mathf.Abs(single18));
			this.currentInputState.yaw = (single17 > 0f ? 1f : 0f);
			BaseHelicopterVehicle.HelicopterInputState helicopterInputState2 = this.currentInputState;
			helicopterInputState2.yaw = helicopterInputState2.yaw - (single17 < 0f ? 1f : 0f);
			this.currentInputState.yaw *= single19;
		}
		float single20 = Mathf.InverseLerp(5f, 30f, single2);
		this.currentInputState.throttle = single20;
	}

	public void SetDropDoorOpen(bool open)
	{
		if (Interface.CallHook("OnHelicopterDropDoorOpen", this) != null)
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.Reserved8, open, false, true);
	}

	public void SetLandingTarget(Vector3 target)
	{
		this.shouldLand = true;
		this.landingTarget = target;
		this.numCrates = 0;
	}

	public void SetMinHoverHeight(float newHeight)
	{
		this.hoverHeight = newHeight;
	}

	public void SetMoveTarget(Vector3 position)
	{
		this._moveTarget = position;
	}

	public bool ShouldLand()
	{
		return this.shouldLand;
	}

	public void SpawnPassenger(Vector3 spawnPos, string prefabPath)
	{
		Quaternion quaternion = Quaternion.identity;
		HumanNPC component = GameManager.server.CreateEntity(prefabPath, spawnPos, quaternion, true).GetComponent<HumanNPC>();
		component.Spawn();
		component.SetNavMeshEnabled(false);
		this.AttemptMount(component);
	}

	public void SpawnPassenger(Vector3 spawnPos)
	{
		Quaternion quaternion = Quaternion.identity;
		HumanNPC component = GameManager.server.CreateEntity(this.dismountablePrefab.resourcePath, spawnPos, quaternion, true).GetComponent<HumanNPC>();
		component.Spawn();
		component.SetNavMeshEnabled(false);
		this.AttemptMount(component);
	}

	public void SpawnScientist(Vector3 spawnPos)
	{
		Quaternion quaternion = Quaternion.identity;
		NPCPlayerApex component = GameManager.server.CreateEntity(this.scientistPrefab.resourcePath, spawnPos, quaternion, true).GetComponent<NPCPlayerApex>();
		component.Spawn();
		component.Mount(this);
		component.Stats.VisionRange = 203f;
		component.Stats.DeaggroRange = 202f;
		component.Stats.AggressionRange = 201f;
		component.Stats.LongRange = 200f;
		component.Stats.Hostility = 0f;
		component.Stats.Defensiveness = 0f;
		component.Stats.OnlyAggroMarkedTargets = true;
		component.InitFacts();
	}

	public void SpawnScientists()
	{
		if (!this.shouldLand)
		{
			for (int i = 0; i < 4; i++)
			{
				Vector3 vector3 = base.transform.position + (base.transform.forward * 10f);
				this.SpawnScientist(vector3);
			}
			for (int j = 0; j < 1; j++)
			{
				Vector3 vector31 = base.transform.position - (base.transform.forward * 15f);
				this.SpawnScientist(vector31);
			}
			return;
		}
		float closest = CH47LandingZone.GetClosest(this.landingTarget).dropoffScale;
		int num = Mathf.FloorToInt((float)((int)this.mountPoints.Length - 2) * closest);
		for (int k = 0; k < num; k++)
		{
			Vector3 vector32 = base.transform.position + (base.transform.forward * 10f);
			this.SpawnPassenger(vector32, this.dismountablePrefab.resourcePath);
		}
		for (int l = 0; l < 1; l++)
		{
			Vector3 vector33 = base.transform.position - (base.transform.forward * 15f);
			this.SpawnPassenger(vector33);
		}
	}

	public void TriggeredEventSpawn()
	{
		float size = TerrainMeta.Size.x;
		float single = 30f;
		Vector3 vector3 = Vector3Ex.Range(-1f, 1f);
		vector3.y = 0f;
		vector3.Normalize();
		vector3 = vector3 * (size * 1f);
		vector3.y = single;
		base.transform.position = vector3;
	}

	public void UnHostile()
	{
		BaseVehicle.MountPointInfo[] mountPointInfoArray = this.mountPoints;
		for (int i = 0; i < (int)mountPointInfoArray.Length; i++)
		{
			BaseVehicle.MountPointInfo mountPointInfo = mountPointInfoArray[i];
			if (mountPointInfo.mountable != null)
			{
				BasePlayer mounted = mountPointInfo.mountable.GetMounted();
				if (mounted)
				{
					NPCPlayerApex nPCPlayerApex = mounted as NPCPlayerApex;
					if (nPCPlayerApex)
					{
						nPCPlayerApex.Stats.Hostility = 0f;
						nPCPlayerApex.Stats.Defensiveness = 0f;
					}
				}
			}
		}
	}

	public override void VehicleFixedUpdate()
	{
		this.hoverForceScale = 1f;
		base.VehicleFixedUpdate();
		base.SetFlag(BaseEntity.Flags.Reserved5, TOD_Sky.Instance.IsNight, false, true);
		this.CalculateDesiredAltitude();
		this.MaintainAIAltutide();
	}
}