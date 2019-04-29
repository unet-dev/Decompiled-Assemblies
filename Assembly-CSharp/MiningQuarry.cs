using Facepunch;
using Oxide.Core;
using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MiningQuarry : BaseResourceExtractor
{
	public Animator beltAnimator;

	public Renderer beltScrollRenderer;

	public int scrollMatIndex = 3;

	public SoundPlayer[] onSounds;

	public float processRate = 5f;

	public float workToAdd = 15f;

	public GameObjectRef bucketDropEffect;

	public GameObject bucketDropTransform;

	public MiningQuarry.ChildPrefab engineSwitchPrefab;

	public MiningQuarry.ChildPrefab hopperPrefab;

	public MiningQuarry.ChildPrefab fuelStoragePrefab;

	public bool isStatic;

	public ResourceDepositManager.ResourceDeposit _linkedDeposit;

	public MiningQuarry.QuarryType staticType;

	public MiningQuarry()
	{
	}

	public void EngineSwitch(bool isOn)
	{
		if (isOn && this.FuelCheck())
		{
			this.SetOn(true);
			return;
		}
		this.SetOn(false);
	}

	public bool FuelCheck()
	{
		Item item = this.fuelStoragePrefab.instance.GetComponent<StorageContainer>().inventory.FindItemsByItemName("lowgradefuel");
		if (item == null || item.amount < 1)
		{
			return false;
		}
		item.UseItem(1);
		return true;
	}

	public bool IsEngineOn()
	{
		return base.HasFlag(BaseEntity.Flags.On);
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk && info.msg.miningQuarry != null)
		{
			if (this.fuelStoragePrefab.instance == null || this.hopperPrefab.instance == null)
			{
				Debug.Log("Cannot load mining quary because children were null");
				return;
			}
			this.fuelStoragePrefab.instance.GetComponent<StorageContainer>().inventory.Load(info.msg.miningQuarry.extractor.fuelContents);
			this.hopperPrefab.instance.GetComponent<StorageContainer>().inventory.Load(info.msg.miningQuarry.extractor.outputContents);
			this.staticType = (MiningQuarry.QuarryType)info.msg.miningQuarry.staticType;
		}
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.EngineSwitch(base.HasFlag(BaseEntity.Flags.On));
		this.UpdateStaticDeposit();
	}

	public void ProcessResources()
	{
		if (this._linkedDeposit == null)
		{
			return;
		}
		if (this.hopperPrefab.instance == null)
		{
			return;
		}
		foreach (ResourceDepositManager.ResourceDeposit.ResourceDepositEntry _resource in this._linkedDeposit._resources)
		{
			if (!this.canExtractLiquid && _resource.isLiquid || !this.canExtractSolid && !_resource.isLiquid)
			{
				continue;
			}
			_resource.workDone += this.workToAdd;
			if (_resource.workDone < _resource.workNeeded)
			{
				continue;
			}
			int num = Mathf.FloorToInt(_resource.workDone / _resource.workNeeded);
			ResourceDepositManager.ResourceDeposit.ResourceDepositEntry resourceDepositEntry = _resource;
			resourceDepositEntry.workDone = resourceDepositEntry.workDone - (float)num * _resource.workNeeded;
			Item item = ItemManager.Create(_resource.type, num, (ulong)0);
			Interface.CallHook("OnQuarryGather", this, item);
			if (item.MoveToContainer(this.hopperPrefab.instance.GetComponent<StorageContainer>().inventory, -1, true))
			{
				continue;
			}
			item.Remove(0f);
			this.SetOn(false);
		}
		if (!this.FuelCheck())
		{
			this.SetOn(false);
		}
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			if (this.fuelStoragePrefab.instance == null || this.hopperPrefab.instance == null)
			{
				Debug.Log("Cannot save mining quary because children were null");
				return;
			}
			info.msg.miningQuarry = Pool.Get<ProtoBuf.MiningQuarry>();
			info.msg.miningQuarry.extractor = Pool.Get<ResourceExtractor>();
			info.msg.miningQuarry.extractor.fuelContents = this.fuelStoragePrefab.instance.GetComponent<StorageContainer>().inventory.Save();
			info.msg.miningQuarry.extractor.outputContents = this.hopperPrefab.instance.GetComponent<StorageContainer>().inventory.Save();
			info.msg.miningQuarry.staticType = (int)this.staticType;
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (!this.isStatic)
		{
			this._linkedDeposit = ResourceDepositManager.GetOrCreate(base.transform.position);
		}
		else
		{
			this.UpdateStaticDeposit();
		}
		this.SpawnChildEntities();
		this.engineSwitchPrefab.instance.SetFlag(BaseEntity.Flags.On, base.HasFlag(BaseEntity.Flags.On), false, true);
	}

	public void SetOn(bool isOn)
	{
		base.SetFlag(BaseEntity.Flags.On, isOn, false, true);
		this.engineSwitchPrefab.instance.SetFlag(BaseEntity.Flags.On, isOn, false, true);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		this.engineSwitchPrefab.instance.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		if (!isOn)
		{
			base.CancelInvoke(new Action(this.ProcessResources));
			return;
		}
		base.InvokeRepeating(new Action(this.ProcessResources), this.processRate, this.processRate);
		Interface.CallHook("OnQuarryEnabled", this);
	}

	public void SpawnChildEntities()
	{
		this.engineSwitchPrefab.DoSpawn(this);
		this.hopperPrefab.DoSpawn(this);
		this.fuelStoragePrefab.DoSpawn(this);
	}

	public void Update()
	{
	}

	public void UpdateStaticDeposit()
	{
		if (!this.isStatic)
		{
			return;
		}
		if (this._linkedDeposit != null)
		{
			this._linkedDeposit._resources.Clear();
		}
		else
		{
			this._linkedDeposit = new ResourceDepositManager.ResourceDeposit();
		}
		if (this.staticType == MiningQuarry.QuarryType.None)
		{
			this._linkedDeposit.Add(ItemManager.FindItemDefinition("stones"), 1f, 1000, 0.3f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
			this._linkedDeposit.Add(ItemManager.FindItemDefinition("metal.ore"), 1f, 1000, 5f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
			this._linkedDeposit.Add(ItemManager.FindItemDefinition("sulfur.ore"), 1f, 1000, 7.5f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
			this._linkedDeposit.Add(ItemManager.FindItemDefinition("hq.metal.ore"), 1f, 1000, 75f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
		}
		else if (this.staticType == MiningQuarry.QuarryType.Basic)
		{
			this._linkedDeposit.Add(ItemManager.FindItemDefinition("metal.ore"), 1f, 1000, 2f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
			this._linkedDeposit.Add(ItemManager.FindItemDefinition("stones"), 1f, 1000, 0.3f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
		}
		else if (this.staticType == MiningQuarry.QuarryType.Sulfur)
		{
			this._linkedDeposit.Add(ItemManager.FindItemDefinition("sulfur.ore"), 1f, 1000, 2f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
		}
		else if (this.staticType == MiningQuarry.QuarryType.HQM)
		{
			this._linkedDeposit.Add(ItemManager.FindItemDefinition("hq.metal.ore"), 1f, 1000, 30f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
		}
		this._linkedDeposit.Add(ItemManager.FindItemDefinition("crude.oil"), 1f, 1000, 10f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, true);
	}

	[Serializable]
	public class ChildPrefab
	{
		public GameObjectRef prefabToSpawn;

		public GameObject origin;

		public BaseEntity instance;

		public ChildPrefab()
		{
		}

		public void DoSpawn(MiningQuarry owner)
		{
			if (!this.prefabToSpawn.isValid)
			{
				return;
			}
			this.instance = GameManager.server.CreateEntity(this.prefabToSpawn.resourcePath, this.origin.transform.localPosition, this.origin.transform.localRotation, true);
			this.instance.SetParent(owner, false, false);
			this.instance.Spawn();
		}
	}

	[Serializable]
	public enum QuarryType
	{
		None,
		Basic,
		Sulfur,
		HQM
	}
}