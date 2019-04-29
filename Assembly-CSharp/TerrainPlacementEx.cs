using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class TerrainPlacementEx
{
	public static void ApplyTerrainPlacements(this Transform transform, TerrainPlacement[] placements, Vector3 pos, Quaternion rot, Vector3 scale)
	{
		if (placements.Length == 0)
		{
			return;
		}
		Matrix4x4 matrix4x4 = Matrix4x4.TRS(pos, rot, scale);
		Matrix4x4 matrix4x41 = matrix4x4.inverse;
		for (int i = 0; i < (int)placements.Length; i++)
		{
			placements[i].Apply(matrix4x4, matrix4x41);
		}
	}

	public static void ApplyTerrainPlacements(this Transform transform, TerrainPlacement[] placements)
	{
		transform.ApplyTerrainPlacements(placements, transform.position, transform.rotation, transform.lossyScale);
	}
}