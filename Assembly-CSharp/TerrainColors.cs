using System;
using UnityEngine;

public class TerrainColors : TerrainExtension
{
	private TerrainSplatMap splatMap;

	private TerrainBiomeMap biomeMap;

	public TerrainColors()
	{
	}

	public Color GetColor(Vector3 worldPos, int mask = -1)
	{
		float single = TerrainMeta.NormalizeX(worldPos.x);
		float single1 = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetColor(single, single1, mask);
	}

	public Color GetColor(float normX, float normZ, int mask = -1)
	{
		float biome = this.biomeMap.GetBiome(normX, normZ, 1);
		float single = this.biomeMap.GetBiome(normX, normZ, 2);
		float biome1 = this.biomeMap.GetBiome(normX, normZ, 4);
		float single1 = this.biomeMap.GetBiome(normX, normZ, 8);
		int index = TerrainSplat.TypeToIndex(this.splatMap.GetSplatMaxType(normX, normZ, mask));
		TerrainConfig.SplatType splats = this.config.Splats[index];
		return (((biome * splats.AridColor) + (single * splats.TemperateColor)) + (biome1 * splats.TundraColor)) + (single1 * splats.ArcticColor);
	}

	public override void Setup()
	{
		this.splatMap = this.terrain.GetComponent<TerrainSplatMap>();
		this.biomeMap = this.terrain.GetComponent<TerrainBiomeMap>();
	}
}