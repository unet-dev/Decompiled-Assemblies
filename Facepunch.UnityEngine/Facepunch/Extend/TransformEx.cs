using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Facepunch.Extend
{
	public static class TransformEx
	{
		public static Transform FindChildRecursive(this Transform transform, string name)
		{
			Transform transforms = transform.Find(name);
			for (int i = 0; i < transform.childCount && transforms == null; i++)
			{
				transforms = transform.GetChild(i).FindChildRecursive(name);
			}
			return transforms;
		}

		public static T GetOrAddComponent<T>(this Transform transform)
		where T : Component
		{
			T component = transform.GetComponent<T>();
			if (component == null)
			{
				component = transform.gameObject.AddComponent<T>();
			}
			return component;
		}
	}
}