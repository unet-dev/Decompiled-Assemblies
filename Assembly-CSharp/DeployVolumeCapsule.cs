using System;
using UnityEngine;

public class DeployVolumeCapsule : DeployVolume
{
	public Vector3 center = Vector3.zero;

	public float radius = 0.5f;

	public float height = 1f;

	public DeployVolumeCapsule()
	{
	}

	protected override bool Check(Vector3 position, Quaternion rotation, int mask = -1)
	{
		position = position + (rotation * ((this.worldRotation * this.center) + this.worldPosition));
		if (DeployVolume.CheckCapsule(position + ((((rotation * this.worldRotation) * Vector3.up) * this.height) * 0.5f), position + ((((rotation * this.worldRotation) * Vector3.down) * this.height) * 0.5f), this.radius, this.layers & mask, this.ignore))
		{
			return true;
		}
		return false;
	}

	protected override bool Check(Vector3 position, Quaternion rotation, OBB obb, int mask = -1)
	{
		return false;
	}
}