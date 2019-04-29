using ConVar;
using System;
using UnityEngine;
using UnityEngine.AI;

public abstract class BuildingManager
{
	public static ServerBuildingManager server;

	protected ListHashSet<DecayEntity> decayEntities = new ListHashSet<DecayEntity>(8);

	protected ListDictionary<uint, BuildingManager.Building> buildingDictionary = new ListDictionary<uint, BuildingManager.Building>(8);

	static BuildingManager()
	{
		BuildingManager.server = new ServerBuildingManager();
	}

	protected BuildingManager()
	{
	}

	public void Add(DecayEntity ent)
	{
		if (ent.buildingID == 0)
		{
			if (!this.decayEntities.Contains(ent))
			{
				this.decayEntities.Add(ent);
			}
			return;
		}
		BuildingManager.Building building = this.GetBuilding(ent.buildingID);
		if (building == null)
		{
			building = this.CreateBuilding(ent.buildingID);
			this.buildingDictionary.Add(ent.buildingID, building);
		}
		building.Add(ent);
		building.Dirty();
	}

	public void Clear()
	{
		this.buildingDictionary.Clear();
	}

	protected abstract BuildingManager.Building CreateBuilding(uint id);

	protected abstract void DisposeBuilding(ref BuildingManager.Building building);

	public BuildingManager.Building GetBuilding(uint buildingID)
	{
		BuildingManager.Building building = null;
		this.buildingDictionary.TryGetValue(buildingID, out building);
		return building;
	}

	public void Remove(DecayEntity ent)
	{
		if (ent.buildingID == 0)
		{
			this.decayEntities.Remove(ent);
			return;
		}
		BuildingManager.Building building = this.GetBuilding(ent.buildingID);
		if (building == null)
		{
			return;
		}
		building.Remove(ent);
		if (!building.IsEmpty())
		{
			building.Dirty();
			return;
		}
		this.buildingDictionary.Remove(ent.buildingID);
		this.DisposeBuilding(ref building);
	}

	public class Building
	{
		public uint ID;

		public ListHashSet<BuildingPrivlidge> buildingPrivileges;

		public ListHashSet<BuildingBlock> buildingBlocks;

		public ListHashSet<DecayEntity> decayEntities;

		public NavMeshObstacle buildingNavMeshObstacle;

		public ListHashSet<NavMeshObstacle> navmeshCarvers;

		public bool isNavMeshCarvingDirty;

		public bool isNavMeshCarveOptimized;

		public Building()
		{
		}

		public void Add(DecayEntity ent)
		{
			this.AddDecayEntity(ent);
			this.AddBuildingBlock(ent as BuildingBlock);
			this.AddBuildingPrivilege(ent as BuildingPrivlidge);
		}

		public void AddBuildingBlock(BuildingBlock ent)
		{
			if (ent == null)
			{
				return;
			}
			if (!this.buildingBlocks.Contains(ent))
			{
				this.buildingBlocks.Add(ent);
				if (AI.nav_carve_use_building_optimization)
				{
					NavMeshObstacle component = ent.GetComponent<NavMeshObstacle>();
					if (component != null)
					{
						this.isNavMeshCarvingDirty = true;
						if (this.navmeshCarvers == null)
						{
							this.navmeshCarvers = new ListHashSet<NavMeshObstacle>(8);
						}
						this.navmeshCarvers.Add(component);
					}
				}
			}
		}

		public void AddBuildingPrivilege(BuildingPrivlidge ent)
		{
			if (ent == null)
			{
				return;
			}
			if (!this.buildingPrivileges.Contains(ent))
			{
				this.buildingPrivileges.Add(ent);
			}
		}

		public void AddDecayEntity(DecayEntity ent)
		{
			if (ent == null)
			{
				return;
			}
			if (!this.decayEntities.Contains(ent))
			{
				this.decayEntities.Add(ent);
			}
		}

		public void Dirty()
		{
			BuildingPrivlidge dominatingBuildingPrivilege = this.GetDominatingBuildingPrivilege();
			if (dominatingBuildingPrivilege != null)
			{
				dominatingBuildingPrivilege.BuildingDirty();
			}
		}

		public BuildingPrivlidge GetDominatingBuildingPrivilege()
		{
			BuildingPrivlidge buildingPrivlidge = null;
			if (this.HasBuildingPrivileges())
			{
				for (int i = 0; i < this.buildingPrivileges.Count; i++)
				{
					BuildingPrivlidge item = this.buildingPrivileges[i];
					if (!(item == null) && item.IsOlderThan(buildingPrivlidge))
					{
						buildingPrivlidge = item;
					}
				}
			}
			return buildingPrivlidge;
		}

		public bool HasBuildingBlocks()
		{
			if (this.buildingBlocks == null)
			{
				return false;
			}
			return this.buildingBlocks.Count > 0;
		}

		public bool HasBuildingPrivileges()
		{
			if (this.buildingPrivileges == null)
			{
				return false;
			}
			return this.buildingPrivileges.Count > 0;
		}

		public bool HasDecayEntities()
		{
			if (this.decayEntities == null)
			{
				return false;
			}
			return this.decayEntities.Count > 0;
		}

		public bool IsEmpty()
		{
			if (this.HasBuildingPrivileges())
			{
				return false;
			}
			if (this.HasBuildingBlocks())
			{
				return false;
			}
			if (this.HasDecayEntities())
			{
				return false;
			}
			return true;
		}

		public void Remove(DecayEntity ent)
		{
			this.RemoveDecayEntity(ent);
			this.RemoveBuildingBlock(ent as BuildingBlock);
			this.RemoveBuildingPrivilege(ent as BuildingPrivlidge);
		}

		public void RemoveBuildingBlock(BuildingBlock ent)
		{
			if (ent == null)
			{
				return;
			}
			this.buildingBlocks.Remove(ent);
			if (AI.nav_carve_use_building_optimization && this.navmeshCarvers != null)
			{
				NavMeshObstacle component = ent.GetComponent<NavMeshObstacle>();
				if (component != null)
				{
					this.navmeshCarvers.Remove(component);
					if (this.navmeshCarvers.Count == 0)
					{
						this.navmeshCarvers = null;
					}
					this.isNavMeshCarvingDirty = true;
					if (this.navmeshCarvers == null)
					{
						BuildingManager.Building building = ent.GetBuilding();
						if (building != null)
						{
							int num = 2;
							BuildingManager.server.UpdateNavMeshCarver(building, ref num, 0);
						}
					}
				}
			}
		}

		public void RemoveBuildingPrivilege(BuildingPrivlidge ent)
		{
			if (ent == null)
			{
				return;
			}
			this.buildingPrivileges.Remove(ent);
		}

		public void RemoveDecayEntity(DecayEntity ent)
		{
			if (ent == null)
			{
				return;
			}
			this.decayEntities.Remove(ent);
		}
	}
}