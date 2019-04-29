using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GenerateCliffSplat : ProceduralComponent
{
	private const int filter = 8389632;

	public GenerateCliffSplat()
	{
	}

	public static void Process(int x, int z)
	{
		TerrainSplatMap splatMap = TerrainMeta.SplatMap;
		float single = splatMap.Coordinate(z);
		float single1 = splatMap.Coordinate(x);
		if ((TerrainMeta.TopologyMap.GetTopology(single1, single) & 8389632) == 0)
		{
			float slope = TerrainMeta.HeightMap.GetSlope(single1, single);
			if (slope > 30f)
			{
				splatMap.SetSplat(x, z, 8, Mathf.InverseLerp(30f, 50f, slope));
			}
		}
	}

	public override void Process(uint seed)
	{
		int splatMap = TerrainMeta.SplatMap.res;
		Parallel.For(0, splatMap, (int z) => {
			for (int i = 0; i < splatMap; i++)
			{
				GenerateCliffSplat.Process(i, z);
			}
		});
	}
}