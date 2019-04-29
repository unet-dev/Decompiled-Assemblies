using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class AssetPool
{
	public static Dictionary<Type, AssetPool.Pool> storage;

	static AssetPool()
	{
		AssetPool.storage = new Dictionary<Type, AssetPool.Pool>();
	}

	public static void Clear()
	{
		foreach (KeyValuePair<Type, AssetPool.Pool> keyValuePair in AssetPool.storage)
		{
			keyValuePair.Value.Clear();
		}
	}

	public static void Free(ref Mesh mesh)
	{
		mesh.Clear();
		AssetPool.GetPool<Mesh>().Push<Mesh>(ref mesh);
	}

	public static T Get<T>()
	where T : UnityEngine.Object, new()
	{
		return AssetPool.GetPool<T>().Pop<T>();
	}

	private static AssetPool.Pool GetPool<T>()
	where T : UnityEngine.Object, new()
	{
		AssetPool.Pool pool;
		if (!AssetPool.storage.TryGetValue(typeof(T), out pool))
		{
			pool = new AssetPool.Pool(string.Concat("Pooled ", typeof(T).Name));
			AssetPool.storage.Add(typeof(T), pool);
		}
		return pool;
	}

	public class Pool
	{
		public Stack<UnityEngine.Object> stack;

		public int allocated;

		public int available;

		public string name;

		public Pool(string name)
		{
			this.name = name;
		}

		public void Clear()
		{
			foreach (UnityEngine.Object obj in this.stack)
			{
				UnityEngine.Object.Destroy(obj);
			}
			this.available -= this.stack.Count;
			this.allocated -= this.stack.Count;
			this.stack.Clear();
		}

		public T Pop<T>()
		where T : UnityEngine.Object, new()
		{
			if (this.stack.Count > 0)
			{
				this.available--;
				return (T)(this.stack.Pop() as T);
			}
			this.allocated++;
			T t = Activator.CreateInstance<T>();
			t.name = this.name;
			return t;
		}

		public void Push<T>(ref T instance)
		where T : UnityEngine.Object, new()
		{
			this.available++;
			this.stack.Push(instance);
			instance = default(T);
		}
	}
}