using System;
using System.Collections.Generic;
using UnityEngine;

public class StringPool
{
	public static Dictionary<uint, string> toString;

	public static Dictionary<string, uint> toNumber;

	private static bool initialized;

	public static uint closest;

	static StringPool()
	{
	}

	public StringPool()
	{
	}

	public static uint Add(string str)
	{
		uint num = 0;
		if (!StringPool.toNumber.TryGetValue(str, out num))
		{
			num = str.ManifestHash();
			StringPool.toString.Add(num, str);
			StringPool.toNumber.Add(str, num);
		}
		return num;
	}

	public static string Get(uint i)
	{
		string str;
		if (i == 0)
		{
			return string.Empty;
		}
		StringPool.Init();
		if (StringPool.toString.TryGetValue(i, out str))
		{
			return str;
		}
		Debug.LogWarning(string.Concat("StringPool.GetString - no string for ID", i));
		return "";
	}

	public static uint Get(string str)
	{
		uint num;
		if (string.IsNullOrEmpty(str))
		{
			return (uint)0;
		}
		StringPool.Init();
		if (StringPool.toNumber.TryGetValue(str, out num))
		{
			return num;
		}
		Debug.LogWarning(string.Concat("StringPool.GetNumber - no number for string ", str));
		return (uint)0;
	}

	private static void Init()
	{
		if (StringPool.initialized)
		{
			return;
		}
		StringPool.toString = new Dictionary<uint, string>();
		StringPool.toNumber = new Dictionary<string, uint>(StringComparer.OrdinalIgnoreCase);
		GameManifest gameManifest = FileSystem.Load<GameManifest>("Assets/manifest.asset", true);
		for (uint i = 0; (ulong)i < (long)((int)gameManifest.pooledStrings.Length); i++)
		{
			StringPool.toString.Add(gameManifest.pooledStrings[i].hash, gameManifest.pooledStrings[i].str);
			StringPool.toNumber.Add(gameManifest.pooledStrings[i].str, gameManifest.pooledStrings[i].hash);
		}
		StringPool.initialized = true;
		StringPool.closest = StringPool.Get("closest");
	}
}