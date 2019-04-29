using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class GenerateRiverTopology : ProceduralComponent
{
	public GenerateRiverTopology()
	{
	}

	public override void Process(uint seed)
	{
		List<PathList> rivers = TerrainMeta.Path.Rivers;
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		foreach (PathList river in rivers)
		{
			river.Path.RecalculateTangents();
		}
		heightMap.Push();
		foreach (PathList pathList in rivers)
		{
			pathList.AdjustTerrainHeight();
			pathList.AdjustTerrainTexture();
			pathList.AdjustTerrainTopology();
		}
		heightMap.Pop();
		int[] numArray = topologyMap.dst;
		int num = topologyMap.res;
		ImageProcessing.Dilate2D(numArray, num, num, 49152, 6, (int x, int y) => {
			if ((numArray[x * num + y] & 49) != 0)
			{
				numArray[x * num + y] |= 32768;
			}
			float single = topologyMap.Coordinate(x);
			float single1 = topologyMap.Coordinate(y);
			if (heightMap.GetSlope(single, single1) > 40f)
			{
				numArray[x * num + y] |= 2;
			}
		});
	}
}