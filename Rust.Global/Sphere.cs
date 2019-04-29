using System;
using UnityEngine;

public struct Sphere
{
	public Vector3 position;

	public float radius;

	public Sphere(Vector3 position, float radius)
	{
		this.position = position;
		this.radius = radius;
	}

	public Vector3 ClosestPoint(Vector3 target)
	{
		Vector3 vector3 = target;
		Vector3 vector31 = target - this.position;
		float single = vector31.magnitude;
		if (single <= this.radius)
		{
			return vector3;
		}
		float single1 = this.radius / single;
		vector3.x = this.position.x + vector31.x * single1;
		vector3.y = this.position.y + vector31.y * single1;
		vector3.z = this.position.z + vector31.z * single1;
		return vector3;
	}

	public bool Contains(Vector3 target)
	{
		return this.ClosestPoint(target) == target;
	}

	public void Move(Vector3 direction, float distance, int layerMask = 0)
	{
		RaycastHit raycastHit;
		if (layerMask == 0 || !Physics.SphereCast(this.position, this.radius, direction, out raycastHit, distance, layerMask))
		{
			this.position = this.position + (direction * distance);
			return;
		}
		this.position = this.position + (direction * raycastHit.distance);
	}

	public bool Trace(Ray ray, out RaycastHit hit, float maxDistance = Single.PositiveInfinity)
	{
		hit = new RaycastHit();
		if (this.radius <= 0f)
		{
			return false;
		}
		float single = 1f;
		float single1 = 2f * Vector3.Dot(ray.direction, ray.origin - this.position);
		Vector3 vector3 = ray.origin - this.position;
		float single2 = vector3.sqrMagnitude - this.radius * this.radius;
		float single3 = single1 * single1 - 4f * single * single2;
		if (single3 < 0f)
		{
			return false;
		}
		float single4 = Mathf.Sqrt(single3);
		float single5 = 2f * single;
		float single6 = -single1;
		float single7 = (single6 - single4) / single5;
		if (single7 < 0f)
		{
			if ((single6 + single4) / single5 < 0f)
			{
				return false;
			}
			hit.point = ray.origin;
			vector3 = hit.point - this.position;
			hit.normal = vector3.normalized;
			hit.distance = 0f;
			return true;
		}
		if (single7 > maxDistance)
		{
			return false;
		}
		hit.point = ray.origin + (single7 * ray.direction);
		hit.normal = (hit.point - this.position) / this.radius;
		hit.distance = single7;
		return true;
	}
}