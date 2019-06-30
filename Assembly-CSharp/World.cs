using ConVar;
using Oxide.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine;

public static class World
{
	public static bool Cached
	{
		get;
		set;
	}

	public static string Checksum
	{
		get;
		set;
	}

	public static string MapFileName
	{
		get
		{
			if (World.CanLoadFromUrl())
			{
				return string.Concat(World.Name, ".map");
			}
			return string.Concat(new object[] { World.Name.Replace(" ", "").ToLower(), ".", World.Size, ".", World.Seed, ".", 179, ".map" });
		}
	}

	public static string MapFolderName
	{
		get
		{
			return Server.rootFolder;
		}
	}

	public static string Name
	{
		get
		{
			if (!World.CanLoadFromUrl())
			{
				return Application.loadedLevelName;
			}
			return Path.GetFileNameWithoutExtension(WWW.UnEscapeURL(World.Url));
		}
	}

	public static bool Procedural
	{
		get;
		set;
	}

	public static uint Salt
	{
		get;
		set;
	}

	public static string SaveFileName
	{
		get
		{
			if (World.CanLoadFromUrl())
			{
				return string.Concat(new object[] { World.Name, ".", 179, ".sav" });
			}
			return string.Concat(new object[] { World.Name.Replace(" ", "").ToLower(), ".", World.Size, ".", World.Seed, ".", 179, ".sav" });
		}
	}

	public static string SaveFolderName
	{
		get
		{
			return Server.rootFolder;
		}
	}

	public static uint Seed
	{
		get;
		set;
	}

	public static WorldSerialization Serialization
	{
		get;
		set;
	}

	public static uint Size
	{
		get;
		set;
	}

	public static string Url
	{
		get;
		set;
	}

	public static void AddMap(string name, byte[] data)
	{
		World.Serialization.AddMap(name, data);
	}

	public static void AddPath(PathList path)
	{
		World.Serialization.AddPath(World.PathListToPathData(path));
	}

	public static void AddPaths(IEnumerable<PathList> paths)
	{
		foreach (PathList path in paths)
		{
			World.AddPath(path);
		}
	}

	public static void AddPrefab(string category, uint id, Vector3 position, Quaternion rotation, Vector3 scale)
	{
		World.Serialization.AddPrefab(category, id, position, rotation, scale);
		if (!World.Cached)
		{
			World.Spawn(category, id, position, rotation, scale);
		}
	}

	public static bool CanLoadFromDisk()
	{
		return File.Exists(string.Concat(World.MapFolderName, "/", World.MapFileName));
	}

	public static bool CanLoadFromUrl()
	{
		return !string.IsNullOrEmpty(World.Url);
	}

	public static void CleanupOldFiles()
	{
		Regex regex = new Regex("proceduralmap\\.[0-9]+\\.[0-9]+\\.[0-9]+\\.map");
		Regex regex1 = new Regex(string.Concat("\\.[0-9]+\\.[0-9]+\\.", 179, "\\.map"));
		foreach (string str in Directory.GetFiles(World.MapFolderName, "*.map").Where<string>((string path) => {
			if (!regex.IsMatch(path))
			{
				return false;
			}
			return !regex1.IsMatch(path);
		}))
		{
			try
			{
				File.Delete(str);
			}
			catch (Exception exception)
			{
				UnityEngine.Debug.LogError(exception.Message);
			}
		}
	}

	public static byte[] GetMap(string name)
	{
		WorldSerialization.MapData map = World.Serialization.GetMap(name);
		if (map == null)
		{
			return null;
		}
		return map.data;
	}

	public static IEnumerable<PathList> GetPaths(string name)
	{
		return 
			from p in World.Serialization.GetPaths(name)
			select World.PathDataToPathList(p);
	}

	public static void InitSalt(int salt)
	{
		World.InitSalt((uint)salt);
	}

	public static void InitSalt(uint salt)
	{
		if (salt == 0)
		{
			salt = World.SaltIdentifier().MurmurHashUnsigned() % 2147483647;
		}
		if (salt == 0)
		{
			salt = 654321;
		}
		World.Salt = salt;
		Server.salt = (int)salt;
	}

	public static void InitSeed(int seed)
	{
		World.InitSeed((uint)seed);
	}

	public static void InitSeed(uint seed)
	{
		if (seed == 0)
		{
			seed = World.SeedIdentifier().MurmurHashUnsigned() % 2147483647;
		}
		if (seed == 0)
		{
			seed = 123456;
		}
		World.Seed = seed;
		Server.seed = (int)seed;
	}

