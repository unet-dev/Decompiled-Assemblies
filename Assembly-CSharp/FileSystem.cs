using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public static class FileSystem
{
	public static bool LogDebug;

	public static bool LogTime;

	public static FileSystemBackend Backend;

	static FileSystem()
	{
	}

	public static string[] FindAll(string folder, string search = "")
	{
		return FileSystem.Backend.FindAll(folder, search);
	}

	public static T Load<T>(string filePath, bool complain = true)
	where T : UnityEngine.Object
	{
		if (!filePath.IsLower())
		{
			filePath = filePath.ToLower();
		}
		Stopwatch stopwatch = Stopwatch.StartNew();
		if (FileSystem.LogDebug)
		{
			File.AppendAllText("filesystem_debug.csv", string.Format("{0}\n", filePath));
		}
		T t = FileSystem.Backend.Load<T>(filePath);
		if (complain && t == null)
		{
			UnityEngine.Debug.LogWarning(string.Concat(new object[] { "[FileSystem] Not Found: ", filePath, " (", typeof(T), ")" }));
		}
		if (FileSystem.LogTime)
		{
			TimeSpan elapsed = stopwatch.Elapsed;
			File.AppendAllText("filesystem.csv", string.Format("{0},{1}\n", filePath, elapsed.TotalMilliseconds));
		}
		return t;
	}

	public static T[] LoadAll<T>(string folder, string search = "")
	where T : UnityEngine.Object
	{
		if (!folder.IsLower())
		{
			folder = folder.ToLower();
		}
		return FileSystem.Backend.LoadAll<T>(folder, search);
	}

	public static GameObject LoadPrefab(string filePath)
	{
		return FileSystem.Backend.LoadPrefab(filePath);
	}

	public static GameObject[] LoadPrefabs(string folder)
	{
		return FileSystem.Backend.LoadPrefabs(folder);
	}
}