using Facepunch;
using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public static class ComponentEx
	{
		public static T Instantiate<T>(this T component)
		where T : Component
		{
			return Instantiate.GameObject(component.gameObject, null).GetComponent<T>();
		}
	}
}