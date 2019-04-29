using System;
using UnityEngine;

public class SocketMod_HotSpot : SocketMod
{
	public float spotSize = 0.1f;

	public SocketMod_HotSpot()
	{
	}

	public override void ModifyPlacement(Construction.Placement place)
	{
		Vector3 vector3 = place.position + (place.rotation * this.worldPosition);
		place.position = vector3;
	}

	private void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
		Gizmos.DrawSphere(Vector3.zero, this.spotSize);
	}
}