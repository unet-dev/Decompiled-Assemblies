using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class BoundsEx
{
	public static Bounds Transform(this Bounds bounds, Matrix4x4 matrix)
	{
		Vector3 vector3 = matrix.MultiplyPoint3x4(bounds.center);
		Vector3 vector31 = bounds.extents;
		Vector3 vector32 = matrix.MultiplyVector(new Vector3(vector31.x, 0f, 0f));
		Vector3 vector33 = matrix.MultiplyVector(new Vector3(0f, vector31.y, 0f));
		Vector3 vector34 = matrix.MultiplyVector(new Vector3(0f, 0f, vector31.z));
		vector31.x = Mathf.Abs(vector32.x) + Mathf.Abs(vector33.x) + Mathf.Abs(vector34.x);
		vector31.y = Mathf.Abs(vector32.y) + Mathf.Abs(vector33.y) + Mathf.Abs(vector34.y);
		vector31.z = Mathf.Abs(vector32.z) + Mathf.Abs(vector33.z) + Mathf.Abs(vector34.z);
		return new Bounds()
		{
			center = vector3,
			extents = vector31
		};
	}

	public static Bounds XZ3D(this Bounds bounds)
	{
		return new Bounds(bounds.center.XZ3D(), bounds.size.XZ3D());
	}
}