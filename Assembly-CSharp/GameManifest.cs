using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GameManifest : ScriptableObject
{
	public GameManifest.PooledString[] pooledStrings;

	public GameManifest.MeshColliderInfo[] meshColliders;

	public GameManifest.PrefabProperties[] prefabProperties;

	public GameManifest.EffectCategory[] effectCategories;

	public string[] skinnables;

	public string[] entities;

	internal static GameManifest loadedManifest;

	public static Dictionary<string, string> guidToPath;

	public static Dictionary<string, UnityEngine.Object> guidToObject;

	public static GameManifest Current
	{
		get
		{
			if (GameManifest.loadedManifest != null)
			{
				return GameManifest.loadedManifest;
			}
			GameManifest.Load();
			return GameManifest.loadedManifest;
		}
	}

	static GameManifest()
	{
		GameManifest.guidToPath = new Dictionary<string, string>();
		GameManifest.guidToObject = new Dictionary<string, UnityEngine.Object>();
	}

	public GameManifest()
	{
	}

	private static string GetAssetStatus()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (GameManifest.loadedManifest == null)
		{
			stringBuilder.Append("Manifest Assets Missing");
		}
		else
		{
			stringBuilder.Append("Manifest Assets Loaded");
			stringBuilder.AppendLine();
			stringBuilder.Append("\t");
			stringBuilder.Append((Skinnable.All != null ? ((int)Skinnable.All.Length).ToString() : "0"));
			stringBuilder.Append(" skinnable objects");
		}
		return stringBuilder.ToString();
	}

	private static string GetMetadataStatus()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (GameManifest.loadedManifest == null)
		{
			stringBuilder.Append("Manifest Metadata Missing");
		}
		else
		{
			stringBuilder.Append("Manifest Metadata Loaded");
			stringBuilder.AppendLine();
			stringBuilder.Append("\t");
			int length = (int)GameManifest.loadedManifest.pooledStrings.Length;
			stringBuilder.Append(length.ToString());
			stringBuilder.Append(" pooled strings");
			stringBuilder.AppendLine();
			stringBuilder.Append("\t");
			length = (int)GameManifest.loadedManifest.meshColliders.Length;
			stringBuilder.Append(length.ToString());
			stringBuilder.Append(" mesh colliders");
			stringBuilder.AppendLine();
			stringBuilder.Append("\t");
			length = (int)GameManifest.loadedManifest.prefabProperties.Length;
			stringBuilder.Append(length.ToString());
			stringBuilder.Append(" prefab properties");
			stringBuilder.AppendLine();
			stringBuilder.Append("\t");
			length = (int)GameManifest.loadedManifest.effectCategories.Length;
			stringBuilder.Append(length.ToString());
			stringBuilder.Append(" effect categories");
			stringBuilder.AppendLine();
			stringBuilder.Append("\t");
			length = (int)GameManifest.loadedManifest.entities.Length;
			stringBuilder.Append(length.ToString());
			stringBuilder.Append(" entity names");
			stringBuilder.AppendLine();
			stringBuilder.Append("\t");
			length = (int)GameManifest.loadedManifest.skinnables.Length;
			stringBuilder.Append(length.ToString());
			stringBuilder.Append(" skinnable names");
		}
		return stringBuilder.ToString();
	}

	internal static UnityEngine.Object GUIDToObject(string guid)
	{
		UnityEngine.Object obj = null;
		if (GameManifest.guidToObject.TryGetValue(guid, out obj))
		{
			return obj;
		}
		string path = GameManifest.GUIDToPath(guid);
		if (string.IsNullOrEmpty(path))
		{
			Debug.LogWarning(string.Concat("Missing file for guid ", guid));
			GameManifest.guidToObject.Add(guid, null);
			return null;
		}
		UnityEngine.Object obj1 = FileSystem.Load<UnityEngine.Object>(path, true);
		GameManifest.guidToObject.Add(guid, obj1);
		return obj1;
	}

	internal static string GUIDToPath(string guid)
	{
		string str;
		if (string.IsNullOrEmpty(guid))
		{
			Debug.LogError("GUIDToPath: guid is empty");
			return string.Empty;
		}
		GameManifest.Load();
		if (GameManifest.guidToPath.TryGetValue(guid, out str))
		{
			return str;
		}
		Debug.LogWarning(string.Concat("GUIDToPath: no path found for guid ", guid));
		return string.Empty;
	}

	public static void Load()
	{
		if (GameManifest.loadedManifest != null)
		{
			return;
		}
		GameManifest.loadedManifest = FileSystem.Load<GameManifest>("Assets/manifest.asset", true);
		GameManifest.PrefabProperties[] prefabPropertiesArray = GameManifest.loadedManifest.prefabProperties;
		for (int i = 0; i < (int)prefabPropertiesArray.Length; i++)
		{
			GameManifest.PrefabProperties prefabProperty = prefabPropertiesArray[i];
			GameManifest.guidToPath.Add(prefabProperty.guid, prefabProperty.name);
		}
		DebugEx.Log(GameManifest.GetMetadataStatus(), StackTraceLogType.None);
	}

	public static void LoadAssets()
	{
		if (Skinnable.All != null)
		{
			return;
		}
		Skinnable.All = GameManifest.LoadSkinnableAssets();
		DebugEx.Log(GameManifest.GetAssetStatus(), StackTraceLogType.None);
	}

	internal static Dictionary<string, string[]> LoadEffectDictionary()
	{
		GameManifest.EffectCategory[] effectCategoryArray = GameManifest.loadedManifest.effectCategories;
		Dictionary<string, string[]> strs = new Dictionary<string, string[]>();
		GameManifest.EffectCategory[] effectCategoryArray1 = effectCategoryArray;
		for (int i = 0; i < (int)effectCategoryArray1.Length; i++)
		{
			GameManifest.EffectCategory effectCategory = effectCategoryArray1[i];
			strs.Add(effectCategory.folder, effectCategory.prefabs.ToArray());
		}
		return strs;
	}

	private static Skinnable[] LoadSkinnableAssets()
	{
		string[] current = GameManifest.Current.skinnables;
		Skinnable[] skinnableArray = new Skinnable[(int)current.Length];
		for (int i = 0; i < (int)current.Length; i++)
		{
			skinnableArray[i] = FileSystem.Load<Skinnable>(current[i], true);
		}
		return skinnableArray;
	}

	[Serializable]
	public class EffectCategory
	{
		[HideInInspector]
		public string folder;

		public List<string> prefabs;

		public EffectCategory()
		{
		}
	}

	[Serializable]
	public class MeshColliderInfo
	{
		[HideInInspector]
		public string name;

		public uint hash;

		public PhysicMaterial physicMaterial;

		public MeshColliderInfo()
		{
		}
	}

	[Serializable]
	public struct PooledString
	{
		[HideInInspector]
		public string str;

		public uint hash;
	}

	[Serializable]
	public class PrefabProperties
	{
		[HideInInspector]
		public string name;

		public string guid;

		public uint hash;

		public bool pool;

		public PrefabProperties()
		{
		}
	}
}