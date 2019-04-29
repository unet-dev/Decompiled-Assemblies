using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Facepunch.Extend
{
	public static class RayExtensions
	{
		public static float ClosestDistance(this Ray ray, Vector3 position)
		{
			return ray.ClosestPoint(position).magnitude;
		}

		public static Vector3 ClosestPoint(this Ray ray, Vector3 position)
		{
			Vector3 vector3 = ray.origin - position;
			return vector3 - (Vector3.Dot(vector3, ray.direction) * ray.direction);
		}
	}
}