	public static void InitSize(int size)
	{
		World.InitSize((uint)size);
	}

	public static void InitSize(uint size)
	{
		if (size == 0)
		{
			size = 4000;
		}
		if (size < 1000)
		{
			size = 1000;
		}
		if (size > 6000)
		{
			size = 6000;
		}
		World.Size = size;
		Server.worldsize = (int)size;
	}

	public static PathList PathDataToPathList(WorldSerialization.PathData src)
	{
		PathList pathList = new PathList(src.name, Array.ConvertAll<WorldSerialization.VectorData, Vector3>(src.nodes, (WorldSerialization.VectorData item) => item))
		{
			Spline = src.spline,
			Start = src.start,
			End = src.end,
			Width = src.width,
			InnerPadding = src.innerPadding,
			OuterPadding = src.outerPadding,
			InnerFade = src.innerFade,
			OuterFade = src.outerFade,
			RandomScale = src.randomScale,
			MeshOffset = src.meshOffset,
			TerrainOffset = src.terrainOffset,
			Splat = src.splat,
			Topology = src.topology
		};
		pathList.Path.RecalculateTangents();
		return pathList;
	}

	public static WorldSerialization.PathData PathListToPathData(PathList src)
	{
		return new WorldSerialization.PathData()
		{
			name = src.Name,
			spline = src.Spline,
			start = src.Start,
			end = src.End,
			width = src.Width,
			innerPadding = src.InnerPadding,
			outerPadding = src.OuterPadding,
			innerFade = src.InnerFade,
			outerFade = src.OuterFade,
			randomScale = src.RandomScale,
			meshOffset = src.MeshOffset,
			terrainOffset = src.TerrainOffset,
			splat = src.Splat,
			topology = src.Topology,
			nodes = Array.ConvertAll<Vector3, WorldSerialization.VectorData>(src.Path.Points, (Vector3 item) => item)
		};
	}

	private static string SaltIdentifier()
	{
		return string.Concat(SystemInfo.deviceUniqueIdentifier, "_salt");
	}

	private static string SeedIdentifier()
	{
		return string.Concat(SystemInfo.deviceUniqueIdentifier, "_", 179);
	}

	public static IEnumerator Spawn(float deltaTime, Action<string> statusFunction = null)
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		for (int i = 0; i < World.Serialization.world.prefabs.Count; i++)
		{
			if (stopwatch.Elapsed.TotalSeconds > (double)deltaTime || i == 0 || i == World.Serialization.world.prefabs.Count - 1)
			{
				World.Status(statusFunction, "Spawning World ({0}/{1})", i + 1, World.Serialization.world.prefabs.Count);
				yield return CoroutineEx.waitForEndOfFrame;
				stopwatch.Reset();
				stopwatch.Start();
			}
			World.Spawn(World.Serialization.world.prefabs[i]);
		}
	}

	public static void Spawn()
	{
		for (int i = 0; i < World.Serialization.world.prefabs.Count; i++)
		{
			World.Spawn(World.Serialization.world.prefabs[i]);
		}
	}

	private static void Spawn(WorldSerialization.PrefabData prefab)
	{
		World.Spawn(prefab.category, prefab.id, prefab.position, prefab.rotation, prefab.scale);
	}

	private static void Spawn(string category, uint id, Vector3 position, Quaternion rotation, Vector3 scale)
	{
		GameObject gameObject = Prefab.DefaultManager.CreatePrefab(StringPool.Get(id), position, rotation, scale, true);
		if (gameObject)
		{
			Interface.CallHook("OnWorldPrefabSpawned", gameObject, category);
			gameObject.SetHierarchyGroup(category, true, false);
		}
	}

	private static void Status(Action<string> statusFunction, string status, object obj1)
	{
		if (statusFunction != null)
		{
			statusFunction(string.Format(status, obj1));
		}
	}

	private static void Status(Action<string> statusFunction, string status, object obj1, object obj2)
	{
		if (statusFunction != null)
		{
			statusFunction(string.Format(status, obj1, obj2));
		}
	}

	private static void Status(Action<string> statusFunction, string status, object obj1, object obj2, object obj3)
	{
		if (statusFunction != null)
		{
			statusFunction(string.Format(status, obj1, obj2, obj3));
		}
	}

	private static void Status(Action<string> statusFunction, string status, params object[] objs)
	{
		if (statusFunction != null)
		{
			statusFunction(string.Format(status, objs));
		}
	}
}