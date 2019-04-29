using System;
using UnityEngine;

public class DeployVolumeEntityBounds : DeployVolume
{
	private Bounds bounds = new Bounds(Vector3.zero, Vector3.one);

	public DeployVolumeEntityBounds()
	{
	}

	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		this.bounds = rootObj.GetComponent<BaseEntity>().bounds;
	}

	protected override bool Check(Vector3 position, Quaternion rotation, int mask = -1)
	{
		position = position + (rotation * this.bounds.center);
		if (DeployVolume.CheckOBB(new OBB(position, this.bounds.size, rotation), this.layers & mask, this.ignore))
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