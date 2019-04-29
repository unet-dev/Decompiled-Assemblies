using System;
using UnityEngine;

public class DeployVolumeOBB : DeployVolume
{
	public Bounds bounds = new Bounds(Vector3.zero, Vector3.one);

	public DeployVolumeOBB()
	{
	}

	protected override bool Check(Vector3 position, Quaternion rotation, int mask = -1)
	{
		position = position + (rotation * ((this.worldRotation * this.bounds.center) + this.worldPosition));
		if (DeployVolume.CheckOBB(new OBB(position, this.bounds.size, rotation * this.worldRotation), this.layers & mask, this.ignore))
		{
			return true;
		}
		return false;
	}

	protected override bool Check(Vector3 position, Quaternion rotation, OBB test, int mask = -1)
	{
		position = position + (rotation * ((this.worldRotation * this.bounds.center) + this.worldPosition));
		OBB oBB = new OBB(position, this.bounds.size, rotation * this.worldRotation);
		if ((this.layers & mask) != 0 && oBB.Intersects(test))
		{
			return true;
		}
		return false;
	}
}