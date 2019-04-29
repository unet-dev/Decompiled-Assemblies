using ConVar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class ServerBuildingManager : BuildingManager
{
	private int decayTickBuildingIndex;

	private int decayTickEntityIndex;

	private int decayTickWorldIndex;

	private int navmeshCarveTickBuildingIndex;

	private uint maxBuildingID;

	public ServerBuildingManager()
	{
	}

	public void CheckMerge(DecayEntity ent)
	{
		if (ent.buildingID == 0)
		{
			return;
		}
		BuildingManager.Building building1 = ent.GetBuilding();
		if (building1 == null)
		{
			return;
		}
		ent.EntityLinkMessage<BuildingBlock>((BuildingBlock b) => {
			if (b.buildingID != building1.ID)
			{
				BuildingManager.Building building = b.GetBuilding();
				if (building != null)
				{
					this.Merge(building1, building);
				}
			}
		});
		if (AI.nav_carve_use_building_optimization)
		{
			building1.isNavMeshCarvingDirty = true;
			int num = 2;
			this.UpdateNavMeshCarver(building1, ref num, 0);
		}
	}

	public void CheckSplit(DecayEntity ent)
	{
		if (ent.buildingID == 0)
		{
			return;
		}
		BuildingManager.Building building = ent.GetBuilding();
		if (building == null)
		{
			return;
		}
		if (this.ShouldSplit(building))
		{
			this.Split(building);
		}
	}

	protected override BuildingManager.Building CreateBuilding(uint id)
	{
		return new BuildingManager.Building()
		{
			ID = id
		};
	}

	public void Cycle()
	{
		using (TimeWarning timeWarning = TimeWarning.New("StabilityCheckQueue", 0.1f))
		{
			StabilityEntity.stabilityCheckQueue.RunQueue((double)Stability.stabilityqueue);
		}
		using (timeWarning = TimeWarning.New("UpdateSurroundingsQueue", 0.1f))
		{
			StabilityEntity.updateSurroundingsQueue.RunQueue((double)Stability.surroundingsqueue);
		}
		using (timeWarning = TimeWarning.New("UpdateSkinQueue", 0.1f))
		{
			BuildingBlock.updateSkinQueueServer.RunQueue(1);
		}
		using (timeWarning = TimeWarning.New("BuildingDecayTick", 0.1f))
		{
			int num = 5;
			BufferList<BuildingManager.Building> values = this.buildingDictionary.Values;
			for (int i = this.decayTickBuildingIndex; i < values.Count && num > 0; i++)
			{
				BufferList<DecayEntity> decayEntities = values[i].decayEntities.Values;
				for (int j = this.decayTickEntityIndex; j < decayEntities.Count && num > 0; j++)
				{
					decayEntities[j].DecayTick();
					num--;
					if (num <= 0)
					{
						this.decayTickBuildingIndex = i;
						this.decayTickEntityIndex = j;
					}
				}
				if (num > 0)
				{
					this.decayTickEntityIndex = 0;
				}
			}
			if (num > 0)
			{
				this.decayTickBuildingIndex = 0;
			}
		}
		using (timeWarning = TimeWarning.New("WorldDecayTick", 0.1f))
		{
			int num1 = 5;
			BufferList<DecayEntity> values1 = this.decayEntities.Values;
			for (int k = this.decayTickWorldIndex; k < values1.Count && num1 > 0; k++)
			{
				values1[k].DecayTick();
				num1--;
				if (num1 <= 0)
				{
					this.decayTickWorldIndex = k;
				}
			}
			if (num1 > 0)
			{
				this.decayTickWorldIndex = 0;
			}
		}
		if (AI.nav_carve_use_building_optimization)
		{
			using (timeWarning = TimeWarning.New("NavMeshCarving", 0.1f))
			{
				int num2 = 5;
				BufferList<BuildingManager.Building> buildings = this.buildingDictionary.Values;
				for (int l = this.navmeshCarveTickBuildingIndex; l < buildings.Count && num2 > 0; l++)
				{
					this.UpdateNavMeshCarver(buildings[l], ref num2, l);
				}
				if (num2 > 0)
				{
					this.navmeshCarveTickBuildingIndex = 0;
				}
			}
		}
	}

	protected override void DisposeBuilding(ref BuildingManager.Building building)
	{
		building = null;
	}

	public void LoadBuildingID(uint id)
	{
		this.maxBuildingID = Mathx.Max(this.maxBuildingID, id);
	}

	private void Merge(BuildingManager.Building building1, BuildingManager.Building building2)
	{
		while (building2.HasDecayEntities())
		{
			building2.decayEntities[0].AttachToBuilding(building1.ID);
		}
		if (AI.nav_carve_use_building_optimization)
		{
			building1.isNavMeshCarvingDirty = true;
			building2.isNavMeshCarvingDirty = true;
			int num = 3;
			this.UpdateNavMeshCarver(building1, ref num, 0);
			this.UpdateNavMeshCarver(building1, ref num, 0);
		}
	}

	public uint NewBuildingID()
	{
		uint num = this.maxBuildingID + 1;
		this.maxBuildingID = num;
		return num;
	}

	private bool ShouldSplit(BuildingManager.Building building)
	{
		bool flag;
		if (building.HasBuildingBlocks())
		{
			building.buildingBlocks[0].EntityLinkBroadcast();
			using (IEnumerator<BuildingBlock> enumerator = building.buildingBlocks.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.ReceivedEntityLinkBroadcast())
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			return flag;
		}
		return false;
	}

	private void Split(BuildingManager.Building building)
	{
		uint num;
		uint num1;
		while (building.HasBuildingBlocks())
		{
			BuildingBlock item = building.buildingBlocks[0];
			uint num2 = BuildingManager.server.NewBuildingID();
			item.EntityLinkBroadcast<BuildingBlock>((BuildingBlock b) => b.AttachToBuilding(num2));
		}
		while (building.HasBuildingPrivileges())
		{
			BuildingPrivlidge buildingPrivlidge = building.buildingPrivileges[0];
			BuildingBlock nearbyBuildingBlock = buildingPrivlidge.GetNearbyBuildingBlock();
			if (nearbyBuildingBlock)
			{
				num = nearbyBuildingBlock.buildingID;
			}
			else
			{
				num = 0;
			}
			buildingPrivlidge.AttachToBuilding(num);
		}
		while (building.HasDecayEntities())
		{
			DecayEntity decayEntity = building.decayEntities[0];
			BuildingBlock buildingBlock = decayEntity.GetNearbyBuildingBlock();
			if (buildingBlock)
			{
				num1 = buildingBlock.buildingID;
			}
			else
			{
				num1 = 0;
			}
			decayEntity.AttachToBuilding(num1);
		}
		if (AI.nav_carve_use_building_optimization)
		{
			building.isNavMeshCarvingDirty = true;
			int num3 = 2;
			this.UpdateNavMeshCarver(building, ref num3, 0);
		}
	}

	public void UpdateNavMeshCarver(BuildingManager.Building building, ref int ticks, int i)
	{
		Vector3 vector3;
		if (!AI.nav_carve_use_building_optimization || !building.isNavMeshCarveOptimized && building.navmeshCarvers.Count < AI.nav_carve_min_building_blocks_to_apply_optimization)
		{
			return;
		}
		if (building.isNavMeshCarvingDirty)
		{
			building.isNavMeshCarvingDirty = false;
			if (building.navmeshCarvers == null)
			{
				if (building.buildingNavMeshObstacle != null)
				{
					UnityEngine.Object.Destroy(building.buildingNavMeshObstacle.gameObject);
					building.buildingNavMeshObstacle = null;
					building.isNavMeshCarveOptimized = false;
				}
				return;
			}
			Vector3 item = new Vector3((float)((float)World.Size), (float)((float)World.Size), (float)((float)World.Size));
			Vector3 vector31 = new Vector3((float)(-(ulong)World.Size), (float)(-(ulong)World.Size), (float)(-(ulong)World.Size));
			int count = building.navmeshCarvers.Count;
			if (count > 0)
			{
				for (int num = 0; num < count; num++)
				{
					NavMeshObstacle navMeshObstacle = building.navmeshCarvers[num];
					if (navMeshObstacle.enabled)
					{
						navMeshObstacle.enabled = false;
					}
					for (int j = 0; j < 3; j++)
					{
						if (navMeshObstacle.transform.position[j] < item[j])
						{
							vector3 = navMeshObstacle.transform.position;
							item[j] = vector3[j];
						}
						if (navMeshObstacle.transform.position[j] > vector31[j])
						{
							vector3 = navMeshObstacle.transform.position;
							vector31[j] = vector3[j];
						}
					}
				}
				Vector3 vector32 = (vector31 + item) * 0.5f;
				Vector3 navCarveSizeMultiplier = Vector3.zero;
				float single = Mathf.Abs(vector32.x - item.x);
				float single1 = Mathf.Abs(vector32.y - item.y);
				float single2 = Mathf.Abs(vector32.z - item.z);
				float single3 = Mathf.Abs(vector31.x - vector32.x);
				float single4 = Mathf.Abs(vector31.y - vector32.y);
				float single5 = Mathf.Abs(vector31.z - vector32.z);
				navCarveSizeMultiplier.x = Mathf.Max((single > single3 ? single : single3), AI.nav_carve_min_base_size);
				navCarveSizeMultiplier.y = Mathf.Max((single1 > single4 ? single1 : single4), AI.nav_carve_min_base_size);
				navCarveSizeMultiplier.z = Mathf.Max((single2 > single5 ? single2 : single5), AI.nav_carve_min_base_size);
				if (count >= 10)
				{
					navCarveSizeMultiplier = navCarveSizeMultiplier * (AI.nav_carve_size_multiplier - 1f);
				}
				else
				{
					navCarveSizeMultiplier *= AI.nav_carve_size_multiplier;
				}
				if (building.navmeshCarvers.Count > 0)
				{
					if (building.buildingNavMeshObstacle == null)
					{
						building.buildingNavMeshObstacle = (new GameObject(string.Format("Building ({0}) NavMesh Carver", building.ID))).AddComponent<NavMeshObstacle>();
						building.buildingNavMeshObstacle.enabled = false;
						building.buildingNavMeshObstacle.carving = true;
						building.buildingNavMeshObstacle.shape = NavMeshObstacleShape.Box;
						building.buildingNavMeshObstacle.height = AI.nav_carve_height;
						building.isNavMeshCarveOptimized = true;
					}
					if (building.buildingNavMeshObstacle != null)
					{
						building.buildingNavMeshObstacle.transform.position = vector32;
						building.buildingNavMeshObstacle.size = navCarveSizeMultiplier;
						if (!building.buildingNavMeshObstacle.enabled)
						{
							building.buildingNavMeshObstacle.enabled = true;
						}
					}
				}
			}
			else if (building.buildingNavMeshObstacle != null)
			{
				UnityEngine.Object.Destroy(building.buildingNavMeshObstacle.gameObject);
				building.buildingNavMeshObstacle = null;
				building.isNavMeshCarveOptimized = false;
			}
			ticks--;
			if (ticks <= 0)
			{
				this.navmeshCarveTickBuildingIndex = i;
			}
		}
	}
}