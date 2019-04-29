using System;
using UnityEngine;

public class SocketMod_AreaCheck : SocketMod
{
	public Bounds bounds = new Bounds(Vector3.zero, Vector3.one * 0.1f);

	public LayerMask layerMask;

	public bool wantsInside = true;

	public SocketMod_AreaCheck()
	{
	}

	public bool DoCheck(Vector3 position, Quaternion rotation)
	{
		Vector3 vector3 = position + (rotation * this.worldPosition);
		Quaternion quaternion = rotation * this.worldRotation;
		return SocketMod_AreaCheck.IsInArea(vector3, quaternion, this.bounds, this.layerMask) == this.wantsInside;
	}

	public override bool DoCheck(Construction.Placement place)
	{
		if (this.DoCheck(place.position, place.rotation))
		{
			return true;
		}
		Construction.lastPlacementError = string.Concat("Failed Check: IsInArea (", this.hierachyName, ")");
		return false;
	}

	public static bool IsInArea(Vector3 position, Quaternion rotation, Bounds bounds, LayerMask layerMask)
	{
		return GamePhysics.CheckOBB(new OBB(position, rotation, bounds), layerMask.@value, QueryTriggerInteraction.UseGlobal);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		bool flag = true;
		if (!this.wantsInside)
		{
			flag = !flag;
		}
		Gizmos.color = (flag ? Color.green : Color.red);
		Gizmos.DrawCube(this.bounds.center, this.bounds.size);
	}
}