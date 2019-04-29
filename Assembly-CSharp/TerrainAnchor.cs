using System;
using UnityEngine;

public class TerrainAnchor : PrefabAttribute
{
	public float Extents = 1f;

	public float Offset;

	public TerrainAnchor()
	{
	}

	public void Apply(out float height, out float min, out float max, Vector3 pos)
	{
		float extents = this.Extents;
		float offset = this.Offset;
		height = TerrainMeta.HeightMap.GetHeight(pos);
		min = height - offset - extents;
		max = height - offset + extents;
	}

	protected override Type GetIndexedType()
	{
		return typeof(TerrainAnchor);
	}

	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 1f);
		Gizmos.DrawLine((base.transform.position + (Vector3.up * this.Offset)) - (Vector3.up * this.Extents), (base.transform.position + (Vector3.up * this.Offset)) + (Vector3.up * this.Extents));
	}
}