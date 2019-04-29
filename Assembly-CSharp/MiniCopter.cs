using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class MiniCopter : BaseHelicopterVehicle
{
	public Transform waterSample;

	public WheelCollider leftWheel;

	public WheelCollider rightWheel;

	public WheelCollider frontWheel;

	public Transform leftWheelTrans;

	public Transform rightWheelTrans;

	public Transform frontWheelTrans;

	public float cachedrotation_left;

	public float cachedrotation_right;

	public float cachedrotation_front;

	public AnimationCurve bladeEngineCurve;

	public const BaseEntity.Flags Flag_EngineStart = BaseEntity.Flags.Reserved4;

	public Transform mainRotorBlur;

	public Transform mainRotorBlades;

	public Transform rearRotorBlades;

	public Transform rearRotorBlur;

	public float motorForceConstant = 150f;

	public float brakeForceConstant = 500f;

	public GameObject preventBuildingObject;

	[ServerVar(Help="Population active on the server")]
	public static float population;

	[ServerVar(Help="How long before a minicopter is killed while outside")]
	public static float outsidedecayminutes;

	private float lastEngineTime;

	public float rotorBlurThreshold = 15f;

	[Header("Fuel")]
	public GameObjectRef fuelStoragePrefab;

	public Transform fuelStoragePoint;

	public EntityRef fuelStorageInstance;

	public float fuelPerSec = 0.25f;

	private float nextFuelCheckTime;

	private bool cachedHasFuel;

	private float pendingFuel;

	static MiniCopter()
	{
		MiniCopter.population = 1f;
		MiniCopter.outsidedecayminutes = 240f;
	}

	public MiniCopter()
	{
	}

	private void ApplyForceAtWheels()
	{
		if (this.rigidBody == null)
		{
			return;
		}
		float single = 50f;
		float single1 = 0f;
		float single2 = 0f;
		if (!this.currentInputState.groundControl)
		{
			single = 20f;
			single2 = 0f;
			single1 = 0f;
		}
		else
		{
			single = (this.currentInputState.throttle == 0f ? 50f : 0f);
			single1 = this.currentInputState.throttle;
			single2 = this.currentInputState.yaw;
		}
		single1 = single1 * (base.IsOn() ? 1f : 0f);
		this.ApplyWheelForce(this.frontWheel, single1, single, single2);
		this.ApplyWheelForce(this.leftWheel, single1, single, 0f);
		this.ApplyWheelForce(this.rightWheel, single1, single, 0f);
	}

	public void ApplyWheelForce(WheelCollider wheel, float gasScale, float brakeScale, float turning)
	{
		if (wheel.isGrounded)
		{
			wheel.motorTorque = gasScale * this.motorForceConstant;
			wheel.brakeTorque = brakeScale * this.brakeForceConstant;
			wheel.steerAngle = 45f * turning;
		}
	}

	public void DecayTick()
	{
		if (base.healthFraction == 0f)
		{
			return;
		}
		if (UnityEngine.Time.time < this.lastEngineTime + 600f)
		{
			return;
		}
		float single = 1f / HotAirBalloon.outsidedecayminutes;
		this.Hurt(this.MaxHealth() * single * (this.IsOutside() ? 1f : 0.5f), DamageType.Decay, this, false);
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

	public void EngineOff()
	{
		if (!base.IsOn() && !this.IsStartingUp())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved4, false, false, true);
		this.lastEngineTime = UnityEngine.Time.time;
	}

	public void EngineOn()
	{
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved4, false, false, true);
		this.lastEngineTime = UnityEngine.Time.time;
	}

	public void EngineStartup()
	{
		if (this.Waterlogged())
		{
			return;
		}
		base.Invoke(new Action(this.EngineOn), 5f);
		base.SetFlag(BaseEntity.Flags.Reserved4, true, false, true);
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

	public override float GetServiceCeiling()
	{
		return HotAirBalloon.serviceCeiling;
	}

	public bool Grounded()
	{
		if (!this.leftWheel.isGrounded)
		{
			return false;
		}
		return this.rightWheel.isGrounded;
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

	public bool IsStartingUp()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved4);
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.motorBoat != null)
		{
			this.fuelStorageInstance.uid = info.msg.motorBoat.fuelStorageID;
		}
	}

	public override void MovementUpdate()
	{
		if (this.Grounded())
		{
			this.ApplyForceAtWheels();
		}
		if (base.IsOn() && (!this.currentInputState.groundControl || !this.Grounded()))
		{
			base.MovementUpdate();
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("MiniCopter.OnRpcMessage", 0.1f))
		{
			if (rpc != 1851540757 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_OpenFuel "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("RPC_OpenFuel", 0.1f))
				{
					try
					{
						using (TimeWarning timeWarning2 = TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage rPCMessage = new BaseEntity.RPCMessage()
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_OpenFuel(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in RPC_OpenFuel");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public override bool PhysicsDriven()
	{
		return true;
	}

	public override void PilotInput(InputState inputState, BasePlayer player)
	{
		base.PilotInput(inputState, player);
		if (!base.IsOn() && !this.IsStartingUp() && base.HasDriver() && inputState.IsDown(BUTTON.FORWARD) && this.HasFuel(false))
		{
			this.EngineStartup();
		}
		this.currentInputState.groundControl = inputState.IsDown(BUTTON.DUCK);
		if (this.currentInputState.groundControl)
		{
			this.currentInputState.roll = 0f;
			this.currentInputState.throttle = (inputState.IsDown(BUTTON.FORWARD) ? 1f : 0f);
			BaseHelicopterVehicle.HelicopterInputState helicopterInputState = this.currentInputState;
			helicopterInputState.throttle = helicopterInputState.throttle - (inputState.IsDown(BUTTON.BACKWARD) ? 1f : 0f);
		}
	}

	[RPC_Server]
	public void RPC_OpenFuel(BaseEntity.RPCMessage msg)
	{
		BasePlayer basePlayer = msg.player;
		if (basePlayer == null)
		{
			return;
		}
		if (this.fuelStorageInstance.IsValid(base.isServer))
		{
			this.fuelStorageInstance.Get(base.isServer).GetComponent<StorageContainer>().PlayerOpenLoot(basePlayer);
		}
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.motorBoat = Facepunch.Pool.Get<Motorboat>();
		info.msg.motorBoat.fuelStorageID = this.fuelStorageInstance.uid;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		this.rigidBody.inertiaTensor = this.rigidBody.inertiaTensor;
		this.preventBuildingObject.SetActive(true);
		base.InvokeRepeating(new Action(this.UpdateNetwork), 0f, 0.25f);
		base.InvokeRandomized(new Action(this.DecayTick), UnityEngine.Random.Range(30f, 60f), 60f, 6f);
	}

	public override void SetDefaultInputState()
	{
		this.currentInputState.Reset();
		if (this.Grounded())
		{
			return;
		}
		if (!this.IsMounted())
		{
			this.currentInputState.throttle = -1f;
		}
		else
		{
			float single = Vector3.Dot(Vector3.up, base.transform.right);
			float single1 = Vector3.Dot(Vector3.up, base.transform.forward);
			this.currentInputState.roll = (single < 0f ? 1f : 0f);
			BaseHelicopterVehicle.HelicopterInputState helicopterInputState = this.currentInputState;
			helicopterInputState.roll = helicopterInputState.roll - (single > 0f ? 1f : 0f);
			if (single1 < 0f)
			{
				this.currentInputState.pitch = -1f;
				return;
			}
			if (single1 > 0f)
			{
				this.currentInputState.pitch = 1f;
				return;
			}
		}
	}

	public void SpawnFuelObject()
	{
		if (!Rust.Application.isLoadingSave)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.fuelStoragePrefab.resourcePath, this.fuelStoragePoint.localPosition, this.fuelStoragePoint.localRotation, true);
			baseEntity.Spawn();
			baseEntity.SetParent(this, false, false);
			this.fuelStorageInstance.Set(baseEntity);
		}
	}

	public override void SpawnSubEntities()
	{
		base.SpawnSubEntities();
		this.SpawnFuelObject();
	}

	public void UpdateCOM()
	{
		this.rigidBody.centerOfMass = this.com.localPosition;
	}

	public void UpdateNetwork()
	{
		base.SetFlag(BaseEntity.Flags.Reserved1, this.leftWheel.isGrounded, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved2, this.rightWheel.isGrounded, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved3, this.frontWheel.isGrounded, false, false);
		base.SendNetworkUpdate_Flags();
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
		if (base.IsOn())
		{
			this.UseFuel(UnityEngine.Time.fixedDeltaTime);
			if (UnityEngine.Time.time > this.lastPlayerInputTime + 1f && !base.HasDriver() || !this.HasFuel(false) || this.Waterlogged())
			{
				this.EngineOff();
			}
		}
		int num = (int)((base.HasDriver() ? false : this.currentInputState.throttle <= 0f));
		WheelFrictionCurve wheelFrictionCurve = this.leftWheel.forwardFriction;
	}

	public bool Waterlogged()
	{
		return WaterLevel.Test(this.waterSample.transform.position);
	}
}