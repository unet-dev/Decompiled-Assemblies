using System;
using UnityEngine;

public class TerrainSplatSet : TerrainModifier
{
	public TerrainSplat.Enum SplatType;

	public TerrainSplatSet()
	{
	}

	protected override void Apply(Vector3 position, float opacity, float radius, float fade)
	{
		if (!TerrainMeta.SplatMap)
		{
			return;
		}
		TerrainMeta.SplatMap.SetSplat(position, (int)this.SplatType, opacity, radius, fade);
	}
}