using System;
using UnityEngine;

public class TerrainTopologySet : TerrainModifier
{
	[InspectorFlags]
	public TerrainTopology.Enum TopologyType = TerrainTopology.Enum.Decor;

	public TerrainTopologySet()
	{
	}

	protected override void Apply(Vector3 position, float opacity, float radius, float fade)
	{
		if (!TerrainMeta.TopologyMap)
		{
			return;
		}
		TerrainMeta.TopologyMap.SetTopology(position, (int)this.TopologyType, radius, fade);
	}
}