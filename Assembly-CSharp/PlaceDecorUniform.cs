using System;
using UnityEngine;

public class PlaceDecorUniform : ProceduralComponent
{
	public SpawnFilter Filter;

	public string ResourceFolder = string.Empty;

	public float ObjectDistance = 10f;

	public float ObjectDithering = 5f;

	public PlaceDecorUniform()
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
		float single = position.x;
		float single1 = position.z;
		float single2 = position.x + size.x;
		float single3 = position.z + size.z;
		for (float i = single1; i < single3; i += this.ObjectDistance)
		{
			for (float j = single; j < single2; j += this.ObjectDistance)
			{
				float single4 = j + SeedRandom.Range(ref seed, -this.ObjectDithering, this.ObjectDithering);
				float single5 = i + SeedRandom.Range(ref seed, -this.ObjectDithering, this.ObjectDithering);
				float single6 = TerrainMeta.NormalizeX(single4);
				float single7 = TerrainMeta.NormalizeZ(single5);
				float single8 = SeedRandom.Value(ref seed);
				float factor = this.Filter.GetFactor(single6, single7);
				Prefab random = prefabArray.GetRandom<Prefab>(ref seed);
				if (factor * factor >= single8)
				{
					float height = heightMap.GetHeight(single6, single7);
					Vector3 vector3 = new Vector3(single4, height, single5);
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
}