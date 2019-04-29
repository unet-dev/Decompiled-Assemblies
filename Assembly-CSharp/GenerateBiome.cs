using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class GenerateBiome : ProceduralComponent
{
	public GenerateBiome()
	{
	}

	[DllImport("RustNative", CharSet=CharSet.None, EntryPoint="generate_biome", ExactSpelling=false)]
	public static extern void Native_GenerateBiome(byte[] map, int res, Vector3 pos, Vector3 size, uint seed, float lootAngle, float biomeAngle, short[] heightmap, int heightres);

	public override void Process(uint seed)
	{
		T[] biomeMap = TerrainMeta.BiomeMap.dst;
		int num = TerrainMeta.BiomeMap.res;
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		float lootAxisAngle = TerrainMeta.LootAxisAngle;
		float biomeAxisAngle = TerrainMeta.BiomeAxisAngle;
		short[] heightMap = TerrainMeta.HeightMap.src;
		int heightMap1 = TerrainMeta.HeightMap.res;
		GenerateBiome.Native_GenerateBiome(biomeMap, num, position, size, seed, lootAxisAngle, biomeAxisAngle, heightMap, heightMap1);
	}
}