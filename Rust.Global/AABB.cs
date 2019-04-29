using System;
using UnityEngine;

public struct AABB
{
	public Vector3 position;

	public Vector3 extents;

	public AABB(Vector3 position, Vector3 size)
	{
		this.position = position;
		this.extents = size * 0.5f;
	}

	public Vector3 ClosestPoint(Vector3 target)
	{
		Vector3 vector3 = target;
		float single = this.position.x - this.extents.x;
		float single1 = this.position.x + this.extents.x;
		if (target.x < single)
		{
			vector3.x = single;
		}
		else if (target.x > single1)
		{
			vector3.x = single1;
		}
		float single2 = this.position.y - this.extents.y;
		float single3 = this.position.y + this.extents.y;
		if (target.y < single2)
		{
			vector3.y = single2;
		}
		else if (target.y > single3)
		{
			vector3.y = single3;
		}
		float single4 = this.position.z - this.extents.z;
		float single5 = this.position.z + this.extents.z;
		if (target.z < single4)
		{
			vector3.z = single4;
		}
		else if (target.z > single5)
		{
			vector3.z = single5;
		}
		return vector3;
	}

	public bool Contains(Vector3 target)
	{
		return this.ClosestPoint(target) == target;
	}
}