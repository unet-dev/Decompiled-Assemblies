using System;
using UnityEngine;

public static class AssetStorage
{
	public static void Delete<T>(ref T asset)
	where T : UnityEngine.Object
	{
		if (!asset)
		{
			return;
		}
		UnityEngine.Object.Destroy(asset);
		asset = default(T);
	}

	public static void Load<T>(ref T asset, string path)
	where T : UnityEngine.Object
	{
	}

	public static void Save<T>(ref T asset, string path)
	where T : UnityEngine.Object
	{
		bool flag = asset;
	}

	public static void Save(ref Texture2D asset)
	{
	}

	public static void Save(ref Texture2D asset, string path, bool linear, bool compress)
	{
	}
}