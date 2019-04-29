using System;
using UnityEngine;

namespace Rust.Ai
{
	public class NavmeshPrefabInstantiator : MonoBehaviour
	{
		public GameObjectRef NavmeshPrefab;

		public NavmeshPrefabInstantiator()
		{
		}

		private void Start()
		{
			if (this.NavmeshPrefab != null)
			{
				this.NavmeshPrefab.Instantiate(base.transform).SetActive(true);
				UnityEngine.Object.Destroy(this);
			}
		}
	}
}