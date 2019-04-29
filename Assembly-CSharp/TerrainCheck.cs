using System;
using UnityEngine;

public class TerrainCheck : PrefabAttribute
{
	public bool Rotate = true;

	public float Extents = 1f;

	public TerrainCheck()
	{
	}

	public bool Check(Vector3 pos)
	{
		float extents = this.Extents;
		float height = TerrainMeta.HeightMap.GetHeight(pos);
		float single = pos.y + extents;
		if (pos.y - extents > height)
		{
			return false;
		}
		if (single < height)
		{
			return false;
		}
		return true;
	}

	protected override Type GetIndexedType()
	{
		return typeof(TerrainCheck);
	}

	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 1f);
		Gizmos.DrawLine(base.transform.position - (Vector3.up * this.Extents), base.transform.position + (Vector3.up * this.Extents));
	}
}