using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GeneratePowerlineLayout : ProceduralComponent
{
	public const float Width = 10f;

	private const int MaxDepth = 100000;

	public GeneratePowerlineLayout()
	{
	}

	public override void Process(uint seed)
	{
		List<PathList> pathLists = new List<PathList>();
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		List<MonumentInfo> monuments = TerrainMeta.Path.Monuments;
		if (monuments.Count == 0)
		{
			return;
		}
		int num = Mathf.NextPowerOfTwo((int)((float)((float)World.Size) / 10f));
		int[,] numArray = new int[num, num];
		float single = 5f;
		for (int i = 0; i < num; i++)
		{
			float single1 = ((float)i + 0.5f) / (float)num;
			for (int j = 0; j < num; j++)
			{
				float single2 = ((float)j + 0.5f) / (float)num;
				float slope = heightMap.GetSlope(single2, single1);
				int topology = topologyMap.GetTopology(single2, single1, single);
				int num1 = 2295174;
				int num2 = 55296;
				int num3 = 512;
				if ((topology & num1) != 0)
				{
					numArray[i, j] = 2147483647;
				}
				else if ((topology & num2) != 0)
				{
					numArray[i, j] = 2500;
				}
				else if ((topology & num3) == 0)
				{
					numArray[i, j] = 1 + (int)(slope * slope * 10f);
				}
				else
				{
					numArray[i, j] = 1000;
				}
			}
		}
		PathFinder pathFinder = new PathFinder(numArray, true);
		List<GeneratePowerlineLayout.PathSegment> pathSegments = new List<GeneratePowerlineLayout.PathSegment>();
		List<GeneratePowerlineLayout.PathNode> pathNodes = new List<GeneratePowerlineLayout.PathNode>();
		List<GeneratePowerlineLayout.PathNode> pathNodes1 = new List<GeneratePowerlineLayout.PathNode>();
		List<PathFinder.Point> points = new List<PathFinder.Point>();
		List<PathFinder.Point> points1 = new List<PathFinder.Point>();
		List<PathFinder.Point> points2 = new List<PathFinder.Point>();
		foreach (MonumentInfo monument in monuments)
		{
			bool count = pathNodes.Count == 0;
			foreach (TerrainPathConnect target in monument.GetTargets(InfrastructureType.Power))
			{
				PathFinder.Point point = target.GetPoint(num);
				PathFinder.Node node = pathFinder.FindClosestWalkable(point, 100000);
				if (node == null)
				{
					continue;
				}
				GeneratePowerlineLayout.PathNode pathNode = new GeneratePowerlineLayout.PathNode()
				{
					monument = monument,
					node = node
				};
				if (!count)
				{
					pathNodes1.Add(pathNode);
				}
				else
				{
					pathNodes.Add(pathNode);
				}
			}
		}
		while (pathNodes1.Count != 0)
		{
			points1.Clear();
			points2.Clear();
			points1.AddRange(
				from x in pathNodes
				select x.node.point);
			points1.AddRange(points);
			points2.AddRange(
				from x in pathNodes1
				select x.node.point);
			PathFinder.Node node1 = pathFinder.FindPathUndirected(points1, points2, 100000);
			if (node1 != null)
			{
				GeneratePowerlineLayout.PathSegment pathSegment = new GeneratePowerlineLayout.PathSegment();
				for (PathFinder.Node k = node1; k != null; k = k.next)
				{
					if (k == node1)
					{
						pathSegment.start = k;
					}
					if (k.next == null)
					{
						pathSegment.end = k;
					}
				}
				pathSegments.Add(pathSegment);
				GeneratePowerlineLayout.PathNode pathNode1 = pathNodes1.Find((GeneratePowerlineLayout.PathNode x) => {
					if (x.node.point == pathSegment.start.point)
					{
						return true;
					}
					return x.node.point == pathSegment.end.point;
				});
				pathNodes.AddRange(
					from x in pathNodes1
					where x.monument == pathNode1.monument
					select x);
				pathNodes1.RemoveAll((GeneratePowerlineLayout.PathNode x) => x.monument == pathNode1.monument);
				int num4 = 1;
				for (PathFinder.Node l = node1; l != null; l = l.next)
				{
					if (num4 % 8 == 0)
					{
						points.Add(l.point);
					}
					num4++;
				}
			}
			else
			{
				GeneratePowerlineLayout.PathNode item = pathNodes1[0];
				pathNodes.AddRange(
					from x in pathNodes1
					where x.monument == item.monument
					select x);
				pathNodes1.RemoveAll((GeneratePowerlineLayout.PathNode x) => x.monument == item.monument);
			}
		}
		List<Vector3> vector3s = new List<Vector3>();
		foreach (GeneratePowerlineLayout.PathSegment pathSegment1 in pathSegments)
		{
			for (PathFinder.Node m = pathSegment1.start; m != null; m = m.next)
			{
				float single3 = ((float)m.point.x + 0.5f) / (float)num;
				float single4 = ((float)m.point.y + 0.5f) / (float)num;
				float height01 = heightMap.GetHeight01(single3, single4);
				vector3s.Add(TerrainMeta.Denormalize(new Vector3(single3, height01, single4)));
			}
			if (vector3s.Count == 0)
			{
				continue;
			}
			if (vector3s.Count >= 8)
			{
				PathList pathList = new PathList(string.Concat("Powerline ", pathLists.Count), vector3s.ToArray())
				{
					Start = true,
					End = true
				};
				pathLists.Add(pathList);
			}
			vector3s.Clear();
		}
		TerrainMeta.Path.Powerlines.AddRange(pathLists);
	}

	private class PathNode
	{
		public MonumentInfo monument;

		public PathFinder.Node node;

		public PathNode()
		{
		}
	}

	private class PathSegment
	{
		public PathFinder.Node start;

		public PathFinder.Node end;

		public PathSegment()
		{
		}
	}
}