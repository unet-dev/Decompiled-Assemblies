using System;
using UnityEngine;

public class Socket_Free : Socket_Base
{
	public Vector3 idealPlacementNormal = Vector3.up;

	public bool useTargetNormal = true;

	public Socket_Free()
	{
	}

	public override Construction.Placement DoPlacement(Construction.Target target)
	{
		Vector3 vector3;
		Quaternion quaternion = Quaternion.identity;
		if (!this.useTargetNormal)
		{
			vector3 = target.position - target.ray.origin;
			Vector3 vector31 = vector3.normalized;
			vector31.y = 0f;
			quaternion = Quaternion.LookRotation(vector31, this.idealPlacementNormal) * Quaternion.Euler(target.rotation);
		}
		else
		{
			vector3 = target.position - target.ray.origin;
			Vector3 vector32 = vector3.normalized;
			float single = Mathf.Abs(Vector3.Dot(vector32, target.normal));
			vector32 = Vector3.Lerp(vector32, this.idealPlacementNormal, single);
			quaternion = (Quaternion.LookRotation(target.normal, vector32) * Quaternion.Inverse(this.rotation)) * Quaternion.Euler(target.rotation);
		}
		Vector3 vector33 = target.position;
		vector33 = vector33 - (quaternion * this.position);
		return new Construction.Placement()
		{
			rotation = quaternion,
			position = vector33
		};
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.green;
		Gizmos.DrawLine(Vector3.zero, Vector3.forward * 1f);
		GizmosUtil.DrawWireCircleZ(Vector3.forward * 0f, 0.2f);
		Gizmos.DrawIcon(base.transform.position, "light_circle_green.png", false);
	}

	public override bool TestTarget(Construction.Target target)
	{
		return target.onTerrain;
	}
}