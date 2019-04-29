using System;
using UnityEngine;

public class SocketMod_InWater : SocketMod
{
	public bool wantsInWater = true;

	public SocketMod_InWater()
	{
	}

	public override bool DoCheck(Construction.Placement place)
	{
		if (WaterLevel.Test(place.position + (place.rotation * this.worldPosition)) == this.wantsInWater)
		{
			return true;
		}
		Construction.lastPlacementError = string.Concat("Failed Check: InWater (", this.hierachyName, ")");
		return false;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.cyan;
		Gizmos.DrawSphere(Vector3.zero, 0.1f);
	}
}