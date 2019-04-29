using System;
using UnityEngine;

public class TerrainCarve : TerrainModifier
{
	public TerrainCarve()
	{
	}

	protected override void Apply(Vector3 position, float opacity, float radius, float fade)
	{
		if (!TerrainMeta.AlphaMap)
		{
			return;
		}
		TerrainMeta.AlphaMap.SetAlpha(position, 0f, opacity, radius, fade);
	}
}