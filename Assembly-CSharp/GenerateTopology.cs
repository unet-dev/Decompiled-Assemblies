using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class GenerateTopology : ProceduralComponent
{
	public GenerateTopology()
	{
	}

	[DllImport("RustNative", CharSet=CharSet.None, EntryPoint="generate_topology", ExactSpelling=false)]
	public static extern void Native_GenerateTopology(int[] map, int res, Vector3 pos, Vector3 size, uint seed, float lootAngle, float biomeAngle, short[] heightmap, int heightres, byte[] biomemap, int biomeres);

	public override void Process(uint seed)
	{
		T[] topologyMap = TerrainMeta.TopologyMap.dst;
		int num = TerrainMeta.TopologyMap.res;
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		float lootAxisAngle = TerrainMeta.LootAxisAngle;
		float biomeAxisAngle = TerrainMeta.BiomeAxisAngle;
		short[] heightMap = TerrainMeta.HeightMap.src;
		int heightMap1 = TerrainMeta.HeightMap.res;
		byte[] biomeMap = TerrainMeta.BiomeMap.src;
		int biomeMap1 = TerrainMeta.BiomeMap.res;
		GenerateTopology.Native_GenerateTopology(topologyMap, num, position, size, seed, lootAxisAngle, biomeAxisAngle, heightMap, heightMap1, biomeMap, biomeMap1);
	}
}