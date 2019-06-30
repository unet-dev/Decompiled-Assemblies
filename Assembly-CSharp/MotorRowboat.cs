using ConVar;
using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using Rust;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;

public class MotorRowboat : MotorBoat
{
	protected const BaseEntity.Flags Flag_EngineOn = BaseEntity.Flags.Reserved1;

	protected const BaseEntity.Flags Flag_ThrottleOn = BaseEntity.Flags.Reserved2;

	protected const BaseEntity.Flags Flag_TurnLeft = BaseEntity.Flags.Reserved3;

	protected const BaseEntity.Flags Flag_TurnRight = BaseEntity.Flags.Reserved4;

	protected const BaseEntity.Flags Flag_Submerged = BaseEntity.Flags.Reserved5;

	protected const BaseEntity.Flags Flag_HasFuel = BaseEntity.Flags.Reserved6;

	protected const BaseEntity.Flags Flag_Stationary = BaseEntity.Flags.Reserved7;

	protected const BaseEntity.Flags Flag_RecentlyPushed = BaseEntity.Flags.Reserved8;

	private const float submergeFractionMinimum = 0.85f;

	[Header("Fuel")]
	public GameObjectRef fuelStoragePrefab;

	public Transform fuelStoragePoint;

	public EntityRef fuelStorageInstance;

	public float fuelPerSec;

	[Header("Storage")]
	public GameObjectRef storageUnitPrefab;

	public Transform storageUnitPoint;

	public EntityRef storageUnitInstance;

	[Header("Effects")]
	public Transform boatRear;

	public ParticleSystemContainer wakeEffect;

	public ParticleSystemContainer engineEffectIdle;

	public ParticleSystemContainer engineEffectThrottle;

	public Projector causticsProjector;

	public Transform causticsDepthTest;

	public Transform engineLeftHandPosition;

	public Transform engineRotate;

	public Transform propellerRotate;

	[ServerVar(Help="Population active on the server")]
	public static float population;

	[ServerVar(Help="How long before a boat is killed while outside")]
	public static float outsidedecayminutes;

	public Transform[] stationaryDismounts;

	public Collider mainCollider;

	private float nextFuelCheckTime;

	private bool cachedHasFuel;

	private float pendingFuel;

	private float lastUsedFuelTime;

	private float nextPushTime;

	private float lastHadDriverTime;

	public float angularDragBase = 0.5f;

	public float angularDragVelocity = 0.5f;

	public float landDrag = 0.2f;

	public float waterDrag = 0.8f;

	public float offAxisDrag = 1f;

	public float offAxisDot = 0.25f;

	private bool dying;

	private const float maxVelForStationaryDismount = 4f;

	[Header("Audio")]
	public BlendedSoundLoops engineLoops;

	public BlendedSoundLoops waterLoops;

	public SoundDefinition engineStartSoundDef;

	public SoundDefinition engineStopSoundDef;

	public SoundDefinition movementSplashAccentSoundDef;

	public SoundDefinition engineSteerSoundDef;

	public GameObjectRef pushLandEffect;

	public GameObjectRef pushWaterEffect;

	public float waterSpeedDivisor = 10f;

	public float turnPitchModScale = -0.25f;

	public float tiltPitchModScale = 0.3f;

	public float splashAccentFrequencyMin = 1f;

	public float splashAccentFrequencyMax = 10f;

	static MotorRowboat()
	{
		MotorRowboat.population = 4f;
		MotorRowboat.outsidedecayminutes = 180f;
	}

	public MotorRowboat()
	{
	}

	public void ActualDeath()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	public override void AttemptMount(BasePlayer player)
	{
		if (this.dying)
		{
			return;
		}
		base.AttemptMount(player);
	}

