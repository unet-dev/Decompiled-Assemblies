using System;
using UnityEngine;

public struct OBB
{
	public Quaternion rotation;

	public Vector3 position;

	public Vector3 extents;

	public Vector3 forward;

	public Vector3 right;

	public Vector3 up;

	public float reject;

	public OBB(Transform transform, Bounds bounds) : this(transform.position, transform.lossyScale, transform.rotation, bounds)
	{
	}

	public OBB(Vector3 position, Vector3 scale, Quaternion rotation, Bounds bounds)
	{
		this.rotation = rotation;
		this.position = position + (rotation * Vector3.Scale(scale, bounds.center));
		this.extents = Vector3.Scale(scale, bounds.extents);
		this.forward = rotation * Vector3.forward;
		this.right = rotation * Vector3.right;
		this.up = rotation * Vector3.up;
		this.reject = this.extents.sqrMagnitude;
	}

	public OBB(Vector3 position, Quaternion rotation, Bounds bounds)
	{
		this.rotation = rotation;
		this.position = position + (rotation * bounds.center);
		this.extents = bounds.extents;
		this.forward = rotation * Vector3.forward;
		this.right = rotation * Vector3.right;
		this.up = rotation * Vector3.up;
		this.reject = this.extents.sqrMagnitude;
	}

	public OBB(Vector3 position, Vector3 size, Quaternion rotation)
	{
		this.rotation = rotation;
		this.position = position;
		this.extents = size * 0.5f;
		this.forward = rotation * Vector3.forward;
		this.right = rotation * Vector3.right;
		this.up = rotation * Vector3.up;
		this.reject = this.extents.sqrMagnitude;
	}

	public Vector3 ClosestPoint(Vector3 target)
	{
		bool flag = false;
		bool flag1 = false;
		bool flag2 = false;
		Vector3 vector3 = this.position;
		Vector3 vector31 = target - this.position;
		float single = Vector3.Dot(vector31, this.right);
		if (single > this.extents.x)
		{
			vector3 = vector3 + (this.right * this.extents.x);
		}
		else if (single >= -this.extents.x)
		{
			flag = true;
			vector3 = vector3 + (this.right * single);
		}
		else
		{
			vector3 = vector3 - (this.right * this.extents.x);
		}
		float single1 = Vector3.Dot(vector31, this.up);
		if (single1 > this.extents.y)
		{
			vector3 = vector3 + (this.up * this.extents.y);
		}
		else if (single1 >= -this.extents.y)
		{
			flag1 = true;
			vector3 = vector3 + (this.up * single1);
		}
		else
		{
			vector3 = vector3 - (this.up * this.extents.y);
		}
		float single2 = Vector3.Dot(vector31, this.forward);
		if (single2 > this.extents.z)
		{
			vector3 = vector3 + (this.forward * this.extents.z);
		}
		else if (single2 >= -this.extents.z)
		{
			flag2 = true;
			vector3 = vector3 + (this.forward * single2);
		}
		else
		{
			vector3 = vector3 - (this.forward * this.extents.z);
		}
		if (flag & flag1 & flag2)
		{
			return target;
		}
		return vector3;
	}

	public bool Contains(Vector3 target)
	{
		if ((target - this.position).sqrMagnitude > this.reject)
		{
			return false;
		}
		return this.ClosestPoint(target) == target;
	}

	public float Distance(OBB other)
	{
		OBB oBB = this;
		OBB oBB1 = other;
		Vector3 vector3 = oBB.position;
		vector3 = oBB.ClosestPoint(oBB1.position);
		Vector3 vector31 = oBB1.ClosestPoint(vector3);
		vector3 = oBB.ClosestPoint(vector31);
		vector31 = oBB1.ClosestPoint(vector3);
		return Vector3.Distance(vector3, vector31);
	}

	public Vector3 GetPoint(float x, float y, float z)
	{
		return ((this.position + (x * this.extents.x * this.right)) + (y * this.extents.y * this.up)) + (z * this.extents.z * this.forward);
	}

