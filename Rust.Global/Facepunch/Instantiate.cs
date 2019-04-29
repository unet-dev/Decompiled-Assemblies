using System;
using UnityEngine;

namespace Facepunch
{
	public static class Instantiate
	{
		public static GameObject GameObject(GameObject go, Transform parent = null)
		{
			long num = Instantiate.WrapStart(go);
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(go, parent);
			Instantiate.WrapEnd(go, num);
			return gameObject;
		}

		public static GameObject GameObject(GameObject go, Vector3 pos, Quaternion rot)
		{
			long num = Instantiate.WrapStart(go);
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(go, pos, rot);
			Instantiate.WrapEnd(go, num);
			return gameObject;
		}

		public static void WrapEnd(GameObject go, long memory)
		{
		}

		public static long WrapStart(GameObject go)
		{
			return (long)0;
		}
	}
}