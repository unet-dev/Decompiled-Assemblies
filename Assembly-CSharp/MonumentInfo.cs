using System;
using System.Collections.Generic;
using UnityEngine;

public class MonumentInfo : MonoBehaviour
{
	public MonumentType Type = MonumentType.Building;

	[InspectorFlags]
	public MonumentTier Tier = MonumentTier.Tier0 | MonumentTier.Tier1 | MonumentTier.Tier2;

	[ReadOnly]
	public UnityEngine.Bounds Bounds = new UnityEngine.Bounds(Vector3.zero, Vector3.zero);

	public bool HasNavmesh;

	public bool shouldDisplayOnMap;

	public Translate.Phrase displayPhrase;

	private Dictionary<InfrastructureType, List<TerrainPathConnect>> targets = new Dictionary<InfrastructureType, List<TerrainPathConnect>>();

	public MonumentInfo()
	{
	}

	public void AddTarget(TerrainPathConnect target)
	{
		InfrastructureType type = target.Type;
		if (!this.targets.ContainsKey(type))
		{
			this.targets.Add(type, new List<TerrainPathConnect>());
		}
		this.targets[type].Add(target);
	}

	protected void Awake()
	{
		if (TerrainMeta.Path)
		{
			TerrainMeta.Path.Monuments.Add(this);
		}
		TerrainPathConnect[] componentsInChildren = base.GetComponentsInChildren<TerrainPathConnect>();
		for (int i = 0; i < (int)componentsInChildren.Length; i++)
		{
			this.AddTarget(componentsInChildren[i]);
		}
	}

	public bool CheckPlacement(Vector3 pos, Quaternion rot, Vector3 scale)
	{
		OBB oBB = new OBB(pos, scale, rot, this.Bounds);
		Vector3 point = oBB.GetPoint(-1f, 0f, -1f);
		Vector3 vector3 = oBB.GetPoint(-1f, 0f, 1f);
		Vector3 point1 = oBB.GetPoint(1f, 0f, -1f);
		Vector3 vector31 = oBB.GetPoint(1f, 0f, 1f);
		int topology = TerrainMeta.TopologyMap.GetTopology(point);
		int num = TerrainMeta.TopologyMap.GetTopology(vector3);
		int topology1 = TerrainMeta.TopologyMap.GetTopology(point1);
		int num1 = TerrainMeta.TopologyMap.GetTopology(vector31);
		int num2 = 0;
		if ((int)(this.Tier & MonumentTier.Tier0) != 0)
		{
			num2 |= 67108864;
		}
		if ((int)(this.Tier & MonumentTier.Tier1) != 0)
		{
			num2 |= 134217728;
		}
		if ((int)(this.Tier & MonumentTier.Tier2) != 0)
		{
			num2 |= 268435456;
		}
		if ((num2 & topology) == 0)
		{
			return false;
		}
		if ((num2 & num) == 0)
		{
			return false;
		}
		if ((num2 & topology1) == 0)
		{
			return false;
		}
		if ((num2 & num1) == 0)
		{
			return false;
		}
		return true;
	}

	public MonumentNavMesh GetMonumentNavMesh()
	{
		return base.GetComponent<MonumentNavMesh>();
	}

	public List<TerrainPathConnect> GetTargets(InfrastructureType type)
	{
		if (!this.targets.ContainsKey(type))
		{
			this.targets.Add(type, new List<TerrainPathConnect>());
		}
		return this.targets[type];
	}

	protected void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = new Color(0f, 0.7f, 1f, 0.1f);
		Gizmos.DrawCube(this.Bounds.center, this.Bounds.size);
		Gizmos.color = new Color(0f, 0.7f, 1f, 1f);
		Gizmos.DrawWireCube(this.Bounds.center, this.Bounds.size);
	}
}