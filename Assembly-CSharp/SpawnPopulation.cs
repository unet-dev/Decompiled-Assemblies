using ConVar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName="Rust/Spawn Population")]
public class SpawnPopulation : BaseScriptableObject
{
	[Header("Spawnables")]
	public string ResourceFolder = string.Empty;

	public GameObjectRef[] ResourceList;

	[FormerlySerializedAs("TargetDensity")]
	[Header("Spawn Info")]
	[SerializeField]
	[Tooltip("Usually per square km")]
	public float _targetDensity = 1f;

	public float SpawnRate = 1f;

	public int ClusterSizeMin = 1;

	public int ClusterSizeMax = 1;

	public int ClusterDithering;

	public int SpawnAttemptsInitial = 20;

	public int SpawnAttemptsRepeating = 10;

	public bool EnforcePopulationLimits = true;

	public bool ScaleWithSpawnFilter = true;

	public bool ScaleWithServerPopulation;

	public bool AlignToNormal;

	public SpawnFilter Filter = new SpawnFilter();

	public Prefab<Spawnable>[] Prefabs;

	private int[] numToSpawn;

	private int sumToSpawn;

	public virtual float TargetDensity
	{
		get
		{
			return this._targetDensity;
		}
	}

	public SpawnPopulation()
	{
	}

	public float GetCurrentSpawnDensity()
	{
		if (!this.ScaleWithServerPopulation)
		{
			return this.TargetDensity * Spawn.max_density * 1E-06f;
		}
		return this.TargetDensity * SpawnHandler.PlayerLerp(Spawn.min_density, Spawn.max_density) * 1E-06f;
	}

	public float GetCurrentSpawnRate()
	{
		if (!this.ScaleWithServerPopulation)
		{
			return this.SpawnRate * Spawn.max_rate;
		}
		return this.SpawnRate * SpawnHandler.PlayerLerp(Spawn.min_rate, Spawn.max_rate);
	}

	public float GetMaximumSpawnDensity()
	{
		if (!this.ScaleWithServerPopulation)
		{
			return 2f * this.TargetDensity * Spawn.max_density * 1E-06f;
		}
		return 2f * this.TargetDensity * SpawnHandler.PlayerLerp(Spawn.min_density, Spawn.max_density) * 1E-06f;
	}

	public Prefab<Spawnable> GetRandomPrefab()
	{
		int num = UnityEngine.Random.Range(0, this.sumToSpawn);
		for (int i = 0; i < (int)this.Prefabs.Length; i++)
		{
			int num1 = num - this.numToSpawn[i];
			num = num1;
			if (num1 < 0)
			{
				this.numToSpawn[i]--;
				this.sumToSpawn--;
				return this.Prefabs[i];
			}
		}
		return null;
	}

	public bool Initialize()
	{
		if (this.Prefabs == null || this.Prefabs.Length == 0)
		{
			if (!string.IsNullOrEmpty(this.ResourceFolder))
			{
				this.Prefabs = Prefab.Load<Spawnable>(string.Concat("assets/bundled/prefabs/autospawn/", this.ResourceFolder), GameManager.server, PrefabAttribute.server, false);
			}
			if (this.ResourceList != null && this.ResourceList.Length != 0)
			{
				this.Prefabs = Prefab.Load<Spawnable>((
					from x in (IEnumerable<GameObjectRef>)this.ResourceList
					select x.resourcePath).ToArray<string>(), GameManager.server, PrefabAttribute.server);
			}
			if (this.Prefabs == null || this.Prefabs.Length == 0)
			{
				return false;
			}
			this.numToSpawn = new int[(int)this.Prefabs.Length];
		}
		return true;
	}

	public void UpdateWeights(SpawnDistribution distribution, int targetCount)
	{
		int num;
		int num1 = 0;
		for (int i = 0; i < (int)this.Prefabs.Length; i++)
		{
			Prefab<Spawnable> prefabs = this.Prefabs[i];
			num1 += (prefabs.Parameters ? prefabs.Parameters.Count : 1);
		}
		int num2 = Mathf.CeilToInt((float)targetCount / (float)num1);
		this.sumToSpawn = 0;
		for (int j = 0; j < (int)this.Prefabs.Length; j++)
		{
			Prefab<Spawnable> prefab = this.Prefabs[j];
			num = (prefab.Parameters ? prefab.Parameters.Count : 1);
			int count = distribution.GetCount(prefab.ID);
			int num3 = Mathf.Max(num * num2 - count, 0);
			this.numToSpawn[j] = num3;
			this.sumToSpawn += num3;
		}
	}
}