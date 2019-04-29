using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class GenerateHeight : ProceduralComponent
{
	public GenerateHeight()
	{
	}

	[DllImport("RustNative", CharSet=CharSet.None, EntryPoint="generate_height", ExactSpelling=false)]
	public static extern void Native_GenerateHeight(short[] map, int res, Vector3 pos, Vector3 size, uint seed, float lootAngle, float biomeAngle);

	public override void Process(uint seed)
	{
		T[] heightMap = TerrainMeta.HeightMap.dst;
		int num = TerrainMeta.HeightMap.res;
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		float lootAxisAngle = TerrainMeta.LootAxisAngle;
		float biomeAxisAngle = TerrainMeta.BiomeAxisAngle;
		GenerateHeight.Native_GenerateHeight(heightMap, num, position, size, seed, lootAxisAngle, biomeAxisAngle);
	}
}