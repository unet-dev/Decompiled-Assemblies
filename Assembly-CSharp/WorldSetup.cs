using ConVar;
using Rust;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

public class WorldSetup : SingletonComponent<WorldSetup>
{
	public bool AutomaticallySetup;

	public GameObject terrain;

	public GameObject decorPrefab;

	public GameObject grassPrefab;

	public GameObject spawnPrefab;

	private TerrainMeta terrainMeta;

	public uint EditorSeed;

	public uint EditorSalt;

	public uint EditorSize;

	public string EditorUrl = string.Empty;

	internal List<ProceduralObject> ProceduralObjects = new List<ProceduralObject>();

	public WorldSetup()
	{
	}

	protected override void Awake()
	{
		int i;
		base.Awake();
		Prefab[] prefabArray = Prefab.Load("assets/bundled/prefabs/world", null, null, true);
		for (i = 0; i < (int)prefabArray.Length; i++)
		{
			Prefab prefab = prefabArray[i];
			if (prefab.Object.GetComponent<BaseEntity>() == null)
			{
				prefab.Spawn(Vector3.zero, Quaternion.identity);
			}
			else
			{
				prefab.SpawnEntity(Vector3.zero, Quaternion.identity).Spawn();
			}
		}
		SingletonComponent[] singletonComponentArray = UnityEngine.Object.FindObjectsOfType<SingletonComponent>();
		for (i = 0; i < (int)singletonComponentArray.Length; i++)
		{
			singletonComponentArray[i].SingletonSetup();
		}
		if (this.terrain)
		{
			if (!this.terrain.GetComponent<TerrainGenerator>())
			{
				World.Procedural = false;
				this.terrainMeta = this.terrain.GetComponent<TerrainMeta>();
				this.terrainMeta.Init(null, null);
				this.terrainMeta.SetupComponents();
				World.InitSize(Mathf.RoundToInt(TerrainMeta.Size.x));
				this.CreateObject(this.decorPrefab);
				this.CreateObject(this.grassPrefab);
				this.CreateObject(this.spawnPrefab);
			}
			else
			{
				World.Procedural = true;
			}
		}
		World.Serialization = new WorldSerialization();
		World.Cached = false;
		World.CleanupOldFiles();
		if (this.AutomaticallySetup)
		{
			base.StartCoroutine(this.InitCoroutine());
		}
	}

	private void CancelSetup(string msg)
	{
		UnityEngine.Debug.LogError(msg);
		Rust.Application.Quit();
	}

