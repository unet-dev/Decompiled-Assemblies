using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainPathChildObjects : MonoBehaviour
{
	public bool Spline = true;

	public float Width;

	public float Offset;

	public float Fade;

	[InspectorFlags]
	public TerrainSplat.Enum Splat = TerrainSplat.Enum.Dirt;

	[InspectorFlags]
	public TerrainTopology.Enum Topology = TerrainTopology.Enum.Road;

	public InfrastructureType Type;

	public TerrainPathChildObjects()
	{
	}

	protected void Awake()
	{
		List<Vector3> vector3s = new List<Vector3>();
		foreach (Transform transforms in base.transform)
		{
			vector3s.Add(transforms.position);
		}
		if (vector3s.Count >= 2)
		{
			InfrastructureType type = this.Type;
			if (type == InfrastructureType.Road)
			{
				PathList pathList = new PathList(string.Concat("Road ", TerrainMeta.Path.Roads.Count), vector3s.ToArray())
				{
					Width = this.Width,
					InnerFade = this.Fade * 0.5f,
					OuterFade = this.Fade * 0.5f,
					MeshOffset = this.Offset * 0.3f,
					TerrainOffset = this.Offset,
					Topology = (int)this.Topology,
					Splat = (int)this.Splat,
					Spline = this.Spline
				};
				TerrainMeta.Path.Roads.Add(pathList);
			}
			else if (type == InfrastructureType.Power)
			{
				PathList pathList1 = new PathList(string.Concat("Powerline ", TerrainMeta.Path.Powerlines.Count), vector3s.ToArray())
				{
					Width = this.Width,
					InnerFade = this.Fade * 0.5f,
					OuterFade = this.Fade * 0.5f,
					MeshOffset = this.Offset * 0.3f,
					TerrainOffset = this.Offset,
					Topology = (int)this.Topology,
					Splat = (int)this.Splat,
					Spline = this.Spline
				};
				TerrainMeta.Path.Powerlines.Add(pathList1);
			}
		}
		GameManager.Destroy(base.gameObject, 0f);
	}

	protected void OnDrawGizmos()
	{
		bool flag = false;
		Vector3 vector3 = Vector3.zero;
		foreach (object obj in base.transform)
		{
			Vector3 vector31 = ((Transform)obj).position;
			if (flag)
			{
				Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 1f);
				GizmosUtil.DrawWirePath(vector3, vector31, 0.5f * this.Width);
			}
			vector3 = vector31;
			flag = true;
		}
	}
}