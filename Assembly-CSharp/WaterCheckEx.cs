using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class WaterCheckEx
{
	public static bool ApplyWaterChecks(this Transform transform, WaterCheck[] anchors, Vector3 pos, Quaternion rot, Vector3 scale)
	{
		if (anchors.Length == 0)
		{
			return true;
		}
		for (int i = 0; i < (int)anchors.Length; i++)
		{
			WaterCheck waterCheck = anchors[i];
			Vector3 vector3 = Vector3.Scale(waterCheck.worldPosition, scale);
			if (waterCheck.Rotate)
			{
				vector3 = rot * vector3;
			}
			if (!waterCheck.Check(pos + vector3))
			{
				return false;
			}
		}
		return true;
	}
}