using System;
using UnityEngine;

public class DirectionProperties : PrefabAttribute
{
	private const float radius = 200f;

	public Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

	public ProtectionProperties extraProtection;

	public DirectionProperties()
	{
	}

	protected override Type GetIndexedType()
	{
		return typeof(DirectionProperties);
	}

	public bool IsWeakspot(Transform tx, HitInfo info)
	{
		if (this.bounds.size == Vector3.zero)
		{
			return false;
		}
		Vector3 vector3 = tx.worldToLocalMatrix.MultiplyPoint3x4(info.PointStart);
		float single = this.worldForward.DotDegrees(vector3);
		OBB oBB = new OBB(tx.position + (tx.rotation * ((this.worldRotation * this.bounds.center) + this.worldPosition)), this.bounds.size, tx.rotation * this.worldRotation);
		if (single <= 100f)
		{
			return false;
		}
		return oBB.Contains(info.HitPositionWorld);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		GizmosUtil.DrawSemiCircle(200f);
		Gizmos.DrawWireCube(this.bounds.center, this.bounds.size);
	}
}