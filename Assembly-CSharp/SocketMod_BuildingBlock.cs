using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SocketMod_BuildingBlock : SocketMod
{
	public float sphereRadius = 1f;

	public LayerMask layerMask;

	public QueryTriggerInteraction queryTriggers;

	public bool wantsCollide;

	public SocketMod_BuildingBlock()
	{
	}

	public override bool DoCheck(Construction.Placement place)
	{
		Vector3 vector3 = place.position + (place.rotation * this.worldPosition);
		List<BuildingBlock> list = Pool.GetList<BuildingBlock>();
		Vis.Entities<BuildingBlock>(vector3, this.sphereRadius, list, this.layerMask.@value, this.queryTriggers);
		bool count = list.Count > 0;
		if (count && this.wantsCollide)
		{
			Pool.FreeList<BuildingBlock>(ref list);
			return true;
		}
		if (count && !this.wantsCollide)
		{
			Pool.FreeList<BuildingBlock>(ref list);
			return false;
		}
		Pool.FreeList<BuildingBlock>(ref list);
		return !this.wantsCollide;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = (this.wantsCollide ? new Color(0f, 1f, 0f, 0.7f) : new Color(1f, 0f, 0f, 0.7f));
		Gizmos.DrawSphere(Vector3.zero, this.sphereRadius);
	}
}