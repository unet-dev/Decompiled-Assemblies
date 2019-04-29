using System;
using UnityEngine;

public struct Cylinder
{
	public Vector3 position;

	public float radius;

	public float extent;

	public Cylinder(Vector3 position, float radius, float extent)
	{
		this.position = position;
		this.radius = radius;
		this.extent = extent;
	}

	public Vector3 ClosestPoint(Vector3 target)
	{
		Vector3 vector3 = target;
		Vector3 vector31 = target - this.position;
		float single = vector31.Magnitude2D();
		if (single > this.radius)
		{
			float single1 = this.radius / single;
			vector3.x = this.position.x + vector31.x * single1;
			vector3.z = this.position.z + vector31.z * single1;
		}
		vector3.y = Mathf.Clamp(target.y, this.position.y - this.extent, this.position.y + this.extent);
		return vector3;
	}

	public bool Contains(Vector3 target)
	{
		return this.ClosestPoint(target) == target;
	}
}