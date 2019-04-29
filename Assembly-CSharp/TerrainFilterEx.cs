using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class TerrainFilterEx
{
	public static bool ApplyTerrainFilters(this Transform transform, TerrainFilter[] filters, Vector3 pos, Quaternion rot, Vector3 scale, SpawnFilter globalFilter = null)
	{
		if (filters.Length == 0)
		{
			return true;
		}
		for (int i = 0; i < (int)filters.Length; i++)
		{
			TerrainFilter terrainFilter = filters[i];
			Vector3 vector3 = Vector3.Scale(terrainFilter.worldPosition, scale);
			vector3 = rot * vector3;
			Vector3 vector31 = pos + vector3;
			if (TerrainMeta.OutOfBounds(vector31))
			{
				return false;
			}
			if (globalFilter != null && globalFilter.GetFactor(vector31) == 0f)
			{
				return false;
			}
			if (!terrainFilter.Check(vector31))
			{
				return false;
			}
		}
		return true;
	}
}