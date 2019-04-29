using System;
using System.Collections.Generic;
using UnityEngine;

public class PrefabPoolCollection
{
	public Dictionary<uint, PrefabPool> storage = new Dictionary<uint, PrefabPool>();

	public PrefabPoolCollection()
	{
	}

	public void Clear()
	{
		foreach (KeyValuePair<uint, PrefabPool> keyValuePair in this.storage)
		{
			keyValuePair.Value.Clear();
		}
		this.storage.Clear();
	}

	public GameObject Pop(uint id, Vector3 pos = null, Quaternion rot = null)
	{
		PrefabPool prefabPool;
		if (!this.storage.TryGetValue(id, out prefabPool))
		{
			return null;
		}
		return prefabPool.Pop(pos, rot);
	}

	public void Push(GameObject instance)
	{
		PrefabPool prefabPool;
		Poolable component = instance.GetComponent<Poolable>();
		if (!this.storage.TryGetValue(component.prefabID, out prefabPool))
		{
			prefabPool = new PrefabPool();
			this.storage.Add(component.prefabID, prefabPool);
		}
		prefabPool.Push(component);
	}
}