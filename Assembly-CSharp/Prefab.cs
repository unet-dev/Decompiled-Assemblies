using System;
using System.Collections.Generic;
using UnityEngine;

public class Prefab : IComparable<Prefab>
{
	public uint ID;

	public string Name;

	public GameObject Object;

	public GameManager Manager;

	public PrefabAttribute.Library Attribute;

	public PrefabParameters Parameters;

	public static PrefabAttribute.Library DefaultAttribute
	{
		get
		{
			return PrefabAttribute.server;
		}
	}

	public static GameManager DefaultManager
	{
		get
		{
			return GameManager.server;
		}
	}

	public Prefab(string name, GameObject prefab, GameManager manager, PrefabAttribute.Library attribute)
	{
		this.ID = StringPool.Get(name);
		this.Name = name;
		this.Object = prefab;
		this.Manager = manager;
		this.Attribute = attribute;
		this.Parameters = prefab.GetComponent<PrefabParameters>();
	}

	public void ApplyDecorComponents(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		DecorComponent[] decorComponentArray = this.Attribute.FindAll<DecorComponent>(this.ID);
		this.Object.transform.ApplyDecorComponents(decorComponentArray, ref pos, ref rot, ref scale);
	}

	public bool ApplyTerrainAnchors(ref Vector3 pos, Quaternion rot, Vector3 scale, TerrainAnchorMode mode, SpawnFilter filter = null)
	{
		TerrainAnchor[] terrainAnchorArray = this.Attribute.FindAll<TerrainAnchor>(this.ID);
		return this.Object.transform.ApplyTerrainAnchors(terrainAnchorArray, ref pos, rot, scale, mode, filter);
	}

	public bool ApplyTerrainAnchors(ref Vector3 pos, Quaternion rot, Vector3 scale, SpawnFilter filter = null)
	{
		TerrainAnchor[] terrainAnchorArray = this.Attribute.FindAll<TerrainAnchor>(this.ID);
		return this.Object.transform.ApplyTerrainAnchors(terrainAnchorArray, ref pos, rot, scale, filter);
	}

	public bool ApplyTerrainChecks(Vector3 pos, Quaternion rot, Vector3 scale, SpawnFilter filter = null)
	{
		TerrainCheck[] terrainCheckArray = this.Attribute.FindAll<TerrainCheck>(this.ID);
		return this.Object.transform.ApplyTerrainChecks(terrainCheckArray, pos, rot, scale, filter);
	}

	public bool ApplyTerrainFilters(Vector3 pos, Quaternion rot, Vector3 scale, SpawnFilter filter = null)
	{
		TerrainFilter[] terrainFilterArray = this.Attribute.FindAll<TerrainFilter>(this.ID);
		return this.Object.transform.ApplyTerrainFilters(terrainFilterArray, pos, rot, scale, filter);
	}

	public void ApplyTerrainModifiers(Vector3 pos, Quaternion rot, Vector3 scale)
	{
		TerrainModifier[] terrainModifierArray = this.Attribute.FindAll<TerrainModifier>(this.ID);
		this.Object.transform.ApplyTerrainModifiers(terrainModifierArray, pos, rot, scale);
	}

	public void ApplyTerrainPlacements(Vector3 pos, Quaternion rot, Vector3 scale)
	{
		TerrainPlacement[] terrainPlacementArray = this.Attribute.FindAll<TerrainPlacement>(this.ID);
		this.Object.transform.ApplyTerrainPlacements(terrainPlacementArray, pos, rot, scale);
	}

	public bool ApplyWaterChecks(Vector3 pos, Quaternion rot, Vector3 scale)
	{
		WaterCheck[] waterCheckArray = this.Attribute.FindAll<WaterCheck>(this.ID);
		return this.Object.transform.ApplyWaterChecks(waterCheckArray, pos, rot, scale);
	}

	public bool CheckEnvironmentVolumes(Vector3 pos, Quaternion rot, Vector3 scale, EnvironmentType type)
	{
		return this.Object.transform.CheckEnvironmentVolumes(pos, rot, scale, type);
	}

	public int CompareTo(Prefab that)
	{
		if (that == null)
		{
			return 1;
		}
		PrefabPriority prefabPriority = (this.Parameters != null ? this.Parameters.Priority : PrefabPriority.Default);
		return ((that.Parameters != null ? that.Parameters.Priority : PrefabPriority.Default)).CompareTo(prefabPriority);
	}

	private static string[] FindPrefabNames(string strPrefab, bool useProbabilities = false)
	{
		strPrefab = strPrefab.TrimEnd(new char[] { '/' }).ToLower();
		GameObject[] gameObjectArray = FileSystem.LoadPrefabs(string.Concat(strPrefab, "/"));
		List<string> strs = new List<string>((int)gameObjectArray.Length);
		GameObject[] gameObjectArray1 = gameObjectArray;
		for (int i = 0; i < (int)gameObjectArray1.Length; i++)
		{
			GameObject gameObject = gameObjectArray1[i];
			string str = string.Concat(strPrefab, "/", gameObject.name.ToLower(), ".prefab");
			if (useProbabilities)
			{
				PrefabParameters component = gameObject.GetComponent<PrefabParameters>();
				int num = (component ? component.Count : 1);
				for (int j = 0; j < num; j++)
				{
					strs.Add(str);
				}
			}
			else
			{
				strs.Add(str);
			}
		}
		strs.Sort();
		return strs.ToArray();
	}

