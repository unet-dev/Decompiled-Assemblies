using System;
using UnityEngine;

public class PlaceMonument : ProceduralComponent
{
	public SpawnFilter Filter;

	public GameObjectRef Monument;

	private const int Attempts = 10000;

	public PlaceMonument()
	{
	}

	public override void Process(uint seed)
	{
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		float single = position.x;
		float single1 = position.z;
		float single2 = position.x + size.x;
		float single3 = position.z + size.z;
		PlaceMonument.SpawnInfo spawnInfo = new PlaceMonument.SpawnInfo();
		int num = -2147483648;
		Prefab<MonumentInfo> prefab = Prefab.Load<MonumentInfo>(this.Monument.resourceID, null, null);
		for (int i = 0; i < 10000; i++)
		{
			float single4 = SeedRandom.Range(ref seed, single, single2);
			float single5 = SeedRandom.Range(ref seed, single1, single3);
			float single6 = TerrainMeta.NormalizeX(single4);
			float single7 = TerrainMeta.NormalizeZ(single5);
			float single8 = SeedRandom.Value(ref seed);
			float factor = this.Filter.GetFactor(single6, single7);
			if (factor * factor >= single8)
			{
				float height = heightMap.GetHeight(single6, single7);
				Vector3 vector3 = new Vector3(single4, height, single5);
				Quaternion obj = prefab.Object.transform.localRotation;
				Vector3 obj1 = prefab.Object.transform.localScale;
				prefab.ApplyDecorComponents(ref vector3, ref obj, ref obj1);
				if ((!prefab.Component || prefab.Component.CheckPlacement(vector3, obj, obj1)) && prefab.ApplyTerrainAnchors(ref vector3, obj, obj1, this.Filter) && prefab.ApplyTerrainChecks(vector3, obj, obj1, this.Filter) && prefab.ApplyTerrainFilters(vector3, obj, obj1, null) && prefab.ApplyWaterChecks(vector3, obj, obj1) && !prefab.CheckEnvironmentVolumes(vector3, obj, obj1, EnvironmentType.Underground))
				{
					PlaceMonument.SpawnInfo spawnInfo1 = new PlaceMonument.SpawnInfo()
					{
						prefab = prefab,
						position = vector3,
						rotation = obj,
						scale = obj1
					};
					int num1 = -Mathf.RoundToInt(vector3.Magnitude2D());
					if (num1 > num)
					{
						num = num1;
						spawnInfo = spawnInfo1;
					}
				}
			}
		}
		if (num != -2147483648)
		{
			Prefab prefab1 = spawnInfo.prefab;
			Vector3 vector31 = spawnInfo.position;
			Quaternion quaternion = spawnInfo.rotation;
			Vector3 vector32 = spawnInfo.scale;
			prefab1.ApplyTerrainPlacements(vector31, quaternion, vector32);
			prefab1.ApplyTerrainModifiers(vector31, quaternion, vector32);
			World.AddPrefab("Monument", prefab1.ID, vector31, quaternion, vector32);
		}
	}

	private struct SpawnInfo
	{
		public Prefab prefab;

		public Vector3 position;

		public Quaternion rotation;

		public Vector3 scale;
	}
}