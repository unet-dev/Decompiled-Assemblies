using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public static class MathEx
	{
		public static bool QuadTest(this Ray ray, Vector3 planeCenter, Quaternion planeRot, Vector2 planeSize, out Vector3 hitPosition, float gridSize = 0f)
		{
			Plane plane = new Plane(planeRot * Vector3.forward, planeCenter);
			hitPosition = Vector3.zero;
			float single = 0f;
			if (!plane.Raycast(ray, out single))
			{
				return false;
			}
			hitPosition = ray.origin + (ray.direction * single);
			Vector3 vector3 = hitPosition - planeCenter;
			float single1 = Vector3.Dot(vector3, planeRot * Vector3.left);
			float single2 = Vector3.Dot(vector3, planeRot * Vector3.up);
			if (Mathf.Abs(single1) > planeSize.x / 2f)
			{
				single1 = (single1 < 0f ? -planeSize.x : planeSize.x) / 2f;
			}
			if (Mathf.Abs(single2) > planeSize.y / 2f)
			{
				single2 = (single2 < 0f ? -planeSize.y : planeSize.y) / 2f;
			}
			if (gridSize > 0f)
			{
				single1 = single1.SnapTo(gridSize);
				single2 = single2.SnapTo(gridSize);
			}
			hitPosition = planeCenter;
			hitPosition = hitPosition + ((planeRot * Vector3.left) * single1);
			hitPosition = hitPosition + ((planeRot * Vector3.up) * single2);
			return true;
		}

		public static float SnapTo(this float val, float snapValue)
		{
			if (snapValue == 0f)
			{
				return val;
			}
			return Mathf.Round(val / snapValue) * snapValue;
		}
	}
}