using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public static class RayEx
	{
		public static Vector3 ClosestPoint(this Ray ray, Vector3 pos)
		{
			return ray.origin + (Vector3.Dot(pos - ray.origin, ray.direction) * ray.direction);
		}

		public static float Distance(this Ray ray, Vector3 pos)
		{
			Vector3 vector3 = Vector3.Cross(ray.direction, pos - ray.origin);
			return vector3.magnitude;
		}

		public static bool IsNaNOrInfinity(this Ray r)
		{
			if (r.origin.IsNaNOrInfinity())
			{
				return true;
			}
			return r.direction.IsNaNOrInfinity();
		}

		public static float SqrDistance(this Ray ray, Vector3 pos)
		{
			Vector3 vector3 = Vector3.Cross(ray.direction, pos - ray.origin);
			return vector3.sqrMagnitude;
		}
	}
}