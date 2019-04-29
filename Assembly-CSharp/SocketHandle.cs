using System;
using UnityEngine;

public class SocketHandle : PrefabAttribute
{
	public SocketHandle()
	{
	}

	internal void AdjustTarget(ref Construction.Target target, float maxplaceDistance)
	{
		Vector3 vector3 = this.worldPosition;
		Vector3 vector31 = (target.ray.origin + (target.ray.direction * maxplaceDistance)) - vector3;
		Vector3 vector32 = vector31 - target.ray.origin;
		target.ray.direction = vector32.normalized;
	}

	protected override Type GetIndexedType()
	{
		return typeof(SocketHandle);
	}
}