	public void BoatDecay()
	{
		if (this.dying || base.healthFraction == 0f)
		{
			return;
		}
		if (UnityEngine.Time.time < this.lastUsedFuelTime + 600f)
		{
			return;
		}
		float single = 1f / MotorRowboat.outsidedecayminutes;
		if (this.IsOutside())
		{
			this.Hurt(this.MaxHealth() * single, DamageType.Decay, this, false);
		}
	}

	public void CheckInvalidBoat()
	{
		if (this.fuelStorageInstance.IsValid(base.isServer) && this.storageUnitInstance.IsValid(base.isServer))
		{
			return;
		}
		Debug.Log("Destroying invalid boat ");
		base.Invoke(new Action(this.ActualDeath), 1f);
	}

	public override void DriverInput(InputState inputState, BasePlayer player)
	{
		base.DriverInput(inputState, player);
		this.lastHadDriverTime = UnityEngine.Time.time;
	}

	public override bool EngineOn()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	public void EngineToggle(bool wantsOn)
	{
		if (!this.HasFuel(true))
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.Reserved1, wantsOn, false, true);
	}

	public override bool GetDismountPosition(BasePlayer player, out Vector3 res)
	{
		if (this.myRigidBody.velocity.magnitude <= 4f)
		{
			List<Vector3> list = Facepunch.Pool.GetList<Vector3>();
			Transform[] transformArrays = this.stationaryDismounts;
			for (int i = 0; i < (int)transformArrays.Length; i++)
			{
				Transform transforms = transformArrays[i];
				if (this.ValidDismountPosition(transforms.transform.position))
				{
					list.Add(transforms.transform.position);
				}
			}
			if (list.Count > 0)
			{
				Vector3 vector3 = player.transform.position;
				list.Sort((Vector3 a, Vector3 b) => Vector3.Distance(a, vector3).CompareTo(Vector3.Distance(b, vector3)));
				res = list[0];
				Facepunch.Pool.FreeList<Vector3>(ref list);
				return true;
			}
			Facepunch.Pool.FreeList<Vector3>(ref list);
		}
		return base.GetDismountPosition(player, out res);
	}

	public int GetFuelAmount()
	{
		if (!this.fuelStorageInstance.IsValid(base.isServer))
		{
			return 0;
		}
		StorageContainer component = this.fuelStorageInstance.Get(base.isServer).GetComponent<StorageContainer>();
		if (component == null)
		{
			return 0;
		}
		Item slot = component.inventory.GetSlot(0);
		if (slot == null || slot.amount < 1)
		{
			return 0;
		}
		return slot.amount;
	}

	public override float GetSteering(BasePlayer player)
	{
		return 0f;
	}

	public virtual bool HasFuel(bool forceCheck = false)
	{
		if (UnityEngine.Time.time > this.nextFuelCheckTime | forceCheck)
		{
			this.cachedHasFuel = (float)this.GetFuelAmount() > 0f;
			this.nextFuelCheckTime = UnityEngine.Time.time + UnityEngine.Random.Range(1f, 2f);
		}
		return this.cachedHasFuel;
	}

	public override bool HasValidDismountPosition(BasePlayer player)
	{
		if (this.myRigidBody.velocity.magnitude <= 4f)
		{
			Transform[] transformArrays = this.stationaryDismounts;
			for (int i = 0; i < (int)transformArrays.Length; i++)
			{
				if (this.ValidDismountPosition(transformArrays[i].transform.position))
				{
					return true;
				}
			}
		}
		return base.HasValidDismountPosition(player);
	}

	public bool IsDriver(BasePlayer player)
	{
		return base.GetPlayerSeat(player) == 0;
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.motorBoat != null)
		{
			this.fuelStorageInstance.uid = info.msg.motorBoat.fuelStorageID;
			this.storageUnitInstance.uid = info.msg.motorBoat.storageid;
		}
	}

	public override bool MountEligable()
	{
		if (this.myRigidBody.velocity.magnitude >= 5f && base.HasDriver())
		{
			return false;
		}
		return base.MountEligable();
	}

	public override void OnKilled(HitInfo info)
	{
		if (this.dying)
		{
			return;
		}
		this.dying = true;
		this.repair.enabled = false;
		MotorRowboat motorRowboat = this;
		base.Invoke(new Action(motorRowboat.DismountAllPlayers), 10f);
		base.Invoke(new Action(this.ActualDeath), vehicle.boat_corpse_seconds);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		BaseEntity.RPCMessage rPCMessage;
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("MotorRowboat.OnRpcMessage", 0.1f))
		{
			if (rpc == 1873751172 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_EngineToggle "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("RPC_EngineToggle", 0.1f))
				{
					try
					{
						using (TimeWarning timeWarning2 = TimeWarning.New("Call", 0.1f))
						{
							rPCMessage = new BaseEntity.RPCMessage()
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_EngineToggle(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in RPC_EngineToggle");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc == 1851540757 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_OpenFuel "));
				}
				using (timeWarning1 = TimeWarning.New("RPC_OpenFuel", 0.1f))
				{
					try
					{
						using (timeWarning2 = TimeWarning.New("Call", 0.1f))
						{
							rPCMessage = new BaseEntity.RPCMessage()
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_OpenFuel(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in RPC_OpenFuel");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
			else if (rpc != 2115395408 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_WantsPush "));
				}
				using (timeWarning1 = TimeWarning.New("RPC_WantsPush", 0.1f))
				{
					try
					{
						using (timeWarning2 = TimeWarning.New("Call", 0.1f))
						{
							rPCMessage = new BaseEntity.RPCMessage()
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_WantsPush(rPCMessage);
						}
					}
					catch (Exception exception2)
					{
						player.Kick("RPC Error in RPC_WantsPush");
						Debug.LogException(exception2);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		base.PlayerServerInput(inputState, player);
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.Invoke(new Action(this.CheckInvalidBoat), 1f);
		if (base.health <= 0f)
		{
			base.Invoke(new Action(this.ActualDeath), vehicle.boat_corpse_seconds);
			this.buoyancy.buoyancyScale = 0f;
			this.dying = true;
		}
	}

	public bool RecentlyPushed()
	{
		return UnityEngine.Time.realtimeSinceStartup < this.nextPushTime;
	}

	[RPC_Server]
	public void RPC_EngineToggle(BaseEntity.RPCMessage msg)
	{
		BasePlayer basePlayer = msg.player;
		if (basePlayer == null)
		{
			return;
		}
		bool flag = msg.read.Bit();
		if (base.InDryDock())
		{
			flag = false;
		}
		if (!this.IsDriver(basePlayer))
		{
			return;
		}
		if (flag == this.EngineOn())
		{
			return;
		}
		this.EngineToggle(flag);
	}

	[RPC_Server]
	public void RPC_OpenFuel(BaseEntity.RPCMessage msg)
	{
		BasePlayer basePlayer = msg.player;
		if (basePlayer == null)
		{
			return;
		}
		if (!this.IsDriver(basePlayer))
		{
			return;
		}
		if (this.fuelStorageInstance.IsValid(base.isServer))
		{
			this.fuelStorageInstance.Get(base.isServer).GetComponent<StorageContainer>().PlayerOpenLoot(basePlayer);
		}
	}

	[RPC_Server]
	public void RPC_WantsPush(BaseEntity.RPCMessage msg)
	{
		BasePlayer basePlayer = msg.player;
		if (basePlayer.isMounted)
		{
			return;
		}
		if (this.RecentlyPushed())
		{
			return;
		}
		if (!base.HasFlag(BaseEntity.Flags.Reserved7) || basePlayer.WaterFactor() > 0.6f && !base.IsFlipped())
		{
			return;
		}
		if (Vector3.Distance(basePlayer.transform.position, base.transform.position) > 5f)
		{
			return;
		}
		if (this.dying)
		{
			return;
		}
		if (Interface.CallHook("CanPushBoat", basePlayer, this) != null)
		{
			return;
		}
		basePlayer.metabolism.calories.Subtract(2f);
		basePlayer.metabolism.SendChangesToClient();
		if (!base.IsFlipped())
		{
			Vector3 vector3 = Vector3Ex.Direction2D(basePlayer.transform.position, base.transform.position);
			Vector3 vector31 = Vector3Ex.Direction2D(basePlayer.transform.position + (basePlayer.eyes.BodyForward() * 3f), basePlayer.transform.position);
			vector31 = ((Vector3.up * 0.1f) + vector31).normalized;
			Vector3 vector32 = base.transform.position + (vector3 * 2f);
			float single = 3f;
			float single1 = Vector3.Dot(base.transform.forward, vector31);
			single = single + Mathf.InverseLerp(0.8f, 1f, single1) * 3f;
			this.myRigidBody.AddForceAtPosition(vector31 * single, vector32, ForceMode.VelocityChange);
		}
		else
		{
			this.myRigidBody.AddRelativeTorque(Vector3.forward * 5f, ForceMode.VelocityChange);
		}
		this.nextPushTime = UnityEngine.Time.realtimeSinceStartup + 1f;
		if (base.HasFlag(BaseEntity.Flags.Reserved5))
		{
			if (this.pushWaterEffect.isValid)
			{
				Effect.server.Run(this.pushWaterEffect.resourcePath, this, 0, Vector3.zero, Vector3.zero, null, false);
				return;
			}
		}
		else if (this.pushLandEffect.isValid)
		{
			Effect.server.Run(this.pushLandEffect.resourcePath, this, 0, Vector3.zero, Vector3.zero, null, false);
		}
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.motorBoat = Facepunch.Pool.Get<Motorboat>();
		info.msg.motorBoat.storageid = this.storageUnitInstance.uid;
		info.msg.motorBoat.fuelStorageID = this.fuelStorageInstance.uid;
	}

	public override void SeatClippedWorld(BaseMountable mountable)
	{
		BasePlayer mounted = mountable.GetMounted();
		if (mounted == null)
		{
			return;
		}
		if (this.IsDriver(mounted))
		{
			this.steering = 0f;
			this.gasPedal = 0f;
		}
		Vector3 vector3 = this.myRigidBody.velocity;
		float single = Mathf.InverseLerp(4f, 20f, vector3.magnitude);
		if (single > 0f)
		{
			mounted.Hurt(single * 100f, DamageType.Blunt, this, false);
		}
		if (mounted != null && mounted.isMounted)
		{
			base.SeatClippedWorld(mountable);
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRandomized(new Action(this.BoatDecay), UnityEngine.Random.Range(30f, 60f), 60f, 6f);
	}

	public override void SpawnSubEntities()
	{
		base.SpawnSubEntities();
		if (!Rust.Application.isLoadingSave)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.storageUnitPrefab.resourcePath, this.storageUnitPoint.localPosition, this.storageUnitPoint.localRotation, true);
			baseEntity.Spawn();
			baseEntity.SetParent(this, false, false);
			this.storageUnitInstance.Set(baseEntity);
			UnityEngine.Physics.IgnoreCollision(baseEntity.GetComponent<Collider>(), this.mainCollider, true);
			BaseEntity baseEntity1 = GameManager.server.CreateEntity(this.fuelStoragePrefab.resourcePath, this.fuelStoragePoint.localPosition, this.fuelStoragePoint.localRotation, true);
			baseEntity1.Spawn();
			baseEntity1.SetParent(this, false, false);
			this.fuelStorageInstance.Set(baseEntity1);
			UnityEngine.Physics.IgnoreCollision(baseEntity1.GetComponent<Collider>(), this.mainCollider, true);
		}
	}

	public float TimeSinceDriver()
	{
		return UnityEngine.Time.time - this.lastHadDriverTime;
	}

	public void UpdateDrag()
	{
		float single = this.myRigidBody.velocity.SqrMagnitude2D();
		float single1 = Mathf.InverseLerp(0f, 2f, single);
		this.myRigidBody.angularDrag = this.angularDragBase + this.angularDragVelocity * single1;
		this.myRigidBody.drag = this.landDrag + this.waterDrag * Mathf.InverseLerp(0f, 1f, this.buoyancy.submergedFraction);
		if (this.offAxisDrag > 0f)
		{
			Vector3 vector3 = base.transform.forward;
			Vector3 vector31 = this.myRigidBody.velocity;
			float single2 = Vector3.Dot(vector3, vector31.normalized);
			float single3 = Mathf.InverseLerp(0.98f, 0.92f, single2);
			Rigidbody rigidbody = this.myRigidBody;
			rigidbody.drag = rigidbody.drag + single3 * this.offAxisDrag * this.buoyancy.submergedFraction;
		}
	}

	public virtual bool UseFuel(float seconds)
	{
		if (!this.fuelStorageInstance.IsValid(base.isServer))
		{
			return false;
		}
		StorageContainer component = this.fuelStorageInstance.Get(base.isServer).GetComponent<StorageContainer>();
		if (component == null)
		{
			return false;
		}
		Item slot = component.inventory.GetSlot(0);
		if (slot == null || slot.amount < 1)
		{
			return false;
		}
		this.pendingFuel = this.pendingFuel + seconds * this.fuelPerSec;
		if (this.pendingFuel >= 1f)
		{
			int num = Mathf.FloorToInt(this.pendingFuel);
			slot.UseItem(num);
			this.pendingFuel -= (float)num;
		}
		return true;
	}

	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		float single = this.TimeSinceDriver();
		bool flag = (!this.EngineOn() || base.IsFlipped() || base.healthFraction <= 0f || !this.HasFuel(false) ? false : single < 75f);
		if (single > 15f)
		{
			this.steering += Mathf.InverseLerp(15f, 30f, single);
			this.steering = Mathf.Clamp(-1f, 1f, this.steering);
			if (single > 75f)
			{
				this.gasPedal = 0f;
			}
		}
		base.SetFlag(BaseEntity.Flags.Reserved3, this.steering > 0f, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved4, this.steering < 0f, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved1, flag, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved2, (!this.EngineOn() ? false : this.gasPedal != 0f), false, false);
		base.SetFlag(BaseEntity.Flags.Reserved5, this.buoyancy.submergedFraction > 0.85f, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved6, this.HasFuel(false), false, false);
		Vector3 vector3 = this.myRigidBody.velocity;
		base.SetFlag(BaseEntity.Flags.Reserved7, vector3.magnitude < 1f, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved8, this.RecentlyPushed(), false, false);
		base.SendNetworkUpdate_Flags();
		this.UpdateDrag();
		if (!this.dying)
		{
			float single1 = 1f;
			float single2 = this.myRigidBody.velocity.Magnitude2D();
			float single3 = Mathf.InverseLerp(1f, 10f, single2) * 0.5f * base.healthFraction;
			if (!this.EngineOn())
			{
				single3 = 0f;
			}
			float single4 = 1f - 0.3f * (1f - base.healthFraction);
			this.buoyancy.buoyancyScale = (single1 + single3) * single4;
		}
		else
		{
			this.buoyancy.buoyancyScale = Mathf.Lerp(this.buoyancy.buoyancyScale, 0f, UnityEngine.Time.fixedDeltaTime * 0.1f);
		}
		if (this.EngineOn())
		{
			this.UseFuel(UnityEngine.Time.fixedDeltaTime * (base.HasFlag(BaseEntity.Flags.Reserved2) ? 1f : 0.0333f));
			this.lastUsedFuelTime = UnityEngine.Time.time;
		}
	}
}