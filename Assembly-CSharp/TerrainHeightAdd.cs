using System;
using UnityEngine;

public class TerrainHeightAdd : TerrainModifier
{
	public float Delta = 1f;

	public TerrainHeightAdd()
	{
	}

	protected override void Apply(Vector3 position, float opacity, float radius, float fade)
	{
		if (!TerrainMeta.HeightMap)
		{
			return;
		}
		TerrainMeta.HeightMap.AddHeight(position, opacity * this.Delta * TerrainMeta.OneOverSize.y, radius, fade);
	}
}