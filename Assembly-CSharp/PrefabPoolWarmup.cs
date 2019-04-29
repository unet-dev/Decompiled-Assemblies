using Rust;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PrefabPoolWarmup
{
	public PrefabPoolWarmup()
	{
	}

	private static string[] GetAssetList()
	{
		return (
			from x in (IEnumerable<GameManifest.PrefabProperties>)GameManifest.Current.prefabProperties
			where x.pool
			select x.name).ToArray<string>();
	}

	private static void PrefabWarmup(string path)
	{
		if (!string.IsNullOrEmpty(path))
		{
			GameObject gameObject = GameManager.server.FindPrefab(path);
			if (gameObject != null && gameObject.SupportsPooling())
			{
				int serverCount = gameObject.GetComponent<Poolable>().ServerCount;
				List<GameObject> gameObjects = new List<GameObject>();
				for (int i = 0; i < serverCount; i++)
				{
					gameObjects.Add(GameManager.server.CreatePrefab(path, true));
				}
				for (int j = 0; j < serverCount; j++)
				{
					GameManager.server.Retire(gameObjects[j]);
				}
			}
		}
	}

	public static void Run()
	{
		if (Rust.Application.isLoadingPrefabs)
		{
			return;
		}
		Rust.Application.isLoadingPrefabs = true;
		string[] assetList = PrefabPoolWarmup.GetAssetList();
		for (int i = 0; i < (int)assetList.Length; i++)
		{
			PrefabPoolWarmup.PrefabWarmup(assetList[i]);
		}
		Rust.Application.isLoadingPrefabs = false;
	}

	public static IEnumerator Run(float deltaTime, Action<string> statusFunction = null, string format = null)
	{
		string str;
		if (Rust.Application.isLoadingPrefabs)
		{
			yield break;
		}
		Rust.Application.isLoadingPrefabs = true;
		string[] assetList = PrefabPoolWarmup.GetAssetList();
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
			PrefabPoolWarmup.PrefabWarmup(assetList[i]);
		}
		Rust.Application.isLoadingPrefabs = false;
	}
}