using System;
using System.Runtime.CompilerServices;

public class GenerateClutterTopology : ProceduralComponent
{
	public GenerateClutterTopology()
	{
	}

	public override void Process(uint seed)
	{
		int[] topologyMap = TerrainMeta.TopologyMap.dst;
		int num = TerrainMeta.TopologyMap.res;
		ImageProcessing.Dilate2D(topologyMap, num, num, 16777728, 3, (int x, int y) => {
			if ((topologyMap[x * num + y] & 512) == 0)
			{
				topologyMap[x * num + y] |= 16777216;
			}
		});
	}
}