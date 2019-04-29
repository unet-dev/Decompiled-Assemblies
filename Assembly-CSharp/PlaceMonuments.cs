using System;
using System.Collections.Generic;
using UnityEngine;

public class PlaceMonuments : ProceduralComponent
{
	public SpawnFilter Filter;

	public string ResourceFolder = string.Empty;

	public int Distance = 500;

	public int MinSize;

	private const int Candidates = 10;

	private const int Attempts = 10000;

	public PlaceMonuments()
	{
	}

	private bool CheckRadius(List<PlaceMonuments.SpawnInfo> spawns, Vector3 pos, float radius)
	{
		bool flag;
		float single = radius * radius;
		List<PlaceMonuments.SpawnInfo>.Enumerator enumerator = spawns.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				if ((enumerator.Current.position - pos).sqrMagnitude >= single)
				{
					continue;
				}
				flag = true;
				return flag;
			}
			return false;
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		return flag;
	}

	public override void Process(uint seed)
	{
		int priority;
		if ((ulong)World.Size < (long)this.MinSize)
		{
			return;
		}
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		Prefab<MonumentInfo>[] prefabArray = Prefab.Load<MonumentInfo>(string.Concat("assets/bundled/prefabs/autospawn/", this.ResourceFolder), null, null, true);
		if (prefabArray == null || prefabArray.Length == 0)
		{
			return;
		}
		prefabArray.Shuffle<Prefab<MonumentInfo>>(seed);
		prefabArray.BubbleSort<Prefab<MonumentInfo>>();
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		float single = position.x;
		float single1 = position.z;
		float single2 = position.x + size.x;
		float single3 = position.z + size.z;
		int num = 0;
		List<PlaceMonuments.SpawnInfo> spawnInfos = new List<PlaceMonuments.SpawnInfo>();
		int num1 = 0;
		List<PlaceMonuments.SpawnInfo> spawnInfos1 = new List<PlaceMonuments.SpawnInfo>();
		for (int i = 0; i < 10; i++)
		{
			num = 0;
			spawnInfos.Clear();
			Prefab<MonumentInfo>[] prefabArray1 = prefabArray;
			for (int j = 0; j < (int)prefabArray1.Length; j++)
			{
				Prefab<MonumentInfo> prefab = prefabArray1[j];
				if (prefab.Parameters)
				{
					priority = (int)prefab.Parameters.Priority + (int)PrefabPriority.Low;
				}
				else
				{
					priority = 1;
				}
				int num2 = priority;
				int num3 = num2 * num2 * num2 * num2;
				for (int k = 0; k < 10000; k++)
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
						if (!this.CheckRadius(spawnInfos, vector3, (float)this.Distance))
						{
							prefab.ApplyDecorComponents(ref vector3, ref obj, ref obj1);
							if ((!prefab.Component || prefab.Component.CheckPlacement(vector3, obj, obj1)) && prefab.ApplyTerrainAnchors(ref vector3, obj, obj1, this.Filter) && prefab.ApplyTerrainChecks(vector3, obj, obj1, this.Filter) && prefab.ApplyTerrainFilters(vector3, obj, obj1, null) && prefab.ApplyWaterChecks(vector3, obj, obj1) && !prefab.CheckEnvironmentVolumes(vector3, obj, obj1, EnvironmentType.Underground))
							{
								PlaceMonuments.SpawnInfo spawnInfo = new PlaceMonuments.SpawnInfo()
								{
									prefab = prefab,
									position = vector3,
									rotation = obj,
									scale = obj1
								};
								spawnInfos.Add(spawnInfo);
								num += num3;
								break;
							}
						}
					}
				}
			}
			if (num > num1)
			{
				num1 = num;
				GenericsUtil.Swap<List<PlaceMonuments.SpawnInfo>>(ref spawnInfos, ref spawnInfos1);
			}
		}
		foreach (PlaceMonuments.SpawnInfo spawnInfo1 in spawnInfos1)
		{
			Prefab prefab1 = spawnInfo1.prefab;
			Vector3 vector31 = spawnInfo1.position;
			Quaternion quaternion = spawnInfo1.rotation;
			Vector3 vector32 = spawnInfo1.scale;
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