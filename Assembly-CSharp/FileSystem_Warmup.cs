using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FileSystem_Warmup : MonoBehaviour
{
	private static bool run;

	private static bool running;

	private static string[] excludeFilter;

	static FileSystem_Warmup()
	{
		FileSystem_Warmup.run = true;
		FileSystem_Warmup.running = false;
		FileSystem_Warmup.excludeFilter = new string[] { "/bundled/prefabs/autospawn/monument", "/bundled/prefabs/autospawn/mountain", "/bundled/prefabs/autospawn/canyon", "/bundled/prefabs/autospawn/decor", "/bundled/prefabs/navmesh", "/content/ui/", "/prefabs/ui/", "/prefabs/world/", "/prefabs/system/", "/standard assets/", "/third party/" };
	}

	public FileSystem_Warmup()
	{
	}

	private static string[] GetAssetList()
	{
		return (
			from x in (IEnumerable<GameManifest.PrefabProperties>)GameManifest.Current.prefabProperties
			select x.name into x
			where !FileSystem_Warmup.ShouldIgnore(x)
			select x).ToArray<string>();
	}

	private static void PrefabWarmup(string path)
	{
		GameManager.server.FindPrefab(path);
	}

	public static void Run()
	{
		if (!FileSystem_Warmup.run || FileSystem_Warmup.running)
		{
			return;
		}
		FileSystem_Warmup.running = true;
		string[] assetList = FileSystem_Warmup.GetAssetList();
		for (int i = 0; i < (int)assetList.Length; i++)
		{
			FileSystem_Warmup.PrefabWarmup(assetList[i]);
		}
		int num = 0;
		FileSystem_Warmup.run = (bool)num;
		FileSystem_Warmup.running = (bool)num;
	}

	public static IEnumerator Run(float deltaTime, Action<string> statusFunction = null, string format = null)
	{
		string str;
		if (!FileSystem_Warmup.run || FileSystem_Warmup.running)
		{
			yield break;
		}
		FileSystem_Warmup.running = true;
		string[] assetList = FileSystem_Warmup.GetAssetList();
		Stopwatch stopwatch = Stopwatch.StartNew();
		for (int i = 0; i < (int)assetList.Length; i++)
		{
			if (stopwatch.Elapsed.TotalSeconds > (double)deltaTime || i == 0 || i == (int)assetList.Length - 1)
			{
				if (statusFunction != null)
				{
					Action<string> action = statusFunction;
					str = (format != null ? format : "{0}/{1}");
					action(string.Format(str, i + 1, (int)assetList.Length));
				}
				yield return CoroutineEx.waitForEndOfFrame;
				stopwatch.Reset();
				stopwatch.Start();
			}
			FileSystem_Warmup.PrefabWarmup(assetList[i]);
		}
		int num = 0;
		FileSystem_Warmup.run = (bool)num;
		FileSystem_Warmup.running = (bool)num;
	}

	private static bool ShouldIgnore(string path)
	{
		for (int i = 0; i < (int)FileSystem_Warmup.excludeFilter.Length; i++)
		{
			if (path.Contains(FileSystem_Warmup.excludeFilter[i], CompareOptions.IgnoreCase))
			{
				return true;
			}
		}
		return false;
	}
}