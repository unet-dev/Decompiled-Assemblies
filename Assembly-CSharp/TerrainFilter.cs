using System;
using UnityEngine;

public class TerrainFilter : PrefabAttribute
{
	public SpawnFilter Filter;

	public TerrainFilter()
	{
	}

	public bool Check(Vector3 pos)
	{
		return this.Filter.GetFactor(pos) > 0f;
	}

	protected override Type GetIndexedType()
	{
		return typeof(TerrainFilter);
	}

	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 1f);
		Gizmos.DrawCube(base.transform.position + ((Vector3.up * 50f) * 0.5f), new Vector3(0.5f, 50f, 0.5f));
		Gizmos.DrawSphere(base.transform.position + (Vector3.up * 50f), 2f);
	}
}