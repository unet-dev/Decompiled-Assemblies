using System;
using UnityEngine;

public struct CachedTransform<T>
where T : Component
{
	public T component;

	public Vector3 position;

	public Quaternion rotation;

	public Vector3 localScale;

	public Vector3 forward
	{
		get
		{
			return this.rotation * Vector3.forward;
		}
	}

	public Matrix4x4 localToWorldMatrix
	{
		get
		{
			return Matrix4x4.TRS(this.position, this.rotation, this.localScale);
		}
	}

	public Vector3 right
	{
		get
		{
			return this.rotation * Vector3.right;
		}
	}

	public Vector3 up
	{
		get
		{
			return this.rotation * Vector3.up;
		}
	}

	public Matrix4x4 worldToLocalMatrix
	{
		get
		{
			return this.localToWorldMatrix.inverse;
		}
	}

	public CachedTransform(T instance)
	{
		this.component = instance;
		if (!this.component)
		{
			this.position = Vector3.zero;
			this.rotation = Quaternion.identity;
			this.localScale = Vector3.one;
			return;
		}
		this.position = this.component.transform.position;
		this.rotation = this.component.transform.rotation;
		this.localScale = this.component.transform.localScale;
	}

	public void Apply()
	{
		if (this.component)
		{
			this.component.transform.SetPositionAndRotation(this.position, this.rotation);
			this.component.transform.localScale = this.localScale;
		}
	}

	public static implicit operator Boolean(CachedTransform<T> instance)
	{
		return instance.component != null;
	}

	public void RotateAround(Vector3 center, Vector3 axis, float angle)
	{
		Quaternion quaternion = Quaternion.AngleAxis(angle, axis);
		Vector3 vector3 = quaternion * (this.position - center);
		this.position = center + vector3;
		this.rotation = this.rotation * (Quaternion.Inverse(this.rotation) * quaternion) * this.rotation;
	}
}