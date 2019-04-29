using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class FileSystemBackend
{
	public bool isError;

	public string loadingError = "";

	public static Dictionary<string, UnityEngine.Object> cache = new Dictionary<string, UnityEngine.Object>();

	protected FileSystemBackend()
	{
	}

	public string[] FindAll(string folder, string search = "")
	{
		return this.LoadAssetList(folder, search);
	}

	public T Load<T>(string filePath)
	where T : UnityEngine.Object
	{
		T item = default(T);
		if (!this.cache.ContainsKey(filePath))
		{
			item = this.LoadAsset<T>(filePath);
			if (item != null)
			{
				this.cache.Add(filePath, item);
			}
		}
		else
		{
			item = (T)(this.cache[filePath] as T);
		}
		return item;
	}

	public T[] LoadAll<T>(string folder, string search = "")
	where T : UnityEngine.Object
	{
		List<T> ts = new List<T>();
		string[] strArrays = this.FindAll(folder, search);
		for (int i = 0; i < (int)strArrays.Length; i++)
		{
			T t = this.Load<T>(strArrays[i]);
			if (t != null)
			{
				ts.Add(t);
			}
		}
		return ts.ToArray();
	}

	protected abstract T LoadAsset<T>(string filePath)
	where T : UnityEngine.Object;

	protected abstract string[] LoadAssetList(string folder, string search);

	protected void LoadError(string err)
	{
		Debug.LogError(err);
		this.loadingError = err;
		this.isError = true;
	}

	public GameObject LoadPrefab(string filePath)
	{
		if (!filePath.StartsWith("assets/", StringComparison.CurrentCultureIgnoreCase))
		{
			Debug.LogWarning(string.Concat("LoadPrefab - should start with assets/ - ", filePath));
		}
		return this.Load<GameObject>(filePath);
	}

	public GameObject[] LoadPrefabs(string folder)
	{
		if (!folder.EndsWith("/", StringComparison.CurrentCultureIgnoreCase))
		{
			Debug.LogWarning(string.Concat("LoadPrefabs - folder should end in '/' - ", folder));
		}
		if (!folder.StartsWith("assets/", StringComparison.CurrentCultureIgnoreCase))
		{
			Debug.LogWarning(string.Concat("LoadPrefabs - should start with assets/ - ", folder));
		}
		return this.LoadAll<GameObject>(folder, ".prefab");
	}
}