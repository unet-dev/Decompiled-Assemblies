using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class GenerateRoadTopology : ProceduralComponent
{
	public GenerateRoadTopology()
	{
	}

	public override void Process(uint seed)
	{
		List<PathList> roads = TerrainMeta.Path.Roads;
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		foreach (PathList road in roads)
		{
			road.Path.RecalculateTangents();
		}
		heightMap.Push();
		foreach (PathList pathList in roads)
		{
			pathList.AdjustTerrainHeight();
			pathList.AdjustTerrainTexture();
			pathList.AdjustTerrainTopology();
		}
		heightMap.Pop();
		int[] numArray = topologyMap.dst;
		int num = topologyMap.res;
		ImageProcessing.Dilate2D(numArray, num, num, 6144, 6, (int x, int y) => {
			if ((numArray[x * num + y] & 49) != 0)
			{
				numArray[x * num + y] |= 4096;
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