	protected void CreateObject(GameObject prefab)
	{
		if (prefab == null)
		{
			return;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
		if (gameObject != null)
		{
			gameObject.SetActive(true);
		}
	}

	public IEnumerator InitCoroutine()
	{
		WorldSetup worldSetup = null;
		if (!World.CanLoadFromUrl())
		{
			object[] size = new object[] { "Generating procedural map of size ", World.Size, " with seed ", World.Seed };
			UnityEngine.Debug.Log(string.Concat(size));
		}
		else
		{
			UnityEngine.Debug.Log(string.Concat("Loading custom map from ", World.Url));
		}
		ProceduralComponent[] componentsInChildren = worldSetup.GetComponentsInChildren<ProceduralComponent>(true);
		Timing timing = Timing.Start("Downloading World");
		if (World.Procedural && !World.CanLoadFromDisk() && World.CanLoadFromUrl())
		{
			LoadingScreen.Update("DOWNLOADING WORLD");
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			UnityWebRequest downloadHandlerBuffer = UnityWebRequest.Get(World.Url);
			downloadHandlerBuffer.downloadHandler = new DownloadHandlerBuffer();
			downloadHandlerBuffer.Send();
			while (!downloadHandlerBuffer.isDone)
			{
				float single = downloadHandlerBuffer.downloadProgress * 100f;
				LoadingScreen.Update(string.Concat("DOWNLOADING WORLD ", single.ToString("0.0"), "%"));
				yield return CoroutineEx.waitForEndOfFrame;
			}
			if (downloadHandlerBuffer.isHttpError || downloadHandlerBuffer.isNetworkError)
			{
				string[] name = new string[] { "Couldn't Download Level: ", World.Name, " (", downloadHandlerBuffer.error, ")" };
				worldSetup.CancelSetup(string.Concat(name));
			}
			else
			{
				File.WriteAllBytes(string.Concat(World.MapFolderName, "/", World.MapFileName), downloadHandlerBuffer.downloadHandler.data);
			}
			downloadHandlerBuffer = null;
		}
		timing.End();
		Timing timing1 = Timing.Start("Loading World");
		if (World.Procedural && World.CanLoadFromDisk())
		{
			LoadingScreen.Update("LOADING WORLD");
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			World.Serialization.Load(string.Concat(World.MapFolderName, "/", World.MapFileName));
			World.Cached = true;
		}
		timing1.End();
		if (World.Cached && 9 != World.Serialization.Version)
		{
			object[] version = new object[] { "World cache version mismatch: ", (uint)9, " != ", World.Serialization.Version };
			UnityEngine.Debug.LogWarning(string.Concat(version));
			World.Serialization.Clear();
			World.Cached = false;
			if (World.CanLoadFromUrl())
			{
				worldSetup.CancelSetup(string.Concat("World File Outdated: ", World.Name));
			}
		}
		if (World.Cached && string.IsNullOrEmpty(World.Checksum))
		{
			World.Checksum = World.Serialization.Checksum;
		}
		if (World.Cached)
		{
			World.InitSize(World.Serialization.world.size);
		}
		if (worldSetup.terrain)
		{
			TerrainGenerator terrainGenerator = worldSetup.terrain.GetComponent<TerrainGenerator>();
			if (terrainGenerator)
			{
				worldSetup.terrain = terrainGenerator.CreateTerrain();
				worldSetup.terrainMeta = worldSetup.terrain.GetComponent<TerrainMeta>();
				worldSetup.terrainMeta.Init(null, null);
				worldSetup.terrainMeta.SetupComponents();
				worldSetup.CreateObject(worldSetup.decorPrefab);
				worldSetup.CreateObject(worldSetup.grassPrefab);
				worldSetup.CreateObject(worldSetup.spawnPrefab);
			}
		}
		Timing timing2 = Timing.Start("Spawning World");
		if (World.Cached)
		{
			LoadingScreen.Update("SPAWNING WORLD");
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			TerrainMeta.HeightMap.FromByteArray(World.GetMap("terrain"));
			TerrainMeta.SplatMap.FromByteArray(World.GetMap("splat"));
			TerrainMeta.BiomeMap.FromByteArray(World.GetMap("biome"));
			TerrainMeta.TopologyMap.FromByteArray(World.GetMap("topology"));
			TerrainMeta.AlphaMap.FromByteArray(World.GetMap("alpha"));
			TerrainMeta.WaterMap.FromByteArray(World.GetMap("water"));
			IEnumerator enumerator = World.Spawn(0.2f, (string str) => LoadingScreen.Update(str));
			while (enumerator.MoveNext())
			{
				yield return enumerator.Current;
			}
			TerrainMeta.Path.Clear();
			TerrainMeta.Path.Roads.AddRange(World.GetPaths("Road"));
			TerrainMeta.Path.Rivers.AddRange(World.GetPaths("River"));
			TerrainMeta.Path.Powerlines.AddRange(World.GetPaths("Powerline"));
			enumerator = null;
		}
		timing2.End();
		Timing timing3 = Timing.Start("Processing World");
		if (componentsInChildren.Length != 0)
		{
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				ProceduralComponent proceduralComponent = componentsInChildren[i];
				if (proceduralComponent && proceduralComponent.ShouldRun())
				{
					uint num = (uint)((ulong)World.Seed + (long)i);
					LoadingScreen.Update(proceduralComponent.Description.ToUpper());
					yield return CoroutineEx.waitForEndOfFrame;
					yield return CoroutineEx.waitForEndOfFrame;
					yield return CoroutineEx.waitForEndOfFrame;
					Timing timing4 = Timing.Start(proceduralComponent.Description);
					if (proceduralComponent)
					{
						proceduralComponent.Process(num);
					}
					timing4.End();
					proceduralComponent = null;
				}
			}
		}
		timing3.End();
		Timing timing5 = Timing.Start("Saving World");
		if (ConVar.World.cache && World.Procedural && !World.Cached)
		{
			LoadingScreen.Update("SAVING WORLD");
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			World.Serialization.world.size = World.Size;
			World.AddPaths(TerrainMeta.Path.Roads);
			World.AddPaths(TerrainMeta.Path.Rivers);
			World.AddPaths(TerrainMeta.Path.Powerlines);
			World.Serialization.Save(string.Concat(World.MapFolderName, "/", World.MapFileName));
		}
		timing5.End();
		Timing timing6 = Timing.Start("Calculating Checksum");
		if (string.IsNullOrEmpty(World.Serialization.Checksum))
		{
			LoadingScreen.Update("CALCULATING CHECKSUM");
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			World.Serialization.CalculateChecksum();
		}
		timing6.End();
		if (string.IsNullOrEmpty(World.Checksum))
		{
			World.Checksum = World.Serialization.Checksum;
		}
		Timing timing7 = Timing.Start("Ocean Patrol Paths");
		LoadingScreen.Update("OCEAN PATROL PATHS");
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		if (!BaseBoat.generate_paths || !(TerrainMeta.Path != null))
		{
			UnityEngine.Debug.Log("Skipping ocean patrol paths, baseboat.generate_paths == false");
		}
		else
		{
			TerrainMeta.Path.OceanPatrolFar = BaseBoat.GenerateOceanPatrolPath(200f, 8f);
		}
		timing7.End();
		Timing timing8 = Timing.Start("Finalizing World");
		LoadingScreen.Update("FINALIZING WORLD");
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		if (worldSetup.terrainMeta)
		{
			worldSetup.terrainMeta.BindShaderProperties();
			worldSetup.terrainMeta.PostSetupComponents();
			TerrainMargin.Create();
		}
		World.Serialization.Clear();
		timing8.End();
		LoadingScreen.Update("DONE");
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		if (worldSetup)
		{
			GameManager.Destroy(worldSetup.gameObject, 0f);
		}
	}

	private void OnValidate()
	{
		if (this.terrain == null)
		{
			UnityEngine.Terrain terrain = UnityEngine.Object.FindObjectOfType<UnityEngine.Terrain>();
			if (terrain != null)
			{
				this.terrain = terrain.gameObject;
			}
		}
	}
}