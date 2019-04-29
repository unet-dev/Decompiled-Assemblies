using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Apex.Utilities
{
	public static class ComponentHelper
	{
		public static IEnumerable<T> FindAllComponentsInScene<T>()
		where T : Component
		{
			return UnityEngine.Object.FindObjectsOfType(typeof(T)).Cast<T>();
		}

		public static T FindFirstComponentInScene<T>()
		where T : Component
		{
			UnityEngine.Object[] objArray = UnityEngine.Object.FindObjectsOfType(typeof(T));
			if (objArray == null || objArray.Length == 0)
			{
				return default(T);
			}
			return (T)(objArray[0] as T);
		}
	}
}