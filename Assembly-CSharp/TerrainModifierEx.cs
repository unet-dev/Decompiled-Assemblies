using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class TerrainModifierEx
{
	public static void ApplyTerrainModifiers(this Transform transform, TerrainModifier[] modifiers, Vector3 pos, Quaternion rot, Vector3 scale)
	{
		for (int i = 0; i < (int)modifiers.Length; i++)
		{
			TerrainModifier terrainModifier = modifiers[i];
			Vector3 vector3 = Vector3.Scale(terrainModifier.worldPosition, scale);
			Vector3 vector31 = pos + (rot * vector3);
			terrainModifier.Apply(vector31, scale.y);
		}
	}

	public static void ApplyTerrainModifiers(this Transform transform, TerrainModifier[] modifiers)
	{
		transform.ApplyTerrainModifiers(modifiers, transform.position, transform.rotation, transform.lossyScale);
	}
}