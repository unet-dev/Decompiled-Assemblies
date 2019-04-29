using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingProximity : PrefabAttribute
{
	private const float check_radius = 2f;

	private const float check_forgiveness = 0.01f;

	private const float foundation_width = 3f;

	private const float foundation_extents = 1.5f;

	public BuildingProximity()
	{
	}

	public static bool Check(BasePlayer player, Construction construction, Vector3 position, Quaternion rotation)
	{
		OBB oBB = new OBB(position, rotation, construction.bounds);
		float single = oBB.extents.magnitude + 2f;
		List<BuildingBlock> list = Pool.GetList<BuildingBlock>();
		Vis.Entities<BuildingBlock>(oBB.position, single, list, 2097152, QueryTriggerInteraction.Collide);
		uint d = 0;
		for (int i = 0; i < list.Count; i++)
		{
			BuildingBlock item = list[i];
			Construction construction1 = construction;
			Construction construction2 = item.blockDefinition;
			Vector3 vector3 = position;
			Vector3 vector31 = item.transform.position;
			Quaternion quaternion = rotation;
			Quaternion quaternion1 = item.transform.rotation;
			BuildingProximity.ProximityInfo proximity = BuildingProximity.GetProximity(construction1, vector3, quaternion, construction2, vector31, quaternion1);
			BuildingProximity.ProximityInfo proximityInfo = BuildingProximity.GetProximity(construction2, vector31, quaternion1, construction1, vector3, quaternion);
			BuildingProximity.ProximityInfo proximityInfo1 = new BuildingProximity.ProximityInfo()
			{
				hit = (proximity.hit ? true : proximityInfo.hit),
				connection = (proximity.connection ? true : proximityInfo.connection)
			};
			if (proximity.sqrDist > proximityInfo.sqrDist)
			{
				proximityInfo1.line = proximityInfo.line;
				proximityInfo1.sqrDist = proximityInfo.sqrDist;
			}
			else
			{
				proximityInfo1.line = proximity.line;
				proximityInfo1.sqrDist = proximity.sqrDist;
			}
			if (proximityInfo1.connection)
			{
				BuildingManager.Building building = item.GetBuilding();
				if (building != null)
				{
					BuildingPrivlidge dominatingBuildingPrivilege = building.GetDominatingBuildingPrivilege();
					if (dominatingBuildingPrivilege != null)
					{
						if (!construction.canBypassBuildingPermission && !dominatingBuildingPrivilege.IsAuthed(player))
						{
							Construction.lastPlacementError = "Cannot attach to unauthorized building";
							Pool.FreeList<BuildingBlock>(ref list);
							return true;
						}
						if (d == 0)
						{
							d = building.ID;
						}
						else if (d != building.ID)
						{
							Construction.lastPlacementError = "Cannot connect two buildings with cupboards";
							Pool.FreeList<BuildingBlock>(ref list);
							return true;
						}
					}
				}
			}
			if (proximityInfo1.hit)
			{
				Vector3 vector32 = proximityInfo1.line.point1 - proximityInfo1.line.point0;
				if (Mathf.Abs(vector32.y) <= 1.49f && vector32.Magnitude2D() <= 1.49f)
				{
					Construction.lastPlacementError = "Not enough space";
					Pool.FreeList<BuildingBlock>(ref list);
					return true;
				}
			}
		}
		Pool.FreeList<BuildingBlock>(ref list);
		return false;
	}

	protected override Type GetIndexedType()
	{
		return typeof(BuildingProximity);
	}

	private static BuildingProximity.ProximityInfo GetProximity(Construction construction1, Vector3 position1, Quaternion rotation1, Construction construction2, Vector3 position2, Quaternion rotation2)
	{
		BuildingProximity.ProximityInfo proximityInfo = new BuildingProximity.ProximityInfo()
		{
			hit = false,
			connection = false,
			line = new Line(),
			sqrDist = Single.MaxValue
		};
		for (int i = 0; i < (int)construction1.allSockets.Length; i++)
		{
			ConstructionSocket constructionSocket = construction1.allSockets[i] as ConstructionSocket;
			if (constructionSocket != null)
			{
				for (int j = 0; j < (int)construction2.allSockets.Length; j++)
				{
					if (constructionSocket.CanConnect(position1, rotation1, construction2.allSockets[j], position2, rotation2))
					{
						proximityInfo.connection = true;
						return proximityInfo;
					}
				}
			}
		}
		if (!proximityInfo.connection && construction1.allProximities.Length != 0)
		{
			for (int k = 0; k < (int)construction1.allSockets.Length; k++)
			{
				ConstructionSocket constructionSocket1 = construction1.allSockets[k] as ConstructionSocket;
				if (!(constructionSocket1 == null) && constructionSocket1.socketType == ConstructionSocket.Type.Wall)
				{
					Vector3 selectPivot = constructionSocket1.GetSelectPivot(position1, rotation1);
					for (int l = 0; l < (int)construction2.allProximities.Length; l++)
					{
						Vector3 vector3 = construction2.allProximities[l].GetSelectPivot(position2, rotation2);
						Line line = new Line(selectPivot, vector3);
						float single = (line.point1 - line.point0).sqrMagnitude;
						if (single < proximityInfo.sqrDist)
						{
							proximityInfo.hit = true;
							proximityInfo.line = line;
							proximityInfo.sqrDist = single;
						}
					}
				}
			}
		}
		return proximityInfo;
	}

	public Vector3 GetSelectPivot(Vector3 position, Quaternion rotation)
	{
		return position + (rotation * this.worldPosition);
	}

	private struct ProximityInfo
	{
		public bool hit;

		public bool connection;

		public Line line;

		public float sqrDist;
	}
}