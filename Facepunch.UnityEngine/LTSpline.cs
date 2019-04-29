using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LTSpline
{
	public static int DISTANCE_COUNT;

	public static int SUBLINE_COUNT;

	public float distance;

	public bool constantSpeed = true;

	public Vector3[] pts;

	[NonSerialized]
	public Vector3[] ptsAdj;

	public int ptsAdjLength;

	public bool orientToPath;

	public bool orientToPath2d;

	private int numSections;

	private int currPt;

	static LTSpline()
	{
		LTSpline.DISTANCE_COUNT = 3;
		LTSpline.SUBLINE_COUNT = 20;
	}

	public LTSpline(Vector3[] pts)
	{
		this.init(pts, true);
	}

	public LTSpline(Vector3[] pts, bool constantSpeed)
	{
		this.constantSpeed = constantSpeed;
		this.init(pts, constantSpeed);
	}

	public void drawGizmo(Color color)
	{
		if (this.ptsAdjLength >= 4)
		{
			Vector3 vector3 = this.ptsAdj[0];
			Color color1 = Gizmos.color;
			Gizmos.color = color;
			for (int i = 0; i < this.ptsAdjLength; i++)
			{
				Vector3 vector31 = this.ptsAdj[i];
				Gizmos.DrawLine(vector3, vector31);
				vector3 = vector31;
			}
			Gizmos.color = color1;
		}
	}

	public static void drawGizmo(Transform[] arr, Color color)
	{
		if ((int)arr.Length >= 4)
		{
			Vector3[] vector3Array = new Vector3[(int)arr.Length];
			for (int i = 0; i < (int)arr.Length; i++)
			{
				vector3Array[i] = arr[i].position;
			}
			LTSpline lTSpline = new LTSpline(vector3Array);
			Vector3 vector3 = lTSpline.ptsAdj[0];
			Color color1 = Gizmos.color;
			Gizmos.color = color;
			for (int j = 0; j < lTSpline.ptsAdjLength; j++)
			{
				Vector3 vector31 = lTSpline.ptsAdj[j];
				Gizmos.DrawLine(vector3, vector31);
				vector3 = vector31;
			}
			Gizmos.color = color1;
		}
	}

	public static void drawLine(Transform[] arr, float width, Color color)
	{
		int length = (int)arr.Length;
	}

	public void drawLinesGLLines(Material outlineMaterial, Color color, float width)
	{
		GL.PushMatrix();
		outlineMaterial.SetPass(0);
		GL.LoadPixelMatrix();
		GL.Begin(1);
		GL.Color(color);
		if (this.constantSpeed)
		{
			if (this.ptsAdjLength >= 4)
			{
				Vector3 vector3 = this.ptsAdj[0];
				for (int i = 0; i < this.ptsAdjLength; i++)
				{
					Vector3 vector31 = this.ptsAdj[i];
					GL.Vertex(vector3);
					GL.Vertex(vector31);
					vector3 = vector31;
				}
			}
		}
		else if ((int)this.pts.Length >= 4)
		{
			Vector3 vector32 = this.pts[0];
			float length = 1f / ((float)((int)this.pts.Length) * 10f);
			for (float j = 0f; j < 1f; j += length)
			{
				Vector3 vector33 = this.interp(j / 1f);
				GL.Vertex(vector32);
				GL.Vertex(vector33);
				vector32 = vector33;
			}
		}
		GL.End();
		GL.PopMatrix();
	}

	public Vector3[] generateVectors()
	{
		if ((int)this.pts.Length >= 4)
		{
			List<Vector3> vector3s = new List<Vector3>()
			{
				this.pts[0]
			};
			float length = 1f / ((float)((int)this.pts.Length) * 10f);
			for (float i = 0f; i < 1f; i += length)
			{
				vector3s.Add(this.interp(i / 1f));
			}
			vector3s.ToArray();
		}
		return null;
	}

	public void gizmoDraw(float t = -1f)
	{
		if (this.ptsAdj == null || this.ptsAdj.Length == 0)
		{
			return;
		}
		Vector3 vector3 = this.ptsAdj[0];
		for (int i = 0; i < this.ptsAdjLength; i++)
		{
			Vector3 vector31 = this.ptsAdj[i];
			Gizmos.DrawLine(vector3, vector31);
			vector3 = vector31;
		}
	}

	private void init(Vector3[] pts, bool constantSpeed)
	{
		if ((int)pts.Length < 4)
		{
			LeanTween.logError("LeanTween - When passing values for a spline path, you must pass four or more values!");
			return;
		}
		this.pts = new Vector3[(int)pts.Length];
		Array.Copy(pts, this.pts, (int)pts.Length);
		this.numSections = (int)pts.Length - 3;
		float sUBLINECOUNT = Single.PositiveInfinity;
		Vector3 vector3 = this.pts[1];
		float single = 0f;
		for (int i = 1; i < (int)this.pts.Length - 1; i++)
		{
			float single1 = Vector3.Distance(this.pts[i], vector3);
			if (single1 < sUBLINECOUNT)
			{
				sUBLINECOUNT = single1;
			}
			single += single1;
		}
		if (constantSpeed)
		{
			sUBLINECOUNT = single / (float)(this.numSections * LTSpline.SUBLINE_COUNT);
			float sUBLINECOUNT1 = sUBLINECOUNT / (float)LTSpline.SUBLINE_COUNT;
			int num = (int)Mathf.Ceil(single / sUBLINECOUNT1) * LTSpline.DISTANCE_COUNT;
			if (num <= 1)
			{
				num = 2;
			}
			this.ptsAdj = new Vector3[num];
			vector3 = this.interp(0f);
			int num1 = 1;
			this.ptsAdj[0] = vector3;
			this.distance = 0f;
			for (int j = 0; j < num + 1; j++)
			{
				float single2 = (float)j / (float)num;
				Vector3 vector31 = this.interp(single2);
				float single3 = Vector3.Distance(vector31, vector3);
				if (single3 >= sUBLINECOUNT1 || single2 >= 1f)
				{
					this.ptsAdj[num1] = vector31;
					this.distance += single3;
					vector3 = vector31;
					num1++;
				}
			}
			this.ptsAdjLength = num1;
		}
	}

	public Vector3 interp(float t)
	{
		this.currPt = Mathf.Min(Mathf.FloorToInt(t * (float)this.numSections), this.numSections - 1);
		float single = t * (float)this.numSections - (float)this.currPt;
		Vector3 vector3 = this.pts[this.currPt];
		Vector3 vector31 = this.pts[this.currPt + 1];
		Vector3 vector32 = this.pts[this.currPt + 2];
		Vector3 vector33 = this.pts[this.currPt + 3];
		return 0.5f * (((((((-vector3 + (3f * vector31)) - (3f * vector32)) + vector33) * (single * single * single)) + (((((2f * vector3) - (5f * vector31)) + (4f * vector32)) - vector33) * (single * single))) + ((-vector3 + vector32) * single)) + (2f * vector31));
	}

	public Vector3 map(float u)
	{
		if (u >= 1f)
		{
			return this.pts[(int)this.pts.Length - 2];
		}
		float single = u * (float)(this.ptsAdjLength - 1);
		int num = (int)Mathf.Floor(single);
		int num1 = (int)Mathf.Ceil(single);
		if (num < 0)
		{
			num = 0;
		}
		Vector3 vector3 = this.ptsAdj[num];
		Vector3 vector31 = this.ptsAdj[num1];
		float single1 = single - (float)num;
		vector3 = vector3 + ((vector31 - vector3) * single1);
		return vector3;
	}

	public void place(Transform transform, float ratio)
	{
		this.place(transform, ratio, Vector3.up);
	}

	public void place(Transform transform, float ratio, Vector3 worldUp)
	{
		transform.position = this.point(ratio);
		ratio += 0.001f;
		if (ratio <= 1f)
		{
			transform.LookAt(this.point(ratio), worldUp);
		}
	}

	public void place2d(Transform transform, float ratio)
	{
		transform.position = this.point(ratio);
		ratio += 0.001f;
		if (ratio <= 1f)
		{
			Vector3 vector3 = this.point(ratio) - transform.position;
			float single = Mathf.Atan2(vector3.y, vector3.x) * 57.29578f;
			transform.eulerAngles = new Vector3(0f, 0f, single);
		}
	}

	public void placeLocal(Transform transform, float ratio)
	{
		this.placeLocal(transform, ratio, Vector3.up);
	}

	public void placeLocal(Transform transform, float ratio, Vector3 worldUp)
	{
		transform.localPosition = this.point(ratio);
		ratio += 0.001f;
		if (ratio <= 1f)
		{
			transform.LookAt(transform.parent.TransformPoint(this.point(ratio)), worldUp);
		}
	}

	public void placeLocal2d(Transform transform, float ratio)
	{
		if (transform.parent == null)
		{
			this.place2d(transform, ratio);
			return;
		}
		transform.localPosition = this.point(ratio);
		ratio += 0.001f;
		if (ratio <= 1f)
		{
			Vector3 vector3 = this.point(ratio) - transform.localPosition;
			float single = Mathf.Atan2(vector3.y, vector3.x) * 57.29578f;
			transform.localEulerAngles = new Vector3(0f, 0f, single);
		}
	}

	public Vector3 point(float ratio)
	{
		float single = (ratio > 1f ? 1f : ratio);
		if (!this.constantSpeed)
		{
			return this.interp(single);
		}
		return this.map(single);
	}

	public float ratioAtPoint(Vector3 pt)
	{
		float single = Single.MaxValue;
		int num = 0;
		for (int i = 0; i < this.ptsAdjLength; i++)
		{
			float single1 = Vector3.Distance(pt, this.ptsAdj[i]);
			if (single1 < single)
			{
				single = single1;
				num = i;
			}
		}
		return (float)num / (float)(this.ptsAdjLength - 1);
	}
}