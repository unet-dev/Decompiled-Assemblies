using System;
using UnityEngine;

public class SocketMod_AngleCheck : SocketMod
{
	public bool wantsAngle = true;

	public Vector3 worldNormal = Vector3.up;

	public float withinDegrees = 45f;

	public SocketMod_AngleCheck()
	{
	}

	public override bool DoCheck(Construction.Placement place)
	{
		if (this.worldNormal.DotDegrees(place.rotation * Vector3.up) < this.withinDegrees)
		{
			return true;
		}
		Construction.lastPlacementError = string.Concat("Failed Check: AngleCheck (", this.hierachyName, ")");
		return false;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.yellow;
		Gizmos.DrawFrustum(Vector3.zero, this.withinDegrees, 1f, 0f, 1f);
	}
}