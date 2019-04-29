using System;
using UnityEngine;

public struct Line
{
	public Vector3 point0;

	public Vector3 point1;

	public Line(Vector3 point0, Vector3 point1)
	{
		this.point0 = point0;
		this.point1 = point1;
	}

	public Line(Vector3 origin, Vector3 direction, float length)
	{
		this.point0 = origin;
		this.point1 = origin + (direction * length);
	}

	public Vector3 ClosestPoint(Vector3 pos)
	{
		Vector3 vector3 = this.point1 - this.point0;
		float single = vector3.magnitude;
		Vector3 vector31 = vector3 / single;
		return this.point0 + (Mathf.Clamp(Vector3.Dot(pos - this.point0, vector31), 0f, single) * vector31);
	}

	public float Distance(Vector3 pos)
	{
		return (pos - this.ClosestPoint(pos)).magnitude;
	}

	public float SqrDistance(Vector3 pos)
	{
		return (pos - this.ClosestPoint(pos)).sqrMagnitude;
	}

	public bool Trace(Ray ray, float radius, out RaycastHit hit, float maxDistance = Single.PositiveInfinity)
	{
		hit = new RaycastHit();
		if (radius <= 0f)
		{
			return false;
		}
		Vector3 vector3 = this.point1 - this.point0;
		Vector3 vector31 = ray.direction;
		Vector3 vector32 = this.point0 - ray.origin;
		float single = Vector3.Dot(vector3, vector3);
		float single1 = Vector3.Dot(vector3, vector31);
		float single2 = Vector3.Dot(vector31, vector32);
		float single3 = single - single1 * single1;
		float single4 = 0f;
		float single5 = single2;
		if (single3 >= Mathf.Epsilon)
		{
			float single6 = Vector3.Dot(vector3, vector32);
			float single7 = 1f / single3;
			single4 = single7 * (single1 * single2 - single6);
			single5 = single7 * (single * single2 - single1 * single6);
			single4 = Mathf.Clamp01(single4);
		}
		if (single5 < 0f || single5 > maxDistance)
		{
			return false;
		}
		Vector3 vector33 = this.point0 + (single4 * vector3);
		Vector3 vector34 = (ray.origin + (single5 * vector31)) - vector33;
		float single8 = vector34.magnitude;
		if (single8 > radius)
		{
			return false;
		}
		hit.point = vector33;
		hit.normal = vector34 / single8;
		hit.distance = Vector3.Distance(ray.origin, hit.point);
		return true;
	}
}