using System;
using UnityEngine;

public class SocketMod_SphereCheck : SocketMod
{
	public float sphereRadius = 1f;

	public LayerMask layerMask;

	public bool wantsCollide;

	public SocketMod_SphereCheck()
	{
	}

	public override bool DoCheck(Construction.Placement place)
	{
		Vector3 vector3 = place.position + (place.rotation * this.worldPosition);
		if (this.wantsCollide == GamePhysics.CheckSphere(vector3, this.sphereRadius, this.layerMask.@value, QueryTriggerInteraction.UseGlobal))
		{
			return true;
		}
		Construction.lastPlacementError = string.Concat("Failed Check: Sphere Test (", this.hierachyName, ")");
		return false;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = (this.wantsCollide ? new Color(0f, 1f, 0f, 0.7f) : new Color(1f, 0f, 0f, 0.7f));
		Gizmos.DrawSphere(Vector3.zero, this.sphereRadius);
	}
}