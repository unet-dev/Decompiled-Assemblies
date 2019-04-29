using ConVar;
using Facepunch;
using ProtoBuf;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DecayEntity : BaseCombatEntity
{
	public GameObjectRef debrisPrefab;

	[NonSerialized]
	public uint buildingID;

	public float decayTimer;

	public float upkeepTimer;

	private Upkeep upkeep;

	public Decay decay;

	public DecayPoint[] decayPoints;

	private float lastDecayTick;

	public float decayVariance = 1f;

	public DecayEntity()
	{
	}

	public void AddUpkeepTime(float time)
	{
		this.upkeepTimer -= time;
	}

	public void AttachToBuilding(uint id)
	{
		if (base.isServer)
		{
			BuildingManager.server.Remove(this);
			this.buildingID = id;
			BuildingManager.server.Add(this);
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
	}

	public void AttachToBuilding(DecayEntity other)
	{
		if (other != null)
		{
			this.AttachToBuilding(other.buildingID);
			BuildingManager.server.CheckMerge(this);
			return;
		}
		if (this is BuildingBlock)
		{
			this.AttachToBuilding(BuildingManager.server.NewBuildingID());
			return;
		}
		BuildingBlock nearbyBuildingBlock = this.GetNearbyBuildingBlock();
		if (nearbyBuildingBlock)
		{
			this.AttachToBuilding(nearbyBuildingBlock.buildingID);
		}
	}

	public void CalculateUpkeepCostAmounts(List<ItemAmount> itemAmounts, float multiplier)
	{
		if (this.upkeep == null)
		{
			return;
		}
		float single = this.upkeep.upkeepMultiplier * multiplier;
		if (single == 0f)
		{
			return;
		}
		List<ItemAmount> itemAmounts1 = this.BuildCost();
		if (itemAmounts1 == null)
		{
			return;
		}
		foreach (ItemAmount itemAmount in itemAmounts1)
		{
			if (itemAmount.itemDef.category != ItemCategory.Resources)
			{
				continue;
			}
			float single1 = itemAmount.amount * single;
			bool flag = false;
			foreach (ItemAmount itemAmount1 in itemAmounts)
			{
				if (itemAmount1.itemDef != itemAmount.itemDef)
				{
					continue;
				}
				itemAmount1.amount += single1;
				flag = true;
				goto Label0;
			}
		Label0:
			if (flag)
			{
				continue;
			}
			itemAmounts.Add(new ItemAmount(itemAmount.itemDef, single1));
		}
	}

	public virtual void DecayTick()
	{
		if (this.decay == null)
		{
			return;
		}
		float single = UnityEngine.Time.time - this.lastDecayTick;
		if (single < ConVar.Decay.tick)
		{
			return;
		}
		this.lastDecayTick = UnityEngine.Time.time;
		if (!this.decay.ShouldDecay(this))
		{
			return;
		}
		float single1 = single * ConVar.Decay.scale;
		if (ConVar.Decay.upkeep)
		{
			this.upkeepTimer += single1;
			if (this.upkeepTimer > 0f)
			{
				BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege();
				if (buildingPrivilege != null)
				{
					this.upkeepTimer -= buildingPrivilege.PurchaseUpkeepTime(this, Mathf.Max(this.upkeepTimer, 600f));
				}
			}
			if (this.upkeepTimer < 1f)
			{
				if (base.healthFraction < 1f && ConVar.Decay.upkeep_heal_scale > 0f && base.SecondsSinceAttacked > 600f)
				{
					float decayDuration = single / this.decay.GetDecayDuration(this) * ConVar.Decay.upkeep_heal_scale;
					this.Heal(this.MaxHealth() * decayDuration);
				}
				return;
			}
			this.upkeepTimer = 1f;
		}
		this.decayTimer += single1;
		if (this.decayTimer < this.decay.GetDecayDelay(this))
		{
			return;
		}
		using (TimeWarning timeWarning = TimeWarning.New("DecayTick", 0.1f))
		{
			float upkeepInsideDecayScale = 1f;
			if (!ConVar.Decay.upkeep)
			{
				for (int i = 0; i < (int)this.decayPoints.Length; i++)
				{
					DecayPoint decayPoint = this.decayPoints[i];
					if (decayPoint.IsOccupied(this))
					{
						upkeepInsideDecayScale -= decayPoint.protection;
					}
				}
			}
			else if (!this.IsOutside())
			{
				upkeepInsideDecayScale *= ConVar.Decay.upkeep_inside_decay_scale;
			}
			if (upkeepInsideDecayScale > 0f)
			{
				float decayDuration1 = single1 / this.decay.GetDecayDuration(this) * this.MaxHealth();
				this.Hurt(decayDuration1 * upkeepInsideDecayScale * this.decayVariance, DamageType.Decay, null, true);
			}
		}
	}

	public void DecayTouch()
	{
		this.decayTimer = 0f;
	}

	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		BuildingManager.server.Remove(this);
		BuildingManager.server.CheckSplit(this);
	}

	public BuildingManager.Building GetBuilding()
	{
		if (!base.isServer)
		{
			return null;
		}
		return BuildingManager.server.GetBuilding(this.buildingID);
	}

	public override BuildingPrivlidge GetBuildingPrivilege()
	{
		BuildingManager.Building building = this.GetBuilding();
		if (building != null)
		{
			return building.GetDominatingBuildingPrivilege();
		}
		return base.GetBuildingPrivilege();
	}

	public BuildingBlock GetNearbyBuildingBlock()
	{
		float single = Single.MaxValue;
		BuildingBlock buildingBlock = null;
		Vector3 vector3 = base.PivotPoint();
		List<BuildingBlock> list = Facepunch.Pool.GetList<BuildingBlock>();
		Vis.Entities<BuildingBlock>(vector3, 1.5f, list, 2097152, QueryTriggerInteraction.Collide);
		for (int i = 0; i < list.Count; i++)
		{
			BuildingBlock item = list[i];
			if (item.isServer == base.isServer)
			{
				float single1 = item.SqrDistance(vector3);
				if (single1 < single)
				{
					single = single1;
					buildingBlock = item;
				}
			}
		}
		Facepunch.Pool.FreeList<BuildingBlock>(ref list);
		return buildingBlock;
	}

	public float GetProtectedSeconds()
	{
		return Mathf.Max(0f, -this.upkeepTimer);
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.decayEntity != null)
		{
			this.decayTimer = info.msg.decayEntity.decayTimer;
			this.upkeepTimer = info.msg.decayEntity.upkeepTimer;
			if (this.buildingID != info.msg.decayEntity.buildingID)
			{
				this.AttachToBuilding(info.msg.decayEntity.buildingID);
				if (info.fromDisk)
				{
					BuildingManager.server.LoadBuildingID(this.buildingID);
				}
			}
		}
	}

	public override void OnKilled(HitInfo info)
	{
		if (this.debrisPrefab.isValid)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.debrisPrefab.resourcePath, base.transform.position, base.transform.rotation, true);
			if (baseEntity)
			{
				baseEntity.Spawn();
			}
		}
		base.OnKilled(info);
	}

	public override void OnRepairFinished()
	{
		base.OnRepairFinished();
		this.DecayTouch();
	}

	public override void ResetState()
	{
		base.ResetState();
		this.buildingID = 0;
		this.decayTimer = 0f;
	}

	public void ResetUpkeepTime()
	{
		this.upkeepTimer = 0f;
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.decayEntity = Facepunch.Pool.Get<ProtoBuf.DecayEntity>();
		info.msg.decayEntity.buildingID = this.buildingID;
		if (info.forDisk)
		{
			info.msg.decayEntity.decayTimer = this.decayTimer;
			info.msg.decayEntity.upkeepTimer = this.upkeepTimer;
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		this.decayVariance = UnityEngine.Random.Range(0.95f, 1f);
		this.decay = PrefabAttribute.server.Find<Decay>(this.prefabID);
		this.decayPoints = PrefabAttribute.server.FindAll<DecayPoint>(this.prefabID);
		this.upkeep = PrefabAttribute.server.Find<Upkeep>(this.prefabID);
		BuildingManager.server.Add(this);
		if (!Rust.Application.isLoadingSave)
		{
			BuildingManager.server.CheckMerge(this);
		}
		this.lastDecayTick = UnityEngine.Time.time;
	}
}