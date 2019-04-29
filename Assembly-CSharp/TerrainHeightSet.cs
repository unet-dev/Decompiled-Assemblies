using System;
using UnityEngine;

public class TerrainHeightSet : TerrainModifier
{
	public TerrainHeightSet()
	{
	}

	protected override void Apply(Vector3 position, float opacity, float radius, float fade)
	{
		if (!TerrainMeta.HeightMap)
		{
			return;
		}
		TerrainMeta.HeightMap.SetHeight(position, opacity, radius, fade);
	}
}