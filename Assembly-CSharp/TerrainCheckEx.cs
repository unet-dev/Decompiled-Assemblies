using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class TerrainCheckEx
{
	public static bool ApplyTerrainChecks(this Transform transform, TerrainCheck[] anchors, Vector3 pos, Quaternion rot, Vector3 scale, SpawnFilter filter = null)
	{
		if (anchors.Length == 0)
		{
			return true;
		}
		for (int i = 0; i < (int)anchors.Length; i++)
		{
			TerrainCheck terrainCheck = anchors[i];
			Vector3 vector3 = Vector3.Scale(terrainCheck.worldPosition, scale);
			if (terrainCheck.Rotate)
			{
				vector3 = rot * vector3;
			}
			Vector3 vector31 = pos + vector3;
			if (TerrainMeta.OutOfBounds(vector31))
			{
				return false;
			}
			if (filter != null && filter.GetFactor(vector31) == 0f)
			{
				return false;
			}
			if (!terrainCheck.Check(vector31))
			{
				return false;
			}
		}
		return true;
	}
}