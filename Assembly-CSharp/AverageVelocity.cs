using System;
using UnityEngine;

public class AverageVelocity
{
	private Vector3 pos;

	private float time;

	private float lastEntry;

	private float averageSpeed;

	private Vector3 averageVelocity;

	public Vector3 Average
	{
		get
		{
			return this.averageVelocity;
		}
	}

	public float Speed
	{
		get
		{
			return this.averageSpeed;
		}
	}

	public AverageVelocity()
	{
	}

	public void Record(Vector3 newPos)
	{
		float single = Time.time - this.time;
		if (single < 0.1f)
		{
			return;
		}
		if (this.pos.sqrMagnitude > 0f)
		{
			Vector3 vector3 = newPos - this.pos;
			this.averageVelocity = vector3 * (1f / single);
			this.averageSpeed = this.averageVelocity.magnitude;
		}
		this.time = Time.time;
		this.pos = newPos;
	}
}