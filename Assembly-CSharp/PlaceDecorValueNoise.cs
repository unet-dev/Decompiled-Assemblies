using System;
using UnityEngine;

public class PlaceDecorValueNoise : ProceduralComponent
{
	public SpawnFilter Filter;

	public string ResourceFolder = string.Empty;

	public NoiseParameters Cluster = new NoiseParameters(2, 0.5f, 1f, 0f);

	public float ObjectDensity = 100f;

	public PlaceDecorValueNoise()
	{
	}

	public override void Process(uint seed)
	{
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		Prefab[] prefabArray = Prefab.Load(string.Concat("assets/bundled/prefabs/autospawn/", this.ResourceFolder), null, null, true);
		if (prefabArray == null || prefabArray.Length == 0)
		{
			return;
		}
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		int num = Mathf.RoundToInt(this.ObjectDensity * size.x * size.z * 1E-06f);
		float single = position.x;
		float single1 = position.z;
		float single2 = position.x + size.x;
		float single3 = position.z + size.z;
		float single4 = SeedRandom.Range(ref seed, -1000000f, 1000000f);
		float single5 = SeedRandom.Range(ref seed, -1000000f, 1000000f);
		int octaves = this.Cluster.Octaves;
		float offset = this.Cluster.Offset;
		float frequency = this.Cluster.Frequency * 0.01f;
		float amplitude = this.Cluster.Amplitude;
		for (int i = 0; i < num; i++)
		{
			float single6 = SeedRandom.Range(ref seed, single, single2);
			float single7 = SeedRandom.Range(ref seed, single1, single3);
			float single8 = TerrainMeta.NormalizeX(single6);
			float single9 = TerrainMeta.NormalizeZ(single7);
			float single10 = SeedRandom.Value(ref seed);
			float factor = this.Filter.GetFactor(single8, single9);
			Prefab random = prefabArray.GetRandom<Prefab>(ref seed);
			if (factor > 0f && (offset + Noise.Turbulence(single4 + single6, single5 + single7, octaves, frequency, amplitude, 2f, 0.5f)) * factor * factor >= single10)
			{
				float height = heightMap.GetHeight(single8, single9);
				Vector3 vector3 = new Vector3(single6, height, single7);
				Quaternion obj = random.Object.transform.localRotation;
				Vector3 obj1 = random.Object.transform.localScale;
				random.ApplyDecorComponents(ref vector3, ref obj, ref obj1);
				if (random.ApplyTerrainAnchors(ref vector3, obj, obj1, this.Filter) && random.ApplyTerrainChecks(vector3, obj, obj1, this.Filter) && random.ApplyTerrainFilters(vector3, obj, obj1, null) && random.ApplyWaterChecks(vector3, obj, obj1))
				{
					random.ApplyTerrainModifiers(vector3, obj, obj1);
					World.AddPrefab("Decor", random.ID, vector3, obj, obj1);
				}
			}
		}
	}
}