using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Facepunch
{
	public static class Vector3Extensions
	{
		public static Vector3 RemoveAxis(this Vector3 o, Vector3 axis)
		{
			Vector3 vector3 = axis.normalized;
			return o - (vector3 * Vector3.Dot(o, vector3));
		}

		public static Vector3 XZ(this Vector3 o, float y = 0f)
		{
			return new Vector3(o.x, y, o.z);
		}
	}
}