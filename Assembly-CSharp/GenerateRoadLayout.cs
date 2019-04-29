using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GenerateRoadLayout : ProceduralComponent
{
	public const float Width = 10f;

	public const float InnerPadding = 1f;

	public const float OuterPadding = 1f;

	public const float InnerFade = 1f;

	public const float OuterFade = 8f;

	public const float RandomScale = 0.75f;

	public const float MeshOffset = 0f;

	public const float TerrainOffset = -0.5f;

	private const int MaxDepth = 100000;

	public GenerateRoadLayout()
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
				int num1 = SeedRandom.Range(ref seed, 100, 500);
				float slope = heightMap.GetSlope(single2, single1);
				int topology = topologyMap.GetTopology(single2, single1, single);
				int num2 = 2295686;
				int num3 = 49152;
				if (slope > 20f || (topology & num2) != 0)
				{
					numArray[i, j] = 2147483647;
				}
				else if ((topology & num3) == 0)
				{
					numArray[i, j] = 1 + (int)(slope * slope * 10f) + num1;
				}
				else
				{
					numArray[i, j] = 2500;
				}
			}
		}
		PathFinder pathFinder = new PathFinder(numArray, true);
		List<GenerateRoadLayout.PathSegment> pathSegments = new List<GenerateRoadLayout.PathSegment>();
		List<GenerateRoadLayout.PathNode> pathNodes = new List<GenerateRoadLayout.PathNode>();
		List<GenerateRoadLayout.PathNode> pathNodes1 = new List<GenerateRoadLayout.PathNode>();
		List<PathFinder.Point> points = new List<PathFinder.Point>();
		List<PathFinder.Point> points1 = new List<PathFinder.Point>();
		List<PathFinder.Point> points2 = new List<PathFinder.Point>();
		foreach (MonumentInfo monument in monuments)
		{
			bool count = pathNodes.Count == 0;
			foreach (TerrainPathConnect target in monument.GetTargets(InfrastructureType.Road))
			{
				PathFinder.Node node = pathFinder.FindClosestWalkable(target.GetPoint(num), 100000);
				if (node == null)
				{
					continue;
				}
				GenerateRoadLayout.PathNode pathNode = new GenerateRoadLayout.PathNode()
				{
					monument = monument,
					target = target,
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
				GenerateRoadLayout.PathSegment pathSegment = new GenerateRoadLayout.PathSegment();
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
				GenerateRoadLayout.PathNode pathNode1 = pathNodes1.Find((GenerateRoadLayout.PathNode x) => {
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
				pathNodes1.RemoveAll((GenerateRoadLayout.PathNode x) => x.monument == pathNode1.monument);
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
				GenerateRoadLayout.PathNode item = pathNodes1[0];
				pathNodes.AddRange(
					from x in pathNodes1
					where x.monument == item.monument
					select x);
				pathNodes1.RemoveAll((GenerateRoadLayout.PathNode x) => x.monument == item.monument);
			}
		}
		foreach (GenerateRoadLayout.PathNode pathNode2 in pathNodes)
		{
			GenerateRoadLayout.PathSegment pathSegment1 = pathSegments.Find((GenerateRoadLayout.PathSegment x) => {
				if (x.start.point == pathNode2.node.point)
				{
					return true;
				}
				return x.end.point == pathNode2.node.point;
			});
			if (pathSegment1 == null)
			{
				continue;
			}
			if (pathSegment1.start.point != pathNode2.node.point)
			{
				if (pathSegment1.end.point != pathNode2.node.point)
				{
					continue;
				}
				pathSegment1.end.next = pathNode2.node;
				pathSegment1.end = pathFinder.FindEnd(pathNode2.node);
				pathSegment1.target = pathNode2.target;
			}
			else
			{
				PathFinder.Node node2 = pathNode2.node;
				PathFinder.Node node3 = pathFinder.Reverse(pathNode2.node);
				node2.next = pathSegment1.start;
				pathSegment1.start = node3;
				pathSegment1.origin = pathNode2.target;
			}
		}
		List<Vector3> vector3s = new List<Vector3>();
		foreach (GenerateRoadLayout.PathSegment pathSegment2 in pathSegments)
		{
			bool flag = false;
			bool flag1 = false;
			for (PathFinder.Node m = pathSegment2.start; m != null; m = m.next)
			{
				float single3 = ((float)m.point.x + 0.5f) / (float)num;
				float single4 = ((float)m.point.y + 0.5f) / (float)num;
				if (pathSegment2.start == m && pathSegment2.origin != null)
				{
					flag = true;
					single3 = TerrainMeta.NormalizeX(pathSegment2.origin.transform.position.x);
					single4 = TerrainMeta.NormalizeZ(pathSegment2.origin.transform.position.z);
				}
				else if (pathSegment2.end == m && pathSegment2.target != null)
				{
					flag1 = true;
					single3 = TerrainMeta.NormalizeX(pathSegment2.target.transform.position.x);
					single4 = TerrainMeta.NormalizeZ(pathSegment2.target.transform.position.z);
				}
				float single5 = TerrainMeta.DenormalizeX(single3);
				float single6 = TerrainMeta.DenormalizeZ(single4);
				float single7 = Mathf.Max(heightMap.GetHeight(single3, single4), 1f);
				vector3s.Add(new Vector3(single5, single7, single6));
			}
			if (vector3s.Count == 0)
			{
				continue;
			}
			if (vector3s.Count >= 2)
			{
				PathList pathList = new PathList(string.Concat("Road ", pathLists.Count), vector3s.ToArray())
				{
					Width = 10f,
					InnerPadding = 1f,
					OuterPadding = 1f,
					InnerFade = 1f,
					OuterFade = 8f,
					RandomScale = 0.75f,
					MeshOffset = 0f,
					TerrainOffset = -0.5f,
					Topology = 2048,
					Splat = 128,
					Start = flag,
					End = flag1
				};
				pathLists.Add(pathList);
			}
			vector3s.Clear();
		}
		foreach (PathList pathList1 in pathLists)
		{
			pathList1.Path.Smoothen(2);
		}
		TerrainMeta.Path.Roads.AddRange(pathLists);
	}

	private class PathNode
	{
		public MonumentInfo monument;

		public TerrainPathConnect target;

		public PathFinder.Node node;

		public PathNode()
		{
		}
	}

	private class PathSegment
	{
		public PathFinder.Node start;

		public PathFinder.Node end;

		public TerrainPathConnect origin;

		public TerrainPathConnect target;

		public PathSegment()
		{
		}
	}
}