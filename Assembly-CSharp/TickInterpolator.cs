using System;
using System.Collections.Generic;
using UnityEngine;

public class TickInterpolator
{
	private List<TickInterpolator.Segment> points = new List<TickInterpolator.Segment>();

	private int index;

	public float Length;

	public Vector3 CurrentPoint;

	public Vector3 StartPoint;

	public Vector3 EndPoint;

	public TickInterpolator()
	{
	}

	public void AddPoint(Vector3 point)
	{
		TickInterpolator.Segment segment = new TickInterpolator.Segment(this.EndPoint, point);
		this.points.Add(segment);
		this.Length += segment.length;
		this.EndPoint = segment.point;
	}

	public bool HasNext()
	{
		return this.index < this.points.Count;
	}

	public bool MoveNext(float distance)
	{
		float single = 0f;
		while (single < distance && this.index < this.points.Count)
		{
			TickInterpolator.Segment item = this.points[this.index];
			this.CurrentPoint = item.point;
			single += item.length;
			this.index++;
		}
		return single > 0f;
	}

	public void Reset()
	{
		this.index = 0;
		this.CurrentPoint = this.StartPoint;
	}

	public void Reset(Vector3 point)
	{
		this.points.Clear();
		this.index = 0;
		this.Length = 0f;
		Vector3 vector3 = point;
		Vector3 vector31 = vector3;
		this.EndPoint = vector3;
		Vector3 vector32 = vector31;
		vector31 = vector32;
		this.StartPoint = vector32;
		this.CurrentPoint = vector31;
	}

	public void TransformEntries(Matrix4x4 matrix)
	{
		for (int i = 0; i < this.points.Count; i++)
		{
			TickInterpolator.Segment item = this.points[i];
			item.point = matrix.MultiplyPoint3x4(item.point);
			this.points[i] = item;
		}
		this.CurrentPoint = matrix.MultiplyPoint3x4(this.CurrentPoint);
		this.StartPoint = matrix.MultiplyPoint3x4(this.StartPoint);
		this.EndPoint = matrix.MultiplyPoint3x4(this.EndPoint);
	}

	private struct Segment
	{
		public Vector3 point;

		public float length;

		public Segment(Vector3 a, Vector3 b)
		{
			this.point = b;
			this.length = Vector3.Distance(a, b);
		}
	}
}