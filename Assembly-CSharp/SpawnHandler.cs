using ConVar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

public class SpawnHandler : SingletonComponent<SpawnHandler>
{
	public float TickInterval = 60f;

	public int MinSpawnsPerTick = 100;

	public int MaxSpawnsPerTick = 100;

	public LayerMask PlacementMask;

	public LayerMask PlacementCheckMask;

	public float PlacementCheckHeight = 25f;

	public LayerMask RadiusCheckMask;

	public float RadiusCheckDistance = 5f;

	public LayerMask BoundsCheckMask;

	public SpawnFilter CharacterSpawn;

	public SpawnPopulation[] SpawnPopulations;

	public SpawnDistribution[] SpawnDistributions;

	internal SpawnDistribution CharDistribution;

	public List<ISpawnGroup> SpawnGroups = new List<ISpawnGroup>();

	internal List<SpawnIndividual> SpawnIndividuals = new List<SpawnIndividual>();

	[ReadOnly]
	public SpawnPopulation[] ConvarSpawnPopulations;

	private Dictionary<SpawnPopulation, SpawnDistribution> population2distribution;

	private bool spawnTick;

	public SpawnPopulation[] AllSpawnPopulations;

	public SpawnHandler()
	{
	}

	public void AddInstance(Spawnable spawnable)
	{
		SpawnDistribution spawnDistribution;
		if (spawnable.Population != null)
		{
			if (!this.population2distribution.TryGetValue(spawnable.Population, out spawnDistribution))
			{
				UnityEngine.Debug.LogWarning(string.Concat("[SpawnHandler] trying to add instance to invalid population: ", spawnable.Population));
				return;
			}
			spawnDistribution.AddInstance(spawnable);
		}
	}

	public void AddRespawn(SpawnIndividual individual)
	{
		this.SpawnIndividuals.Add(individual);
	}

	private bool CheckBounds(Prefab<Spawnable> prefab, Vector3 pos, Quaternion rot, Vector3 scale)
	{
		if (this.BoundsCheckMask != 0)
		{
			BaseEntity component = prefab.Object.GetComponent<BaseEntity>();
			if (component != null && UnityEngine.Physics.CheckBox(pos + (rot * Vector3.Scale(component.bounds.center, scale)), Vector3.Scale(component.bounds.extents, scale), rot, this.BoundsCheckMask))
			{
				return false;
			}
		}
		return true;
	}

	public void DumpReport(string filename)
	{
		File.AppendAllText(filename, string.Concat("\r\n\r\nSpawnHandler Report:\r\n\r\n", this.GetReport(true)));
	}

	public void EnforceLimits(bool forceAll = false)
	{
		if (this.SpawnDistributions == null)
		{
			return;
		}
		for (int i = 0; i < (int)this.AllSpawnPopulations.Length; i++)
		{
			if (this.AllSpawnPopulations[i] != null)
			{
				SpawnPopulation allSpawnPopulations = this.AllSpawnPopulations[i];
				SpawnDistribution spawnDistributions = this.SpawnDistributions[i];
				if (forceAll || allSpawnPopulations.EnforcePopulationLimits)
				{
					this.EnforceLimits(allSpawnPopulations, spawnDistributions);
				}
			}
		}
	}

	private void EnforceLimits(SpawnPopulation population, SpawnDistribution distribution)
	{
		int targetCount = this.GetTargetCount(population, distribution);
		Spawnable[] spawnableArray = this.FindAll(population);
		if ((int)spawnableArray.Length <= targetCount)
		{
			return;
		}
		UnityEngine.Debug.Log(string.Concat(new object[] { population, " has ", (int)spawnableArray.Length, " objects, but max allowed is ", targetCount }));
		int length = (int)spawnableArray.Length - targetCount;
		UnityEngine.Debug.Log(string.Concat(" - deleting ", length, " objects"));
		foreach (Spawnable spawnable in spawnableArray.Take<Spawnable>(length))
		{
			BaseEntity baseEntity = spawnable.gameObject.ToBaseEntity();
			if (!baseEntity.IsValid())
			{
				GameManager.Destroy(spawnable.gameObject, 0f);
			}
			else
			{
				baseEntity.Kill(BaseNetworkable.DestroyMode.None);
			}
		}
	}