	public static Prefab<T> Load<T>(uint id, GameManager manager = null, PrefabAttribute.Library attribute = null)
	where T : Component
	{
		if (manager == null)
		{
			manager = Prefab.DefaultManager;
		}
		if (attribute == null)
		{
			attribute = Prefab.DefaultAttribute;
		}
		string str = StringPool.Get(id);
		GameObject gameObject = manager.FindPrefab(str);
		return new Prefab<T>(str, gameObject, gameObject.GetComponent<T>(), manager, attribute);
	}

	public static Prefab[] Load(string folder, GameManager manager = null, PrefabAttribute.Library attribute = null, bool useProbabilities = true)
	{
		if (string.IsNullOrEmpty(folder))
		{
			return null;
		}
		if (manager == null)
		{
			manager = Prefab.DefaultManager;
		}
		if (attribute == null)
		{
			attribute = Prefab.DefaultAttribute;
		}
		string[] strArrays = Prefab.FindPrefabNames(folder, useProbabilities);
		Prefab[] prefab = new Prefab[(int)strArrays.Length];
		for (int i = 0; i < (int)prefab.Length; i++)
		{
			string str = strArrays[i];
			GameObject gameObject = manager.FindPrefab(str);
			prefab[i] = new Prefab(str, gameObject, manager, attribute);
		}
		return prefab;
	}

	public static Prefab<T>[] Load<T>(string folder, GameManager manager = null, PrefabAttribute.Library attribute = null, bool useProbabilities = true)
	where T : Component
	{
		if (string.IsNullOrEmpty(folder))
		{
			return null;
		}
		return Prefab.Load<T>(Prefab.FindPrefabNames(folder, useProbabilities), manager, attribute);
	}

	public static Prefab<T>[] Load<T>(string[] names, GameManager manager = null, PrefabAttribute.Library attribute = null)
	where T : Component
	{
		if (manager == null)
		{
			manager = Prefab.DefaultManager;
		}
		if (attribute == null)
		{
			attribute = Prefab.DefaultAttribute;
		}
		Prefab<T>[] prefab = new Prefab<T>[(int)names.Length];
		for (int i = 0; i < (int)prefab.Length; i++)
		{
			string str = names[i];
			GameObject gameObject = manager.FindPrefab(str);
			T component = gameObject.GetComponent<T>();
			prefab[i] = new Prefab<T>(str, gameObject, component, manager, attribute);
		}
		return prefab;
	}

	public static Prefab LoadRandom(string folder, ref uint seed, GameManager manager = null, PrefabAttribute.Library attribute = null, bool useProbabilities = true)
	{
		if (string.IsNullOrEmpty(folder))
		{
			return null;
		}
		if (manager == null)
		{
			manager = Prefab.DefaultManager;
		}
		if (attribute == null)
		{
			attribute = Prefab.DefaultAttribute;
		}
		string[] strArrays = Prefab.FindPrefabNames(folder, useProbabilities);
		if (strArrays.Length == 0)
		{
			return null;
		}
		string str = strArrays[SeedRandom.Range(ref seed, 0, (int)strArrays.Length)];
		return new Prefab(str, manager.FindPrefab(str), manager, attribute);
	}

	public static Prefab<T> LoadRandom<T>(string folder, ref uint seed, GameManager manager = null, PrefabAttribute.Library attribute = null, bool useProbabilities = true)
	where T : Component
	{
		if (string.IsNullOrEmpty(folder))
		{
			return null;
		}
		if (manager == null)
		{
			manager = Prefab.DefaultManager;
		}
		if (attribute == null)
		{
			attribute = Prefab.DefaultAttribute;
		}
		string[] strArrays = Prefab.FindPrefabNames(folder, useProbabilities);
		if (strArrays.Length == 0)
		{
			return null;
		}
		string str = strArrays[SeedRandom.Range(ref seed, 0, (int)strArrays.Length)];
		GameObject gameObject = manager.FindPrefab(str);
		return new Prefab<T>(str, gameObject, gameObject.GetComponent<T>(), manager, attribute);
	}

	public static implicit operator GameObject(Prefab prefab)
	{
		return prefab.Object;
	}

	public GameObject Spawn(Transform transform)
	{
		return this.Manager.CreatePrefab(this.Name, transform, true);
	}

	public GameObject Spawn(Vector3 pos, Quaternion rot)
	{
		return this.Manager.CreatePrefab(this.Name, pos, rot, true);
	}

	public GameObject Spawn(Vector3 pos, Quaternion rot, Vector3 scale)
	{
		return this.Manager.CreatePrefab(this.Name, pos, rot, scale, true);
	}

	public BaseEntity SpawnEntity(Vector3 pos, Quaternion rot)
	{
		return this.Manager.CreateEntity(this.Name, pos, rot, true);
	}
}