using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class HotAirBalloon : BaseCombatEntity
{
	protected const BaseEntity.Flags Flag_HasFuel = BaseEntity.Flags.Reserved6;

	protected const BaseEntity.Flags Flag_HalfInflated = BaseEntity.Flags.Reserved1;

	protected const BaseEntity.Flags Flag_FullInflated = BaseEntity.Flags.Reserved2;

	public Transform centerOfMass;

	public Rigidbody myRigidbody;

	public Transform buoyancyPoint;

	public float liftAmount = 10f;

	public Transform windSock;

	public Transform[] windFlags;

	public GameObject staticBalloonDeflated;

	public GameObject staticBalloon;

	public GameObject animatedBalloon;

	public Animator balloonAnimator;

	public Transform groundSample;

	public float inflationLevel;

	[Header("Fuel")]
	public GameObjectRef fuelStoragePrefab;

	public Transform fuelStoragePoint;

	public EntityRef fuelStorageInstance;

	public float fuelPerSec = 0.25f;

	[Header("Storage")]
	public GameObjectRef storageUnitPrefab;

	public Transform storageUnitPoint;

	public EntityRef storageUnitInstance;

	public Transform engineHeight;

	public GameObject[] killTriggers;

	[ServerVar(Help="Population active on the server")]
	public static float population;

	[ServerVar(Help="How long before a HAB is killed while outside")]
	public static float outsidedecayminutes;

	[ServerVar]
	public static float serviceCeiling;

	private float currentBuoyancy;

	public float windForce = 30000f;

	public Vector3 currentWindVec = Vector3.zero;

	private float lastBlastTime;

	private float avgTerrainHeight;

	public Bounds collapsedBounds;

	public Bounds raisedBounds;

	public GameObject balloonCollider;

	protected bool grounded;

	private float nextFuelCheckTime;

	private bool cachedHasFuel;

	private float pendingFuel;

	static HotAirBalloon()
	{
		HotAirBalloon.population = 1f;
		HotAirBalloon.outsidedecayminutes = 180f;
		HotAirBalloon.serviceCeiling = 200f;
	}

	public HotAirBalloon()
	{
	}

	public void DecayTick()
	{
		if (base.healthFraction == 0f || this.inflationLevel == 1f)
		{
			return;
		}
		if (UnityEngine.Time.time < this.lastBlastTime + 600f)
		{
			return;
		}
		float single = 1f / HotAirBalloon.outsidedecayminutes;
		if (this.IsOutside())
		{
			this.Hurt(this.MaxHealth() * single, DamageType.Decay, this, false);
		}
	}

	[RPC_Server]
	public void EngineSwitch(BaseEntity.RPCMessage msg)
	{
		base.SetFlag(BaseEntity.Flags.On, msg.read.Bit(), false, true);
		if (!base.IsOn())
		{
			base.CancelInvoke(new Action(this.ScheduleOff));
			return;
		}
		base.Invoke(new Action(this.ScheduleOff), 60f);
	}

	protected void FixedUpdate()
	{
		RaycastHit raycastHit;
		if (base.isClient)
		{
			return;
		}
		if (!this.HasFuel(false) || this.WaterLogged())
		{
			base.SetFlag(BaseEntity.Flags.On, false, false, true);
		}
		if (base.IsOn())
		{
			this.UseFuel(UnityEngine.Time.fixedDeltaTime);
		}
		base.SetFlag(BaseEntity.Flags.Reserved6, this.HasFuel(false), false, true);
		GameObject[] gameObjectArray = this.killTriggers;
		for (int i = 0; i < (int)gameObjectArray.Length; i++)
		{
			gameObjectArray[i].SetActive((this.inflationLevel != 1f || this.myRigidbody.velocity.y >= 0f ? this.myRigidbody.velocity.y < 0.75f : true));
		}
		float single = this.inflationLevel;
		if (base.IsOn() && this.inflationLevel < 1f)
		{
			this.inflationLevel = Mathf.Clamp01(this.inflationLevel + UnityEngine.Time.fixedDeltaTime / 10f);
		}
		else if (this.grounded && this.inflationLevel > 0f && !base.IsOn() && (UnityEngine.Time.time > this.lastBlastTime + 30f || this.WaterLogged()))
		{
			this.inflationLevel = Mathf.Clamp01(this.inflationLevel - UnityEngine.Time.fixedDeltaTime / 10f);
		}
		if (single != this.inflationLevel)
		{
			if (this.inflationLevel == 1f)
			{
				this.bounds = this.raisedBounds;
			}
			else if (this.inflationLevel == 0f)
			{
				this.bounds = this.collapsedBounds;
			}
			base.SetFlag(BaseEntity.Flags.Reserved1, this.inflationLevel > 0.3f, false, true);
			base.SetFlag(BaseEntity.Flags.Reserved2, this.inflationLevel >= 1f, false, true);
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			float single1 = this.inflationLevel;
		}
		if (!base.IsOn())
		{
			this.currentBuoyancy = this.currentBuoyancy - UnityEngine.Time.fixedDeltaTime * 0.1f;
		}
		else if (this.inflationLevel >= 1f)
		{
			this.currentBuoyancy = this.currentBuoyancy + UnityEngine.Time.fixedDeltaTime * 0.2f;
			this.lastBlastTime = UnityEngine.Time.time;
		}
		this.currentBuoyancy = Mathf.Clamp(this.currentBuoyancy, 0f, 0.8f + 0.2f * base.healthFraction);
		if (this.inflationLevel > 0f)
		{
			this.avgTerrainHeight = Mathf.Lerp(this.avgTerrainHeight, TerrainMeta.HeightMap.GetHeight(base.transform.position), UnityEngine.Time.deltaTime);
			float single2 = 1f - Mathf.InverseLerp(this.avgTerrainHeight + HotAirBalloon.serviceCeiling - 20f, this.avgTerrainHeight + HotAirBalloon.serviceCeiling, this.buoyancyPoint.position.y);
			this.myRigidbody.AddForceAtPosition((((Vector3.up * -UnityEngine.Physics.gravity.y) * this.myRigidbody.mass) * 0.5f) * this.inflationLevel, this.buoyancyPoint.position, ForceMode.Force);
			this.myRigidbody.AddForceAtPosition(((Vector3.up * this.liftAmount) * this.currentBuoyancy) * single2, this.buoyancyPoint.position, ForceMode.Force);
			Vector3 windAtPos = this.GetWindAtPos(this.buoyancyPoint.position);
			float single3 = windAtPos.magnitude;
			float single4 = 1f;
			float single5 = Mathf.Max(TerrainMeta.HeightMap.GetHeight(this.buoyancyPoint.position), TerrainMeta.WaterMap.GetHeight(this.buoyancyPoint.position));
			float single6 = Mathf.InverseLerp(single5 + 20f, single5 + 60f, this.buoyancyPoint.position.y);
			float single7 = 1f;
			if (UnityEngine.Physics.SphereCast(new Ray(base.transform.position + (Vector3.up * 2f), Vector3.down), 1.5f, out raycastHit, 5f, 1218511105))
			{
				single7 = Mathf.Clamp01(raycastHit.distance / 5f);
			}
			single4 = single4 * (single6 * single2 * single7);
			single4 = single4 * (0.2f + 0.8f * base.healthFraction);
			Vector3 vector3 = (windAtPos.normalized * single4) * this.windForce;
			this.currentWindVec = Vector3.Lerp(this.currentWindVec, vector3, UnityEngine.Time.fixedDeltaTime * 0.25f);
			this.myRigidbody.AddForceAtPosition(vector3 * 0.1f, this.buoyancyPoint.position, ForceMode.Force);
			this.myRigidbody.AddForce(vector3 * 0.9f, ForceMode.Force);
		}
	}

	public override Quaternion GetAngularVelocityServer()
	{
		return Quaternion.LookRotation(this.myRigidbody.angularVelocity, base.transform.up);
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

	public override Vector3 GetLocalVelocityServer()
	{
		return this.myRigidbody.velocity;
	}

	public Vector3 GetWindAtPos(Vector3 pos)
	{
		float single = pos.y * 6f;
		Vector3 vector3 = new Vector3(Mathf.Sin(single * 0.0174532924f), 0f, Mathf.Cos(single * 0.0174532924f));
		return vector3.normalized * 1f;
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

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.hotAirBalloon != null)
		{
			this.inflationLevel = info.msg.hotAirBalloon.inflationAmount;
			if (info.fromDisk && this.myRigidbody)
			{
				this.myRigidbody.velocity = info.msg.hotAirBalloon.velocity;
			}
		}
		if (info.msg.motorBoat != null)
		{
			this.fuelStorageInstance.uid = info.msg.motorBoat.fuelStorageID;
			this.storageUnitInstance.uid = info.msg.motorBoat.storageid;
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		BaseEntity.RPCMessage rPCMessage;
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("HotAirBalloon.OnRpcMessage", 0.1f))
		{
			if (rpc == 578721460 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - EngineSwitch "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("EngineSwitch", 0.1f))
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
							this.EngineSwitch(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in EngineSwitch");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc != 1851540757 || !(player != null))
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
		}
		return flag;
	}

	public override bool PhysicsDriven()
	{
		return true;
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
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
		info.msg.hotAirBalloon = Facepunch.Pool.Get<ProtoBuf.HotAirBalloon>();
		info.msg.hotAirBalloon.inflationAmount = this.inflationLevel;
		if (info.forDisk && this.myRigidbody)
		{
			info.msg.hotAirBalloon.velocity = this.myRigidbody.velocity;
		}
		info.msg.motorBoat = Facepunch.Pool.Get<Motorboat>();
		info.msg.motorBoat.storageid = this.storageUnitInstance.uid;
		info.msg.motorBoat.fuelStorageID = this.fuelStorageInstance.uid;
	}

	public void ScheduleOff()
	{
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
	}

	public override void ServerInit()
	{
		this.myRigidbody.centerOfMass = this.centerOfMass.localPosition;
		this.myRigidbody.isKinematic = false;
		this.avgTerrainHeight = TerrainMeta.HeightMap.GetHeight(base.transform.position);
		base.ServerInit();
		this.bounds = this.collapsedBounds;
		base.InvokeRandomized(new Action(this.DecayTick), UnityEngine.Random.Range(30f, 60f), 60f, 6f);
		base.InvokeRandomized(new Action(this.UpdateIsGrounded), 0f, 3f, 0.2f);
	}

	public override void Spawn()
	{
		base.Spawn();
		this.SpawnSubEntities();
	}

	public void SpawnSubEntities()
	{
		if (!Rust.Application.isLoadingSave)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.storageUnitPrefab.resourcePath, this.storageUnitPoint.localPosition, this.storageUnitPoint.localRotation, true);
			baseEntity.Spawn();
			baseEntity.SetParent(this, false, false);
			this.storageUnitInstance.Set(baseEntity);
			BaseEntity baseEntity1 = GameManager.server.CreateEntity(this.fuelStoragePrefab.resourcePath, this.fuelStoragePoint.localPosition, this.fuelStoragePoint.localRotation, true);
			baseEntity1.Spawn();
			baseEntity1.SetParent(this, false, false);
			this.fuelStorageInstance.Set(baseEntity1);
		}
	}

	public override bool SupportsChildDeployables()
	{
		return false;
	}

	public void UpdateIsGrounded()
	{
		if (this.lastBlastTime + 5f > UnityEngine.Time.time)
		{
			return;
		}
		List<Collider> list = Facepunch.Pool.GetList<Collider>();
		GamePhysics.OverlapSphere(this.groundSample.transform.position, 1.25f, list, 1218511105, QueryTriggerInteraction.Ignore);
		this.grounded = list.Count > 0;
		Facepunch.Pool.FreeList<Collider>(ref list);
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

	public bool WaterLogged()
	{
		return WaterLevel.Test(this.engineHeight.position);
	}
}