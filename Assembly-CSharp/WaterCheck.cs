using System;
using UnityEngine;

public class WaterCheck : PrefabAttribute
{
	public bool Rotate = true;

	public WaterCheck()
	{
	}

	public bool Check(Vector3 pos)
	{
		return pos.y <= TerrainMeta.WaterMap.GetHeight(pos);
	}

	protected override Type GetIndexedType()
	{
		return typeof(WaterCheck);
	}

	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0f, 0f, 0.5f, 1f);
		Gizmos.DrawSphere(base.transform.position, 1f);
	}
}