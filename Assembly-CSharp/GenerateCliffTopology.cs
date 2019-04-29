using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GenerateCliffTopology : ProceduralComponent
{
	public bool KeepExisting = true;

	private const int filter = 8389632;

	private const int filter_del = 55296;

	public GenerateCliffTopology()
	{
	}

	public static void Process(int x, int z)
	{
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		float single = topologyMap.Coordinate(z);
		float single1 = topologyMap.Coordinate(x);
		if ((topologyMap.GetTopology(x, z) & 8389632) == 0)
		{
			float slope = TerrainMeta.HeightMap.GetSlope(single1, single);
			float splat = TerrainMeta.SplatMap.GetSplat(single1, single, 8);
			if (slope > 40f || splat > 0.4f)
			{
				topologyMap.AddTopology(x, z, 2);
				return;
			}
			if (slope < 20f && splat < 0.2f)
			{
				topologyMap.RemoveTopology(x, z, 2);
			}
		}
	}

	private static void Process(int x, int z, bool keepExisting)
	{
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		float single = topologyMap.Coordinate(z);
		float single1 = topologyMap.Coordinate(x);
		int topology = topologyMap.GetTopology(x, z);
		if ((topology & 8389632) == 0)
		{
			float slope = TerrainMeta.HeightMap.GetSlope(single1, single);
			float splat = TerrainMeta.SplatMap.GetSplat(single1, single, 8);
			if (slope > 40f || splat > 0.4f)
			{
				topologyMap.AddTopology(x, z, 2);
				return;
			}
			if (!keepExisting && slope < 20f && splat < 0.2f && (topology & 55296) != 0)
			{
				topologyMap.RemoveTopology(x, z, 2);
			}
		}
	}

	public override void Process(uint seed)
	{
		int[] topologyMap = TerrainMeta.TopologyMap.dst;
		int num = TerrainMeta.TopologyMap.res;
		Parallel.For(0, num, (int z) => {
			for (int i = 0; i < num; i++)
			{
				GenerateCliffTopology.Process(i, z, this.KeepExisting);
			}
		});
		ImageProcessing.Dilate2D(topologyMap, num, num, 4194306, 1, (int x, int y) => {
			if ((topologyMap[x * num + y] & 2) == 0)
			{
				topologyMap[x * num + y] |= 4194304;
			}
		});
	}
}