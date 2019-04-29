using System;
using UnityEngine;

public class TerrainTopologyAdd : TerrainModifier
{
	[InspectorFlags]
	public TerrainTopology.Enum TopologyType = TerrainTopology.Enum.Decor;

	public TerrainTopologyAdd()
	{
	}

	protected override void Apply(Vector3 position, float opacity, float radius, float fade)
	{
		if (!TerrainMeta.TopologyMap)
		{
			return;
		}
		TerrainMeta.TopologyMap.AddTopology(position, (int)this.TopologyType, radius, fade);
	}
}