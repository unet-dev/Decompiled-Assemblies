using Oxide.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDepositManager : BaseEntity
{
	public static ResourceDepositManager _manager;

	private const int resolution = 20;

	public Dictionary<Vector2i, ResourceDepositManager.ResourceDeposit> _deposits;

	public ResourceDepositManager()
	{
		ResourceDepositManager._manager = this;
		this._deposits = new Dictionary<Vector2i, ResourceDepositManager.ResourceDeposit>();
	}

	public ResourceDepositManager.ResourceDeposit CreateFromPosition(Vector3 pos)
	{
		Vector2i indexFrom = ResourceDepositManager.GetIndexFrom(pos);
		UnityEngine.Random.State state = UnityEngine.Random.state;
		UnityEngine.Random.InitState((int)(new Vector2((float)indexFrom.x, (float)indexFrom.y)).Seed(World.Seed + World.Salt));
		ResourceDepositManager.ResourceDeposit resourceDeposit = new ResourceDepositManager.ResourceDeposit()
		{
			origin = new Vector3((float)(indexFrom.x * 20), 0f, (float)(indexFrom.y * 20))
		};
		if (UnityEngine.Random.Range(0f, 1f) < 0.5f)
		{
			resourceDeposit.Add(ItemManager.FindItemDefinition("stones"), 1f, 100, 1f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
		}
		else if (0 == 0)
		{
			resourceDeposit.Add(ItemManager.FindItemDefinition("stones"), 1f, UnityEngine.Random.Range(30000, 100000), UnityEngine.Random.Range(0.3f, 0.5f), ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
			float single = 0f;
			single = (!World.Procedural ? 0.1f : (TerrainMeta.BiomeMap.GetBiome(pos, 2) > 0.5f ? 1f : 0f) * 0.25f);
			if (UnityEngine.Random.Range(0f, 1f) >= 1f - single)
			{
				resourceDeposit.Add(ItemManager.FindItemDefinition("metal.ore"), 1f, UnityEngine.Random.Range(10000, 100000), UnityEngine.Random.Range(2f, 4f), ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
			}
			float single1 = 0f;
			single1 = (!World.Procedural ? 0.1f : (TerrainMeta.BiomeMap.GetBiome(pos, 1) > 0.5f ? 1f : 0f) * (0.25f + 0.25f * (TerrainMeta.TopologyMap.GetTopology(pos, 8) ? 1f : 0f) + 0.25f * (TerrainMeta.TopologyMap.GetTopology(pos, 1) ? 1f : 0f)));
			if (UnityEngine.Random.Range(0f, 1f) >= 1f - single1)
			{
				resourceDeposit.Add(ItemManager.FindItemDefinition("sulfur.ore"), 1f, UnityEngine.Random.Range(10000, 100000), UnityEngine.Random.Range(4f, 4f), ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
			}
			float single2 = 0f;
			if (!World.Procedural)
			{
				single2 += 0.15f;
			}
			else if (TerrainMeta.BiomeMap.GetBiome(pos, 8) > 0.5f || TerrainMeta.BiomeMap.GetBiome(pos, 4) > 0.5f)
			{
				single2 += 0.25f;
			}
			if (UnityEngine.Random.Range(0f, 1f) >= 1f - single2)
			{
				resourceDeposit.Add(ItemManager.FindItemDefinition("hq.metal.ore"), 1f, UnityEngine.Random.Range(5000, 10000), UnityEngine.Random.Range(30f, 50f), ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
			}
		}
		this._deposits.Add(indexFrom, resourceDeposit);
		Interface.CallHook("OnResourceDepositCreated", resourceDeposit);
		UnityEngine.Random.state = state;
		return resourceDeposit;
	}

	public static ResourceDepositManager Get()
	{
		return ResourceDepositManager._manager;
	}

	public ResourceDepositManager.ResourceDeposit GetFromPosition(Vector3 pos)
	{
		ResourceDepositManager.ResourceDeposit resourceDeposit = null;
		if (this._deposits.TryGetValue(ResourceDepositManager.GetIndexFrom(pos), out resourceDeposit))
		{
			return resourceDeposit;
		}
		return null;
	}

	public static Vector2i GetIndexFrom(Vector3 pos)
	{
		return new Vector2i((int)pos.x / 20, (int)pos.z / 20);
	}

	public static ResourceDepositManager.ResourceDeposit GetOrCreate(Vector3 pos)
	{
		ResourceDepositManager.ResourceDeposit fromPosition = ResourceDepositManager.Get().GetFromPosition(pos);
		if (fromPosition != null)
		{
			return fromPosition;
		}
		return ResourceDepositManager.Get().CreateFromPosition(pos);
	}

	[Serializable]
	public class ResourceDeposit
	{
		public float lastSurveyTime;

		public Vector3 origin;

		public List<ResourceDepositManager.ResourceDeposit.ResourceDepositEntry> _resources;

		public ResourceDeposit()
		{
			this._resources = new List<ResourceDepositManager.ResourceDeposit.ResourceDepositEntry>();
		}

		public void Add(ItemDefinition type, float efficiency, int amount, float workNeeded, ResourceDepositManager.ResourceDeposit.surveySpawnType spawnType, bool liquid = false)
		{
			ResourceDepositManager.ResourceDeposit.ResourceDepositEntry resourceDepositEntry = new ResourceDepositManager.ResourceDeposit.ResourceDepositEntry()
			{
				type = type,
				efficiency = efficiency
			};
			int num = amount;
			int num1 = num;
			resourceDepositEntry.amount = num;
			resourceDepositEntry.startAmount = num1;
			resourceDepositEntry.spawnType = spawnType;
			resourceDepositEntry.workNeeded = workNeeded;
			resourceDepositEntry.isLiquid = liquid;
			this._resources.Add(resourceDepositEntry);
		}

		[Serializable]
		public class ResourceDepositEntry
		{
			public ItemDefinition type;

			public float efficiency;

			public int amount;

			public int startAmount;

			public float workNeeded;

			public float workDone;

			public ResourceDepositManager.ResourceDeposit.surveySpawnType spawnType;

			public bool isLiquid;

			public ResourceDepositEntry()
			{
			}

			public void Subtract(int subamount)
			{
				if (subamount <= 0)
				{
					return;
				}
				this.amount -= subamount;
				if (this.amount < 0)
				{
					this.amount = 0;
				}
			}
		}

		[Serializable]
		public enum surveySpawnType
		{
			ITEM,
			OIL,
			WATER
		}
	}
}