using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GenerateDecorTopology : ProceduralComponent
{
	public bool KeepExisting = true;

	public GenerateDecorTopology()
	{
	}

	public override void Process(uint seed)
	{
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		int num = topologyMap.res;
		Parallel.For(0, num, (int z) => {
			for (int i = 0; i < num; i++)
			{
				if (topologyMap.GetTopology(i, z, 4194306))
				{
					topologyMap.AddTopology(i, z, 512);
				}
				else if (!this.KeepExisting)
				{
					topologyMap.RemoveTopology(i, z, 512);
				}
			}
		});
	}
}