using System;
using UnityEngine;

public class DeployVolumeSphere : DeployVolume
{
	public Vector3 center = Vector3.zero;

	public float radius = 0.5f;

	public DeployVolumeSphere()
	{
	}

	protected override bool Check(Vector3 position, Quaternion rotation, int mask = -1)
	{
		position = position + (rotation * ((this.worldRotation * this.center) + this.worldPosition));
		if (DeployVolume.CheckSphere(position, this.radius, this.layers & mask, this.ignore))
		{
			return true;
		}
		return false;
	}

	protected override bool Check(Vector3 position, Quaternion rotation, OBB obb, int mask = -1)
	{
		position = position + (rotation * ((this.worldRotation * this.center) + this.worldPosition));
		if ((this.layers & mask) != 0 && Vector3.Distance(position, obb.ClosestPoint(position)) <= this.radius)
		{
			return true;
		}
		return false;
	}
}