	public void FillGroups()
	{
		for (int i = 0; i < this.SpawnGroups.Count; i++)
		{
			this.SpawnGroups[i].Fill();
		}
	}

	public void FillIndividuals()
	{
		for (int i = 0; i < this.SpawnIndividuals.Count; i++)
		{
			SpawnIndividual item = this.SpawnIndividuals[i];
			this.Spawn(Prefab.Load<Spawnable>(item.PrefabID, null, null), item.Position, item.Rotation);
		}
	}

	public void FillPopulations()
	{
		if (this.SpawnDistributions == null)
		{
			return;
		}
		for (int i = 0; i < (int)this.AllSpawnPopulations.Length; i++)
		{
			if (this.AllSpawnPopulations[i] != null)
			{
				this.SpawnInitial(this.AllSpawnPopulations[i], this.SpawnDistributions[i]);
			}
		}
	}

	public Spawnable[] FindAll(SpawnPopulation population)
	{
		return UnityEngine.Object.FindObjectsOfType<Spawnable>().Where<Spawnable>((Spawnable x) => {
			if (!x.gameObject.activeInHierarchy)
			{
				return false;
			}
			return x.Population == population;
		}).ToArray<Spawnable>();
	}

	public int GetCurrentCount(SpawnPopulation population, SpawnDistribution distribution)
	{
		return distribution.Count;
	}

