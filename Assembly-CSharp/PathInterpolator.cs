using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PathInterpolator
{
	public Vector3[] Points;

	public Vector3[] Tangents;

	private bool initialized;

	public int DefaultMaxIndex
	{
		get
		{
			return (int)this.Points.Length - 1;
		}
	}

	public int DefaultMinIndex
	{
		get
		{
			return 0;
		}
	}

	public float EndOffset
	{
		get
		{
			return this.Length * (float)(this.DefaultMaxIndex - this.MaxIndex) / (float)(this.DefaultMaxIndex - this.DefaultMinIndex);
		}
	}

	public float Length
	{
		get;
		private set;
	}

	public int MaxIndex
	{
		get;
		set;
	}

	public int MinIndex
	{
		get;
		set;
	}

	public float StartOffset
	{
		get
		{
			return this.Length * (float)(this.MinIndex - this.DefaultMinIndex) / (float)(this.DefaultMaxIndex - this.DefaultMinIndex);
		}
	}

	public float StepSize
	{
		get;
		private set;
	}

	public PathInterpolator(Vector3[] points)
	{
		if ((int)points.Length < 2)
		{
			throw new ArgumentException("Point list too short.");
		}
		this.Points = points;
		this.MinIndex = this.DefaultMinIndex;
		this.MaxIndex = this.DefaultMaxIndex;
	}

	public Vector3 GetEndPoint()
	{
		return this.Points[this.MaxIndex];
	}

	public Vector3 GetEndTangent()
	{
		if (!this.initialized)
		{
			throw new Exception("Tangents have not been calculated yet or are outdated.");
		}
		return this.Tangents[this.MaxIndex];
	}

	public Vector3 GetPoint(float distance)
	{
		float single = distance / this.Length * (float)((int)this.Points.Length);
		int num = (int)single;
		if (single <= (float)this.MinIndex)
		{
			return this.GetStartPoint();
		}
		if (single >= (float)this.MaxIndex)
		{
			return this.GetEndPoint();
		}
		Vector3 points = this.Points[num];
		Vector3 vector3 = this.Points[num + 1];
		return Vector3.Lerp(points, vector3, single - (float)num);
	}

	public Vector3 GetPointCubicHermite(float distance)
	{
		if (!this.initialized)
		{
			throw new Exception("Tangents have not been calculated yet or are outdated.");
		}
		float single = distance / this.Length * (float)((int)this.Points.Length);
		int num = (int)single;
		if (single <= (float)this.MinIndex)
		{
			return this.GetStartPoint();
		}
		if (single >= (float)this.MaxIndex)
		{
			return this.GetEndPoint();
		}
		Vector3 points = this.Points[num];
		Vector3 vector3 = this.Points[num + 1];
		Vector3 tangents = this.Tangents[num] * this.StepSize;
		Vector3 tangents1 = this.Tangents[num + 1] * this.StepSize;
		float single1 = single - (float)num;
		float single2 = single1 * single1;
		float single3 = single1 * single2;
		return ((((2f * single3 - 3f * single2 + 1f) * points) + ((single3 - 2f * single2 + single1) * tangents)) + ((-2f * single3 + 3f * single2) * vector3)) + ((single3 - single2) * tangents1);
	}

	public Vector3 GetStartPoint()
	{
		return this.Points[this.MinIndex];
	}

	public Vector3 GetStartTangent()
	{
		if (!this.initialized)
		{
			throw new Exception("Tangents have not been calculated yet or are outdated.");
		}
		return this.Tangents[this.MinIndex];
	}

	public Vector3 GetTangent(float distance)
	{
		if (!this.initialized)
		{
			throw new Exception("Tangents have not been calculated yet or are outdated.");
		}
		float single = distance / this.Length * (float)((int)this.Tangents.Length);
		int num = (int)single;
		if (single <= (float)this.MinIndex)
		{
			return this.GetStartTangent();
		}
		if (single >= (float)this.MaxIndex)
		{
			return this.GetEndTangent();
		}
		Vector3 tangents = this.Tangents[num];
		Vector3 vector3 = this.Tangents[num + 1];
		return Vector3.Lerp(tangents, vector3, single - (float)num);
	}

	public void RecalculateTangents()
	{
		if (this.Tangents == null || (int)this.Tangents.Length != (int)this.Points.Length)
		{
			this.Tangents = new Vector3[(int)this.Points.Length];
		}
		float single = 0f;
		for (int i = 0; i < (int)this.Tangents.Length; i++)
		{
			Vector3 points = this.Points[Mathf.Max(i - 1, 0)];
			Vector3 vector3 = this.Points[Mathf.Min(i + 1, (int)this.Tangents.Length - 1)] - points;
			float single1 = vector3.magnitude;
			single += single1;
			this.Tangents[i] = vector3 / single1;
		}
		this.Length = single;
		this.StepSize = single / (float)((int)this.Points.Length);
		this.initialized = true;
	}

	public void Resample(float distance)
	{
		if (!this.initialized)
		{
			throw new Exception("Tangents have not been calculated yet or are outdated.");
		}
		Vector3[] pointCubicHermite = new Vector3[Mathf.RoundToInt(this.Length / distance)];
		for (int i = 0; i < (int)pointCubicHermite.Length; i++)
		{
			pointCubicHermite[i] = this.GetPointCubicHermite((float)i * distance);
		}
		this.Points = pointCubicHermite;
		this.initialized = false;
	}

	public void Smoothen(int iterations = 1)
	{
		float single = 0.25f;
		for (int i = 0; i < iterations; i++)
		{
			Vector3 points = this.Points[0];
			for (int j = 1; j < (int)this.Points.Length - 1; j++)
			{
				Vector3 vector3 = this.Points[j];
				Vector3 points1 = this.Points[j + 1];
				this.Points[j] = (((points + vector3) + vector3) + points1) * single;
				points = vector3;
			}
		}
		this.initialized = false;
	}
}