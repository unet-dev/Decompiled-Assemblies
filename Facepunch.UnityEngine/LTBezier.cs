using System;
using UnityEngine;

public class LTBezier
{
	public float length;

	private Vector3 a;

	private Vector3 aa;

	private Vector3 bb;

	private Vector3 cc;

	private float len;

	private float[] arcLengths;

	public LTBezier(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float precision)
	{
		this.a = a;
		this.aa = (-a + (3f * (b - c))) + d;
		this.bb = (3f * (a + c)) - (6f * b);
		this.cc = 3f * (b - a);
		this.len = 1f / precision;
		this.arcLengths = new float[(int)this.len + 1];
		this.arcLengths[0] = 0f;
		Vector3 vector3 = a;
		float single = 0f;
		for (int i = 1; (float)i <= this.len; i++)
		{
			Vector3 vector31 = this.bezierPoint((float)i * precision);
			single += (vector3 - vector31).magnitude;
			this.arcLengths[i] = single;
			vector3 = vector31;
		}
		this.length = single;
	}

	private Vector3 bezierPoint(float t)
	{
		return (((((this.aa * t) + this.bb) * t) + this.cc) * t) + this.a;
	}

	private float map(float u)
	{
		float single = u * this.arcLengths[(int)this.len];
		int num = 0;
		int num1 = (int)this.len;
		int num2 = 0;
		while (num < num1)
		{
			num2 = num + ((int)((float)(num1 - num) / 2f) | 0);
			if (this.arcLengths[num2] >= single)
			{
				num1 = num2;
			}
			else
			{
				num = num2 + 1;
			}
		}
		if (this.arcLengths[num2] > single)
		{
			num2--;
		}
		if (num2 < 0)
		{
			num2 = 0;
		}
		return ((float)num2 + (single - this.arcLengths[num2]) / (this.arcLengths[num2 + 1] - this.arcLengths[num2])) / this.len;
	}

	public Vector3 point(float t)
	{
		return this.bezierPoint(this.map(t));
	}
}