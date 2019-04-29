using Rust;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Facepunch
{
	public class GameObjectPool<T> : ObjectPool<T>
	where T : Component
	{
		private GameObject poolRoot;

		public GameObjectPool()
		{
		}

		public void AddChildrenToPool(Transform t)
		{
			T[] array = (
				from Transform  in t
				select x.GetComponent<T>() into 
				where x != null
				select ).ToArray<T>();
			for (int i = 0; i < (int)array.Length; i++)
			{
				T t1 = array[i];
				if (!t1.CompareTag("persist"))
				{
					this.AddToPool(t1);
				}
			}
		}

		public override void AddToPool(T t)
		{
			if (Rust.Application.isQuitting)
			{
				return;
			}
			if (this.poolRoot == null)
			{
				this.poolRoot = new GameObject(string.Concat("GameObjectPool - ", typeof(T).Name));
				this.poolRoot.SetActive(false);
				UnityEngine.Object.DontDestroyOnLoad(this.poolRoot);
			}
			base.AddToPool(t);
			t.gameObject.SetActive(false);
			t.transform.SetParent(this.poolRoot.transform, false);
		}

		public T TakeOrInstantiate(GameObject prefabSource)
		{
			T t = base.TakeFromPool();
			if (t != null)
			{
				t.gameObject.SetActive(true);
				return t;
			}
			GameObject gameObject = Instantiate.GameObject(prefabSource, null);
			Debug.Assert(gameObject != null, "GameObjectPool - passed prefab didn't have a valid component!");
			gameObject.transform.SetParent(null, false);
			gameObject.SetActive(true);
			return gameObject.GetComponent<T>();
		}
	}
}