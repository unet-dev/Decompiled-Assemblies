using System;
using System.Collections.Generic;
using UnityEngine;

public class BasePath : MonoBehaviour
{
	public List<BasePathNode> nodes;

	public List<PathInterestNode> interestZones;

	public List<PathSpeedZone> speedZones;

	public BasePath()
	{
	}

	public BasePathNode GetClosestToPoint(Vector3 point)
	{
		BasePathNode item = this.nodes[0];
		float single = Single.PositiveInfinity;
		foreach (BasePathNode node in this.nodes)
		{
			if (node == null || node.transform == null)
			{
				continue;
			}
			float single1 = (point - node.transform.position).sqrMagnitude;
			if (single1 >= single)
			{
				continue;
			}
			single = single1;
			item = node;
		}
		return item;
	}

	public void GetNodesNear(Vector3 point, ref List<BasePathNode> nearNodes, float dist = 10f)
	{
		foreach (BasePathNode node in this.nodes)
		{
			if ((Vector3Ex.XZ(point) - Vector3Ex.XZ(node.transform.position)).sqrMagnitude > dist * dist)
			{
				continue;
			}
			nearNodes.Add(node);
		}
	}

	public PathInterestNode GetRandomInterestNodeAwayFrom(Vector3 from, float dist = 10f)
	{
		PathInterestNode item = null;
		for (int i = 0; item == null && i < 20; i++)
		{
			item = this.interestZones[UnityEngine.Random.Range(0, this.interestZones.Count)];
			if ((item.transform.position - from).sqrMagnitude >= 100f)
			{
				break;
			}
			item = null;
		}
		if (item == null)
		{
			item = this.interestZones[0];
		}
		return item;
	}

	public void Start()
	{
	}
}