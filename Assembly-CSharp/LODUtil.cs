using System;
using UnityEngine;

public static class LODUtil
{
	public static float GetDistance(Transform transform, LODDistanceMode mode = 0)
	{
		return LODUtil.GetDistance(transform.position, mode);
	}

	public static float GetDistance(Vector3 worldPos, LODDistanceMode mode = 0)
	{
		if (!MainCamera.isValid)
		{
			return 1000f;
		}
		if (mode != LODDistanceMode.XYZ)
		{
			return Vector3Ex.Distance2D(MainCamera.position, worldPos);
		}
		return Vector3.Distance(MainCamera.position, worldPos);
	}

	public static float VerifyDistance(float distance)
	{
		return Mathf.Min(500f, distance);
	}
}