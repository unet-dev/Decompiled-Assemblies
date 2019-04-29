using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class AssetNameCache
{
	private static Dictionary<UnityEngine.Object, string> mixed;

	private static Dictionary<UnityEngine.Object, string> lower;

	private static Dictionary<UnityEngine.Object, string> upper;

	static AssetNameCache()
	{
		AssetNameCache.mixed = new Dictionary<UnityEngine.Object, string>();
		AssetNameCache.lower = new Dictionary<UnityEngine.Object, string>();
		AssetNameCache.upper = new Dictionary<UnityEngine.Object, string>();
	}

	public static string GetName(this PhysicMaterial mat)
	{
		return AssetNameCache.LookupName(mat);
	}

	public static string GetName(this Material mat)
	{
		return AssetNameCache.LookupName(mat);
	}

	public static string GetNameLower(this PhysicMaterial mat)
	{
		return AssetNameCache.LookupNameLower(mat);
	}

	public static string GetNameLower(this Material mat)
	{
		return AssetNameCache.LookupNameLower(mat);
	}

	public static string GetNameUpper(this PhysicMaterial mat)
	{
		return AssetNameCache.LookupNameUpper(mat);
	}

	public static string GetNameUpper(this Material mat)
	{
		return AssetNameCache.LookupNameUpper(mat);
	}

	private static string LookupName(UnityEngine.Object obj)
	{
		string str;
		if (obj == null)
		{
			return string.Empty;
		}
		if (!AssetNameCache.mixed.TryGetValue(obj, out str))
		{
			str = obj.name;
			AssetNameCache.mixed.Add(obj, str);
		}
		return str;
	}

	private static string LookupNameLower(UnityEngine.Object obj)
	{
		string lower;
		if (obj == null)
		{
			return string.Empty;
		}
		if (!AssetNameCache.lower.TryGetValue(obj, out lower))
		{
			lower = obj.name.ToLower();
			AssetNameCache.lower.Add(obj, lower);
		}
		return lower;
	}

	private static string LookupNameUpper(UnityEngine.Object obj)
	{
		string upper;
		if (obj == null)
		{
			return string.Empty;
		}
		if (!AssetNameCache.upper.TryGetValue(obj, out upper))
		{
			upper = obj.name.ToUpper();
			AssetNameCache.upper.Add(obj, upper);
		}
		return upper;
	}
}