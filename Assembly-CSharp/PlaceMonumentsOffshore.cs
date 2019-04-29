using System;
using System.Collections.Generic;
using UnityEngine;

public class PlaceMonumentsOffshore : ProceduralComponent
{
	public string ResourceFolder = string.Empty;

	public int MinDistanceFromTerrain = 100;

	public int MaxDistanceFromTerrain = 500;

	public int DistanceBetweenMonuments = 500;

	public int MinSize;

	private const int Candidates = 10;

	private const int Attempts = 10000;

	public PlaceMonumentsOffshore()
	{
	}

	private bool CheckRadius(List<PlaceMonumentsOffshore.SpawnInfo> spawns, Vector3 pos, float radius)
	{
		bool flag;
		float single = radius * radius;
		List<PlaceMonumentsOffshore.SpawnInfo>.Enumerator enumerator = spawns.GetEnumerator();
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
		float maxDistanceFromTerrain = position.x - (float)this.MaxDistanceFromTerrain;
		float minDistanceFromTerrain = position.x - (float)this.MinDistanceFromTerrain;
		float single = position.x + size.x + (float)this.MinDistanceFromTerrain;
		float maxDistanceFromTerrain1 = position.x + size.x + (float)this.MaxDistanceFromTerrain;
		float single1 = position.z - (float)this.MaxDistanceFromTerrain;
		int num = this.MinDistanceFromTerrain;
		float minDistanceFromTerrain1 = position.z + size.z + (float)this.MinDistanceFromTerrain;
		float maxDistanceFromTerrain2 = position.z + size.z + (float)this.MaxDistanceFromTerrain;
		int num1 = 0;
		List<PlaceMonumentsOffshore.SpawnInfo> spawnInfos = new List<PlaceMonumentsOffshore.SpawnInfo>();
		int num2 = 0;
		List<PlaceMonumentsOffshore.SpawnInfo> spawnInfos1 = new List<PlaceMonumentsOffshore.SpawnInfo>();
		for (int i = 0; i < 10; i++)
		{
			num1 = 0;
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
				int num3 = priority;
				int num4 = num3 * num3 * num3 * num3;
				for (int k = 0; k < 10000; k++)
				{
					float single2 = 0f;
					float single3 = 0f;
					switch (seed % 4)
					{
						case 0:
						{
							single2 = SeedRandom.Range(ref seed, maxDistanceFromTerrain, minDistanceFromTerrain);
							single3 = SeedRandom.Range(ref seed, single1, maxDistanceFromTerrain2);
							break;
						}
						case 1:
						{
							single2 = SeedRandom.Range(ref seed, single, maxDistanceFromTerrain1);
							single3 = SeedRandom.Range(ref seed, single1, maxDistanceFromTerrain2);
							break;
						}
						case 2:
						{
							single2 = SeedRandom.Range(ref seed, maxDistanceFromTerrain, maxDistanceFromTerrain1);
							single3 = SeedRandom.Range(ref seed, single1, single1);
							break;
						}
						case 3:
						{
							single2 = SeedRandom.Range(ref seed, maxDistanceFromTerrain, maxDistanceFromTerrain1);
							single3 = SeedRandom.Range(ref seed, minDistanceFromTerrain1, maxDistanceFromTerrain2);
							break;
						}
					}
					float single4 = TerrainMeta.NormalizeX(single2);
					float height = heightMap.GetHeight(single4, TerrainMeta.NormalizeZ(single3));
					Vector3 vector3 = new Vector3(single2, height, single3);
					Quaternion obj = prefab.Object.transform.localRotation;
					Vector3 obj1 = prefab.Object.transform.localScale;
					if (!this.CheckRadius(spawnInfos, vector3, (float)this.DistanceBetweenMonuments))
					{
						prefab.ApplyDecorComponents(ref vector3, ref obj, ref obj1);
						if ((!prefab.Component || prefab.Component.CheckPlacement(vector3, obj, obj1)) && !prefab.CheckEnvironmentVolumes(vector3, obj, obj1, EnvironmentType.Underground))
						{
							PlaceMonumentsOffshore.SpawnInfo spawnInfo = new PlaceMonumentsOffshore.SpawnInfo()
							{
								prefab = prefab,
								position = vector3,
								rotation = obj,
								scale = obj1
							};
							spawnInfos.Add(spawnInfo);
							num1 += num4;
							break;
						}
					}
				}
			}
			if (num1 > num2)
			{
				num2 = num1;
				GenericsUtil.Swap<List<PlaceMonumentsOffshore.SpawnInfo>>(ref spawnInfos, ref spawnInfos1);
			}
		}
		foreach (PlaceMonumentsOffshore.SpawnInfo spawnInfo1 in spawnInfos1)
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