	public string GetReport(bool detailed = true)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (this.AllSpawnPopulations == null)
		{
			stringBuilder.AppendLine("Spawn population array is null.");
		}
		if (this.SpawnDistributions == null)
		{
			stringBuilder.AppendLine("Spawn distribution array is null.");
		}
		if (this.AllSpawnPopulations != null && this.SpawnDistributions != null)
		{
			for (int i = 0; i < (int)this.AllSpawnPopulations.Length; i++)
			{
				if (this.AllSpawnPopulations[i] != null)
				{
					SpawnPopulation allSpawnPopulations = this.AllSpawnPopulations[i];
					SpawnDistribution spawnDistributions = this.SpawnDistributions[i];
					if (allSpawnPopulations == null)
					{
						stringBuilder.AppendLine(string.Concat("Population #", i, " is not set."));
					}
					else
					{
						if (string.IsNullOrEmpty(allSpawnPopulations.ResourceFolder))
						{
							stringBuilder.AppendLine(allSpawnPopulations.name);
						}
						else
						{
							stringBuilder.AppendLine(string.Concat(allSpawnPopulations.name, " (autospawn/", allSpawnPopulations.ResourceFolder, ")"));
						}
						if (detailed)
						{
							stringBuilder.AppendLine("\tPrefabs:");
							if (allSpawnPopulations.Prefabs == null)
							{
								stringBuilder.AppendLine("\t\tN/A");
							}
							else
							{
								Prefab<Spawnable>[] prefabs = allSpawnPopulations.Prefabs;
								for (int j = 0; j < (int)prefabs.Length; j++)
								{
									Prefab<Spawnable> prefab = prefabs[j];
									stringBuilder.AppendLine(string.Concat(new object[] { "\t\t", prefab.Name, " - ", prefab.Object }));
								}
							}
						}
						if (spawnDistributions == null)
						{
							stringBuilder.AppendLine(string.Concat("\tDistribution #", i, " is not set."));
						}
						else
						{
							int currentCount = this.GetCurrentCount(allSpawnPopulations, spawnDistributions);
							int targetCount = this.GetTargetCount(allSpawnPopulations, spawnDistributions);
							stringBuilder.AppendLine(string.Concat(new object[] { "\tPopulation: ", currentCount, "/", targetCount }));
						}
					}
					stringBuilder.AppendLine();
				}
			}
		}
		return stringBuilder.ToString();
	}

	public static BasePlayer.SpawnPoint GetSpawnPoint()
	{
		if (SingletonComponent<SpawnHandler>.Instance == null || SingletonComponent<SpawnHandler>.Instance.CharDistribution == null)
		{
			return null;
		}
		BasePlayer.SpawnPoint spawnPoint = new BasePlayer.SpawnPoint();
		for (int i = 0; i < 60; i++)
		{
			if (SingletonComponent<SpawnHandler>.Instance.CharDistribution.Sample(out spawnPoint.pos, out spawnPoint.rot, false, 0f))
			{
				return spawnPoint;
			}
		}
		return null;
	}

	public int GetTargetCount(SpawnPopulation population, SpawnDistribution distribution)
	{
		float size = TerrainMeta.Size.x * TerrainMeta.Size.z;
		float currentSpawnDensity = population.GetCurrentSpawnDensity();
		if (population.ScaleWithSpawnFilter)
		{
			currentSpawnDensity *= distribution.Density;
		}
		return Mathf.RoundToInt(size * currentSpawnDensity);
	}

	public void InitialSpawn()
	{
		if (Spawn.respawn_populations && this.SpawnDistributions != null)
		{
			for (int i = 0; i < (int)this.AllSpawnPopulations.Length; i++)
			{
				if (this.AllSpawnPopulations[i] != null)
				{
					this.SpawnInitial(this.AllSpawnPopulations[i], this.SpawnDistributions[i]);
				}
			}
		}
		if (Spawn.respawn_groups)
		{
			for (int j = 0; j < this.SpawnGroups.Count; j++)
			{
				this.SpawnGroups[j].SpawnInitial();
			}
		}
	}

	protected void OnEnable()
	{
		this.AllSpawnPopulations = this.SpawnPopulations.Concat<SpawnPopulation>(this.ConvarSpawnPopulations).ToArray<SpawnPopulation>();
		base.StartCoroutine(this.SpawnTick());
		base.StartCoroutine(this.SpawnGroupTick());
		base.StartCoroutine(this.SpawnIndividualTick());
	}

	public static float PlayerExcess()
	{
		float single = Mathf.Max(Spawn.player_base, 1f);
		float count = (float)BasePlayer.activePlayerList.Count;
		if (count <= single)
		{
			return 0f;
		}
		return (count - single) / single;
	}

	public static float PlayerFraction()
	{
		float single = (float)Mathf.Max(Server.maxplayers, 1);
		return Mathf.Clamp01((float)BasePlayer.activePlayerList.Count / single);
	}

	public static float PlayerLerp(float min, float max)
	{
		return Mathf.Lerp(min, max, SpawnHandler.PlayerFraction());
	}

	public static float PlayerScale(float scalar)
	{
		return Mathf.Max(1f, SpawnHandler.PlayerExcess() * scalar);
	}

	public void RemoveInstance(Spawnable spawnable)
	{
		SpawnDistribution spawnDistribution;
		if (spawnable.Population != null)
		{
			if (!this.population2distribution.TryGetValue(spawnable.Population, out spawnDistribution))
			{
				UnityEngine.Debug.LogWarning(string.Concat("[SpawnHandler] trying to remove instance from invalid population: ", spawnable.Population));
				return;
			}
			spawnDistribution.RemoveInstance(spawnable);
		}
	}

	private void Spawn(SpawnPopulation population, SpawnDistribution distribution, int targetCount, int numToFill, int numToTry)
	{
		Vector3 vector3;
		Quaternion quaternion;
		if (targetCount == 0)
		{
			return;
		}
		if (!population.Initialize())
		{
			UnityEngine.Debug.LogError(string.Concat("[Spawn] No prefabs to spawn in ", population.ResourceFolder), population);
			return;
		}
		if (Global.developer > 1)
		{
			UnityEngine.Debug.Log(string.Concat(new object[] { "[Spawn] Population ", population.ResourceFolder, " needs to spawn ", numToFill }));
		}
		float single = Mathf.Max((float)population.ClusterSizeMax, distribution.GetGridCellArea() * population.GetMaximumSpawnDensity());
		population.UpdateWeights(distribution, targetCount);
		while (numToFill >= population.ClusterSizeMin && numToTry > 0)
		{
			ByteQuadtree.Element element = distribution.SampleNode();
			int num = UnityEngine.Random.Range(population.ClusterSizeMin, population.ClusterSizeMax + 1);
			num = Mathx.Min(numToTry, numToFill, num);
			for (int i = 0; i < num; i++)
			{
				if (distribution.Sample(out vector3, out quaternion, element, population.AlignToNormal, (float)population.ClusterDithering) && population.Filter.GetFactor(vector3) > 0f && (float)distribution.GetCount(vector3) < single)
				{
					this.Spawn(population, vector3, quaternion);
					numToFill--;
				}
				numToTry--;
			}
		}
	}

	private GameObject Spawn(SpawnPopulation population, Vector3 pos, Quaternion rot)
	{
		Prefab<Spawnable> randomPrefab = population.GetRandomPrefab();
		if (randomPrefab == null)
		{
			return null;
		}
		if (randomPrefab.Component == null)
		{
			UnityEngine.Debug.LogError(string.Concat("[Spawn] Missing component 'Spawnable' on ", randomPrefab.Name));
			return null;
		}
		Vector3 vector3 = Vector3.one;
		DecorComponent[] decorComponentArray = PrefabAttribute.server.FindAll<DecorComponent>(randomPrefab.ID);
		randomPrefab.Object.transform.ApplyDecorComponents(decorComponentArray, ref pos, ref rot, ref vector3);
		if (!randomPrefab.ApplyTerrainAnchors(ref pos, rot, vector3, TerrainAnchorMode.MinimizeMovement, population.Filter))
		{
			return null;
		}
		if (!randomPrefab.ApplyTerrainChecks(pos, rot, vector3, population.Filter))
		{
			return null;
		}
		if (!randomPrefab.ApplyTerrainFilters(pos, rot, vector3, null))
		{
			return null;
		}
		if (!randomPrefab.ApplyWaterChecks(pos, rot, vector3))
		{
			return null;
		}
		if (!this.CheckBounds(randomPrefab, pos, rot, vector3))
		{
			return null;
		}
		if (randomPrefab.Component.Population != population)
		{
			randomPrefab.Component.Population = population;
		}
		if (Global.developer > 1)
		{
			UnityEngine.Debug.Log(string.Concat("[Spawn] Spawning ", randomPrefab.Name));
		}
		BaseEntity baseEntity = randomPrefab.SpawnEntity(pos, rot);
		if (baseEntity != null)
		{
			baseEntity.Spawn();
			return baseEntity.gameObject;
		}
		UnityEngine.Debug.LogWarning(string.Concat("[Spawn] Couldn't create prefab as entity - ", randomPrefab.Name));
		return null;
	}

	private GameObject Spawn(Prefab<Spawnable> prefab, Vector3 pos, Quaternion rot)
	{
		if (!this.CheckBounds(prefab, pos, rot, Vector3.one))
		{
			return null;
		}
		BaseEntity baseEntity = prefab.SpawnEntity(pos, rot);
		if (baseEntity != null)
		{
			baseEntity.Spawn();
			return baseEntity.gameObject;
		}
		UnityEngine.Debug.LogWarning(string.Concat("[Spawn] Couldn't create prefab as entity - ", prefab.Name));
		return null;
	}

	private IEnumerator SpawnGroupTick()
	{
		SpawnHandler spawnHandler = null;
		while (true)
		{
			yield return CoroutineEx.waitForEndOfFrame;
			if (spawnHandler.spawnTick && Spawn.respawn_groups)
			{
				yield return CoroutineEx.waitForSeconds(1f);
				for (int i = 0; i < spawnHandler.SpawnGroups.Count; i++)
				{
					ISpawnGroup item = spawnHandler.SpawnGroups[i];
					if (item != null)
					{
						try
						{
							item.SpawnRepeating();
						}
						catch (Exception exception)
						{
							UnityEngine.Debug.LogError(exception);
						}
						yield return CoroutineEx.waitForEndOfFrame;
					}
				}
			}
		}
	}

	private IEnumerator SpawnIndividualTick()
	{
		SpawnHandler spawnHandler = null;
		while (true)
		{
			yield return CoroutineEx.waitForEndOfFrame;
			if (spawnHandler.spawnTick && Spawn.respawn_individuals)
			{
				yield return CoroutineEx.waitForSeconds(Spawn.tick_individuals);
				for (int i = 0; i < spawnHandler.SpawnIndividuals.Count; i++)
				{
					SpawnIndividual item = spawnHandler.SpawnIndividuals[i];
					try
					{
						spawnHandler.Spawn(Prefab.Load<Spawnable>(item.PrefabID, null, null), item.Position, item.Rotation);
					}
					catch (Exception exception)
					{
						UnityEngine.Debug.LogError(exception);
					}
					yield return CoroutineEx.waitForEndOfFrame;
				}
			}
		}
	}

	public void SpawnInitial(SpawnPopulation population, SpawnDistribution distribution)
	{
		int targetCount = this.GetTargetCount(population, distribution);
		int currentCount = targetCount - this.GetCurrentCount(population, distribution);
		this.Spawn(population, distribution, targetCount, currentCount, currentCount * population.SpawnAttemptsInitial);
	}

	public void SpawnRepeating(SpawnPopulation population, SpawnDistribution distribution)
	{
		int targetCount = this.GetTargetCount(population, distribution);
		int currentCount = targetCount - this.GetCurrentCount(population, distribution);
		currentCount = Mathf.RoundToInt((float)currentCount * population.GetCurrentSpawnRate());
		currentCount = UnityEngine.Random.Range(Mathf.Min(currentCount, this.MinSpawnsPerTick), Mathf.Min(currentCount, this.MaxSpawnsPerTick));
		this.Spawn(population, distribution, targetCount, currentCount, currentCount * population.SpawnAttemptsRepeating);
	}

	private IEnumerator SpawnTick()
	{
		SpawnHandler spawnHandler = null;
		while (true)
		{
			yield return CoroutineEx.waitForEndOfFrame;
			if (spawnHandler.spawnTick && Spawn.respawn_populations)
			{
				yield return CoroutineEx.waitForSeconds(Spawn.tick_populations);
				for (int i = 0; i < (int)spawnHandler.AllSpawnPopulations.Length; i++)
				{
					SpawnPopulation allSpawnPopulations = spawnHandler.AllSpawnPopulations[i];
					if (allSpawnPopulations != null)
					{
						SpawnDistribution spawnDistributions = spawnHandler.SpawnDistributions[i];
						if (spawnDistributions != null)
						{
							try
							{
								if (spawnHandler.SpawnDistributions != null)
								{
									spawnHandler.SpawnRepeating(allSpawnPopulations, spawnDistributions);
								}
							}
							catch (Exception exception)
							{
								UnityEngine.Debug.LogError(exception);
							}
							yield return CoroutineEx.waitForEndOfFrame;
						}
					}
				}
			}
		}
	}

	public void StartSpawnTick()
	{
		this.spawnTick = true;
	}

	public void UpdateDistributions()
	{
		if (World.Size == 0)
		{
			return;
		}
		this.SpawnDistributions = new SpawnDistribution[(int)this.AllSpawnPopulations.Length];
		this.population2distribution = new Dictionary<SpawnPopulation, SpawnDistribution>();
		Vector3 size = TerrainMeta.Size;
		Vector3 position = TerrainMeta.Position;
		int num = Mathf.NextPowerOfTwo((int)((float)((float)World.Size) * 0.25f));
		for (int i1 = 0; i1 < (int)this.AllSpawnPopulations.Length; i1++)
		{
			SpawnPopulation allSpawnPopulations = this.AllSpawnPopulations[i1];
			if (allSpawnPopulations != null)
			{
				byte[] factor = new byte[num * num];
				SpawnFilter filter = allSpawnPopulations.Filter;
				Parallel.For(0, num, (int z) => {
					for (int i = 0; i < num; i++)
					{
						float popRes = ((float)i + 0.5f) / (float)num;
						float single = ((float)z + 0.5f) / (float)num;
						factor[z * num + i] = (byte)(255f * filter.GetFactor(popRes, single));
					}
				});
				SpawnDistribution[] spawnDistributions = this.SpawnDistributions;
				SpawnDistribution spawnDistribution = new SpawnDistribution(this, factor, position, size);
				SpawnDistribution spawnDistribution1 = spawnDistribution;
				spawnDistributions[i1] = spawnDistribution;
				this.population2distribution.Add(allSpawnPopulations, spawnDistribution1);
			}
			else
			{
				UnityEngine.Debug.LogError("Spawn handler contains null spawn population.");
			}
		}
		int num1 = Mathf.NextPowerOfTwo((int)((float)((float)World.Size) * 0.5f));
		byte[] numArray = new byte[num1 * num1];
		SpawnFilter characterSpawn = this.CharacterSpawn;
		Parallel.For(0, num1, (int z) => {
			for (int i = 0; i < num1; i++)
			{
				float charRes = ((float)i + 0.5f) / (float)num1;
				float single = ((float)z + 0.5f) / (float)num1;
				numArray[z * num1 + i] = (byte)(255f * characterSpawn.GetFactor(charRes, single));
			}
		});
		this.CharDistribution = new SpawnDistribution(this, numArray, position, size);
	}
}