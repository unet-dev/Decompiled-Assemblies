using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class GenerateSplat : ProceduralComponent
{
	public GenerateSplat()
	{
	}

	[DllImport("RustNative", CharSet=CharSet.None, EntryPoint="generate_splat", ExactSpelling=false)]
	public static extern void Native_GenerateSplat(byte[] map, int res, Vector3 pos, Vector3 size, uint seed, float lootAngle, float biomeAngle, short[] heightmap, int heightres, byte[] biomemap, int biomeres, int[] topologymap, int topologyres);

	public override void Process(uint seed)
	{
		T[] splatMap = TerrainMeta.SplatMap.dst;
		int num = TerrainMeta.SplatMap.res;
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		float lootAxisAngle = TerrainMeta.LootAxisAngle;
		float biomeAxisAngle = TerrainMeta.BiomeAxisAngle;
		short[] heightMap = TerrainMeta.HeightMap.src;
		int heightMap1 = TerrainMeta.HeightMap.res;
		byte[] biomeMap = TerrainMeta.BiomeMap.src;
		int biomeMap1 = TerrainMeta.BiomeMap.res;
		int[] topologyMap = TerrainMeta.TopologyMap.src;
		int topologyMap1 = TerrainMeta.TopologyMap.res;
		GenerateSplat.Native_GenerateSplat(splatMap, num, position, size, seed, lootAxisAngle, biomeAxisAngle, heightMap, heightMap1, biomeMap, biomeMap1, topologyMap, topologyMap1);
	}
}