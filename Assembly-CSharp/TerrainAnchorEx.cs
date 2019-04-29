using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class TerrainAnchorEx
{
	public static bool ApplyTerrainAnchors(this Transform transform, TerrainAnchor[] anchors, ref Vector3 pos, Quaternion rot, Vector3 scale, SpawnFilter filter = null)
	{
		return transform.ApplyTerrainAnchors(anchors, ref pos, rot, scale, TerrainAnchorMode.MinimizeError, filter);
	}

	public static bool ApplyTerrainAnchors(this Transform transform, TerrainAnchor[] anchors, ref Vector3 pos, Quaternion rot, Vector3 scale, TerrainAnchorMode mode, SpawnFilter filter = null)
	{
		float single;
		float single1;
		float single2;
		if (anchors.Length == 0)
		{
			return true;
		}
		float single3 = 0f;
		float single4 = Single.MinValue;
		float single5 = Single.MaxValue;
		for (int i = 0; i < (int)anchors.Length; i++)
		{
			TerrainAnchor terrainAnchor = anchors[i];
			Vector3 vector3 = Vector3.Scale(terrainAnchor.worldPosition, scale);
			vector3 = rot * vector3;
			Vector3 vector31 = pos + vector3;
			if (TerrainMeta.OutOfBounds(vector31))
			{
				return false;
			}
			if (filter != null && filter.GetFactor(vector31) == 0f)
			{
				return false;
			}
			terrainAnchor.Apply(out single, out single1, out single2, vector31);
			single3 = single3 + (single - vector3.y);
			single4 = Mathf.Max(single4, single1 - vector3.y);
			single5 = Mathf.Min(single5, single2 - vector3.y);
			if (single5 < single4)
			{
				return false;
			}
		}
		if (mode != TerrainAnchorMode.MinimizeError)
		{
			pos.y = Mathf.Clamp(pos.y, single4, single5);
		}
		else
		{
			pos.y = Mathf.Clamp(single3 / (float)((int)anchors.Length), single4, single5);
		}
		return true;
	}

	public static void ApplyTerrainAnchors(this Transform transform, TerrainAnchor[] anchors)
	{
		Vector3 vector3 = transform.position;
		transform.ApplyTerrainAnchors(anchors, ref vector3, transform.rotation, transform.lossyScale, null);
		transform.position = vector3;
	}
}