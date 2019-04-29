using Facepunch;
using Network;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CargoShip : BaseEntity
{
	public int targetNodeIndex = -1;

	public GameObject wakeParent;

	public GameObjectRef scientistTurretPrefab;

	public Transform[] scientistSpawnPoints;

	public List<Transform> crateSpawns;

	public GameObjectRef lockedCratePrefab;

	public GameObjectRef militaryCratePrefab;

	public GameObjectRef eliteCratePrefab;

	public GameObjectRef junkCratePrefab;

	public Transform waterLine;

	public Transform rudder;

	public Transform propeller;

	public GameObjectRef escapeBoatPrefab;

	public Transform escapeBoatPoint;

	public GameObject radiation;

	public GameObjectRef mapMarkerEntityPrefab;

	public GameObject hornOrigin;

	public SoundDefinition hornDef;

	public CargoShipSounds cargoShipSounds;

	public GameObject[] layouts;

	[ServerVar]
	public static bool event_enabled;

	[ServerVar]
	public static float event_duration_minutes;

	[ServerVar]
	public static float egress_duration_minutes;

	[ServerVar]
	public static int loot_rounds;

	[ServerVar]
	public static float loot_round_spacing_minutes;

	private BaseEntity mapMarkerInstance;

	private Vector3 currentVelocity = Vector3.zero;

	private float currentThrottle;

	private float currentTurnSpeed;

	private float turnScale;

	public GameObjectRef playerTest;

	private int lootRoundsPassed;

	private int hornCount;

	private float currentRadiation;

	private bool egressing;

	static CargoShip()
	{
		CargoShip.event_enabled = true;
		CargoShip.event_duration_minutes = 50f;
		CargoShip.egress_duration_minutes = 10f;
		CargoShip.loot_rounds = 3;
		CargoShip.loot_round_spacing_minutes = 10f;
	}

	public CargoShip()
	{
	}

	public void BuildingCheck()
	{
		List<DecayEntity> list = Pool.GetList<DecayEntity>();
		Vis.Entities<DecayEntity>(base.WorldSpaceBounds(), list, 2097152, QueryTriggerInteraction.Collide);
		foreach (DecayEntity decayEntity in list)
		{
			if (!decayEntity.isServer || !decayEntity.IsAlive())
			{
				continue;
			}
			decayEntity.Kill(BaseNetworkable.DestroyMode.Gib);
		}
		Pool.FreeList<DecayEntity>(ref list);
	}

	public void CreateMapMarker()
	{
		if (this.mapMarkerInstance)
		{
			this.mapMarkerInstance.Kill(BaseNetworkable.DestroyMode.None);
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.mapMarkerEntityPrefab.resourcePath, Vector3.zero, Quaternion.identity, true);
		baseEntity.Spawn();
		baseEntity.SetParent(this, false, false);
		this.mapMarkerInstance = baseEntity;
	}

	public void DelayedDestroy()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	public void DisableCollisionTest()
	{
	}

	public void FindInitialNode()
	{
		this.targetNodeIndex = this.GetClosestNodeToUs();
	}

	public void FixedUpdate()
	{
		if (base.isClient)
		{
			return;
		}
		this.UpdateMovement();
	}

	public override Quaternion GetAngularVelocityServer()
	{
		return Quaternion.Euler(0f, this.currentTurnSpeed, 0f);
	}

	public int GetClosestNodeToUs()
	{
		int num = 0;
		float single = Single.PositiveInfinity;
		for (int i = 0; i < TerrainMeta.Path.OceanPatrolFar.Count; i++)
		{
			Vector3 item = TerrainMeta.Path.OceanPatrolFar[i];
			float single1 = Vector3.Distance(base.transform.position, item);
			if (single1 < single)
			{
				num = i;
				single = single1;
			}
		}
		return num;
	}

	public override Vector3 GetLocalVelocityServer()
	{
		return this.currentVelocity;
	}

	public override float InheritedVelocityScale()
	{
		return 1f;
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning timeWarning = TimeWarning.New("CargoShip.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override bool PhysicsDriven()
	{
		return true;
	}

	public void PickLayout()
	{
		if (!base.HasFlag(BaseEntity.Flags.Reserved1) && !base.HasFlag(BaseEntity.Flags.Reserved2))
		{
			int num = UnityEngine.Random.Range(0, (int)this.layouts.Length);
			if (num == 0)
			{
				base.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
				return;
			}
			if (num == 1)
			{
				base.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
			}
		}
	}

	public void PlayHorn()
	{
		base.ClientRPC(null, "DoHornSound");
		this.hornCount++;
		if (this.hornCount >= 3)
		{
			this.hornCount = 0;
			base.CancelInvoke(new Action(this.PlayHorn));
		}
	}

	public void RespawnLoot()
	{
		base.InvokeRepeating(new Action(this.PlayHorn), 0f, 8f);
		this.SpawnCrate(this.lockedCratePrefab.resourcePath);
		this.SpawnCrate(this.eliteCratePrefab.resourcePath);
		for (int i = 0; i < 4; i++)
		{
			this.SpawnCrate(this.militaryCratePrefab.resourcePath);
		}
		for (int j = 0; j < 4; j++)
		{
			this.SpawnCrate(this.junkCratePrefab.resourcePath);
		}
		this.lootRoundsPassed++;
		if (this.lootRoundsPassed >= CargoShip.loot_rounds)
		{
			base.CancelInvoke(new Action(this.RespawnLoot));
		}
	}

	public override void ServerInit()
	{
		GameObject[] gameObjectArray = this.layouts;
		for (int i = 0; i < (int)gameObjectArray.Length; i++)
		{
			gameObjectArray[i].SetActive(false);
		}
		this.UpdateLayoutFromFlags();
		base.ServerInit();
		base.Invoke(new Action(this.FindInitialNode), 2f);
		base.InvokeRepeating(new Action(this.BuildingCheck), 1f, 5f);
		base.InvokeRepeating(new Action(this.RespawnLoot), 10f, 60f * CargoShip.loot_round_spacing_minutes);
		base.Invoke(new Action(this.DisableCollisionTest), 10f);
		float height = TerrainMeta.WaterMap.GetHeight(base.transform.position);
		Vector3 vector3 = base.transform.InverseTransformPoint(this.waterLine.transform.position);
		base.transform.position = new Vector3(base.transform.position.x, height - vector3.y, base.transform.position.z);
		this.SpawnSubEntities();
		base.Invoke(new Action(this.StartEgress), 60f * CargoShip.event_duration_minutes);
		this.CreateMapMarker();
	}

	public override void Spawn()
	{
		if (!Rust.Application.isLoadingSave)
		{
			this.PickLayout();
		}
		base.Spawn();
	}

	public void SpawnCrate(string resourcePath)
	{
		int num = UnityEngine.Random.Range(0, this.crateSpawns.Count);
		Vector3 item = this.crateSpawns[num].position;
		Quaternion quaternion = this.crateSpawns[num].rotation;
		this.crateSpawns.Remove(this.crateSpawns[num]);
		BaseEntity baseEntity = GameManager.server.CreateEntity(resourcePath, item, quaternion, true);
		if (baseEntity)
		{
			baseEntity.enableSaving = false;
			baseEntity.SendMessage("SetWasDropped", SendMessageOptions.DontRequireReceiver);
			baseEntity.Spawn();
			baseEntity.SetParent(this, true, false);
			Rigidbody component = baseEntity.GetComponent<Rigidbody>();
			if (component != null)
			{
				component.isKinematic = true;
			}
		}
	}

	public void SpawnSubEntities()
	{
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.escapeBoatPrefab.resourcePath, this.escapeBoatPoint.position, this.escapeBoatPoint.rotation, true);
		if (baseEntity)
		{
			baseEntity.enableSaving = false;
			baseEntity.SetParent(this, true, false);
			baseEntity.Spawn();
			baseEntity.GetComponent<Rigidbody>().isKinematic = true;
			RHIB component = baseEntity.GetComponent<RHIB>();
			if (component)
			{
				BaseEntity baseEntity1 = component.fuelStorageInstance.Get(true);
				if (baseEntity1)
				{
					baseEntity1.GetComponent<StorageContainer>().inventory.AddItem(ItemManager.FindItemDefinition("lowgradefuel"), 50);
				}
			}
		}
	}

	public void StartEgress()
	{
		if (this.egressing)
		{
			return;
		}
		this.egressing = true;
		base.CancelInvoke(new Action(this.PlayHorn));
		this.radiation.SetActive(true);
		base.SetFlag(BaseEntity.Flags.Reserved8, true, false, true);
		base.InvokeRepeating(new Action(this.UpdateRadiation), 10f, 1f);
		base.Invoke(new Action(this.DelayedDestroy), 60f * CargoShip.egress_duration_minutes);
	}

	public void TriggeredEventSpawn()
	{
		float size = TerrainMeta.Size.x;
		Vector3 height = Vector3Ex.Range(-1f, 1f);
		height.y = 0f;
		height.Normalize();
		height = height * (size * 1f);
		height.y = TerrainMeta.WaterMap.GetHeight(height);
		base.transform.position = height;
		if (!CargoShip.event_enabled || CargoShip.event_duration_minutes == 0f)
		{
			base.Invoke(new Action(this.DelayedDestroy), 1f);
		}
	}

	public void UpdateLayoutFromFlags()
	{
		if (base.HasFlag(BaseEntity.Flags.Reserved1))
		{
			this.layouts[0].SetActive(true);
			return;
		}
		if (base.HasFlag(BaseEntity.Flags.Reserved2))
		{
			this.layouts[1].SetActive(true);
		}
	}

	public void UpdateMovement()
	{
		Vector3 vector3;
		if (TerrainMeta.Path.OceanPatrolFar == null || TerrainMeta.Path.OceanPatrolFar.Count == 0)
		{
			return;
		}
		if (this.targetNodeIndex == -1)
		{
			return;
		}
		Vector3 item = TerrainMeta.Path.OceanPatrolFar[this.targetNodeIndex];
		if (this.egressing)
		{
			Vector3 vector31 = base.transform.position;
			vector3 = base.transform.position - Vector3.zero;
			item = vector31 + (vector3.normalized * 10000f);
		}
		float single = 0f;
		vector3 = item - base.transform.position;
		Vector3 vector32 = vector3.normalized;
		float single1 = Vector3.Dot(base.transform.forward, vector32);
		single = Mathf.InverseLerp(0f, 1f, single1);
		float single2 = Vector3.Dot(base.transform.right, vector32);
		float single3 = 2.5f;
		float single4 = Mathf.InverseLerp(0.05f, 0.5f, Mathf.Abs(single2));
		this.turnScale = Mathf.Lerp(this.turnScale, single4, Time.deltaTime * 0.2f);
		float single5 = (float)((single2 < 0f ? -1 : 1));
		this.currentTurnSpeed = single3 * this.turnScale * single5;
		base.transform.Rotate(Vector3.up, Time.deltaTime * this.currentTurnSpeed, Space.World);
		this.currentThrottle = Mathf.Lerp(this.currentThrottle, single, Time.deltaTime * 0.2f);
		this.currentVelocity = base.transform.forward * (8f * this.currentThrottle);
		Transform transforms = base.transform;
		transforms.position = transforms.position + (this.currentVelocity * Time.deltaTime);
		if (Vector3.Distance(base.transform.position, item) < 80f)
		{
			this.targetNodeIndex++;
			if (this.targetNodeIndex >= TerrainMeta.Path.OceanPatrolFar.Count)
			{
				this.targetNodeIndex = 0;
			}
		}
	}

	public void UpdateRadiation()
	{
		this.currentRadiation += 1f;
		TriggerRadiation[] componentsInChildren = this.radiation.GetComponentsInChildren<TriggerRadiation>();
		for (int i = 0; i < (int)componentsInChildren.Length; i++)
		{
			componentsInChildren[i].RadiationAmountOverride = this.currentRadiation;
		}
	}
}