	public bool Intersects(OBB target)
	{
		return target.Contains(this.ClosestPoint(target.position));
	}

	public bool Intersects(Ray ray)
	{
		RaycastHit raycastHit;
		return this.Trace(ray, out raycastHit, Single.PositiveInfinity);
	}

	public Bounds ToBounds()
	{
		Vector3 vector3 = this.extents.x * this.right;
		Vector3 vector31 = this.extents.y * this.up;
		Vector3 vector32 = this.extents.z * this.forward;
		Bounds bound = new Bounds(this.position, Vector3.zero);
		bound.Encapsulate(((this.position + vector31) + vector3) + vector32);
		bound.Encapsulate(((this.position + vector31) + vector3) - vector32);
		bound.Encapsulate(((this.position + vector31) - vector3) + vector32);
		bound.Encapsulate(((this.position + vector31) - vector3) - vector32);
		bound.Encapsulate(((this.position - vector31) + vector3) + vector32);
		bound.Encapsulate(((this.position - vector31) + vector3) - vector32);
		bound.Encapsulate(((this.position - vector31) - vector3) + vector32);
		bound.Encapsulate(((this.position - vector31) - vector3) - vector32);
		return bound;
	}

	public bool Trace(Ray ray, out RaycastHit hit, float maxDistance = Single.PositiveInfinity)
	{
		float single;
		float single1;
		float single2;
		float single3;
		float single4;
		float single5;
		hit = new RaycastHit();
		Vector3 vector3 = this.right;
		Vector3 vector31 = this.up;
		Vector3 vector32 = this.forward;
		float single6 = this.extents.x;
		float single7 = this.extents.y;
		float single8 = this.extents.z;
		Vector3 vector33 = ray.origin - this.position;
		Vector3 vector34 = ray.direction;
		float single9 = Vector3.Dot(vector34, vector3);
		float single10 = Vector3.Dot(vector34, vector31);
		float single11 = Vector3.Dot(vector34, vector32);
		float single12 = Vector3.Dot(vector33, vector3);
		float single13 = Vector3.Dot(vector33, vector31);
		float single14 = Vector3.Dot(vector33, vector32);
		if (single9 > 0f)
		{
			single = (-single6 - single12) / single9;
			single1 = (single6 - single12) / single9;
		}
		else if (single9 >= 0f)
		{
			single = Single.MinValue;
			single1 = Single.MaxValue;
		}
		else
		{
			single = (single6 - single12) / single9;
			single1 = (-single6 - single12) / single9;
		}
		if (single10 > 0f)
		{
			single2 = (-single7 - single13) / single10;
			single3 = (single7 - single13) / single10;
		}
		else if (single10 >= 0f)
		{
			single2 = Single.MinValue;
			single3 = Single.MaxValue;
		}
		else
		{
			single2 = (single7 - single13) / single10;
			single3 = (-single7 - single13) / single10;
		}
		if (single11 > 0f)
		{
			single4 = (-single8 - single14) / single11;
			single5 = (single8 - single14) / single11;
		}
		else if (single11 >= 0f)
		{
			single4 = Single.MinValue;
			single5 = Single.MaxValue;
		}
		else
		{
			single4 = (single8 - single14) / single11;
			single5 = (-single8 - single14) / single11;
		}
		float single15 = Mathx.Min(single1, single3, single5);
		if (single15 < 0f)
		{
			return false;
		}
		float single16 = Mathx.Max(single, single2, single4);
		if (single16 > single15)
		{
			return false;
		}
		float single17 = Mathf.Clamp(0f, single16, single15);
		if (single17 > maxDistance)
		{
			return false;
		}
		hit.point = ray.origin + (ray.direction * single17);
		hit.distance = single17;
		return true;
	}

	public void Transform(Vector3 position, Vector3 scale, Quaternion rotation)
	{
		this.rotation *= rotation;
		this.position = position + (rotation * Vector3.Scale(scale, this.position));
		this.extents = Vector3.Scale(scale, this.extents);
	}
}