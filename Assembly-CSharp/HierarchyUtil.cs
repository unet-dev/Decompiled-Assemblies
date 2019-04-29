using System;
using System.Collections.Generic;
using UnityEngine;

public static class HierarchyUtil
{
	public static Dictionary<string, GameObject> rootDict;

	static HierarchyUtil()
	{
		HierarchyUtil.rootDict = new Dictionary<string, GameObject>(StringComparer.OrdinalIgnoreCase);
	}

	public static GameObject GetRoot(string strName, bool groupActive = true, bool persistant = false)
	{
		GameObject gameObject;
		if (HierarchyUtil.rootDict.TryGetValue(strName, out gameObject))
		{
			if (gameObject != null)
			{
				return gameObject;
			}
			HierarchyUtil.rootDict.Remove(strName);
		}
		gameObject = new GameObject(strName);
		gameObject.SetActive(groupActive);
		HierarchyUtil.rootDict.Add(strName, gameObject);
		if (persistant)
		{
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}
		return gameObject;
	}
}