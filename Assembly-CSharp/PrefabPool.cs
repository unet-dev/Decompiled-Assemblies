using System;
using System.Collections.Generic;
using UnityEngine;

public class PrefabPool
{
	public Stack<Poolable> stack = new Stack<Poolable>();

	public int Count
	{
		get
		{
			return this.stack.Count;
		}
	}

	public PrefabPool()
	{
	}

	public void Clear()
	{
		foreach (Poolable poolable in this.stack)
		{
			if (!poolable)
			{
				continue;
			}
			UnityEngine.Object.Destroy(poolable);
		}
		this.stack.Clear();
	}

	public GameObject Pop(Vector3 pos = null, Quaternion rot = null)
	{
		while (this.stack.Count > 0)
		{
			Poolable poolable = this.stack.Pop();
			if (!poolable)
			{
				continue;
			}
			poolable.transform.position = pos;
			poolable.transform.rotation = rot;
			poolable.LeavePool();
			return poolable.gameObject;
		}
		return null;
	}

	public void Push(Poolable info)
	{
		this.stack.Push(info);
		info.EnterPool();
	}

	public void Push(GameObject instance)
	{
		this.Push(instance.GetComponent<Poolable>());
	}
}