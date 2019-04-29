using System;
using UnityEngine;

public class LTBezierPath
{
	public Vector3[] pts;

	public float length;

	public bool orientToPath;

	public bool orientToPath2d;

	private LTBezier[] beziers;

	private float[] lengthRatio;

	private int currentBezier;

	private int previousBezier;

	public float distance
	{
		get
		{
			return this.length;
		}
	}

	public LTBezierPath()
	{
	}

	public LTBezierPath(Vector3[] pts_)
	{
		this.setPoints(pts_);
	}

	public void gizmoDraw(float t = -1f)
	{
		Vector3 vector3 = this.point(0f);
		for (int i = 1; i <= 120; i++)
		{
			Vector3 vector31 = this.point((float)i / 120f);
			Gizmos.color = (this.previousBezier == this.currentBezier ? Color.magenta : Color.grey);
			Gizmos.DrawLine(vector31, vector3);
			vector3 = vector31;
			this.previousBezier = this.currentBezier;
		}
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
		ratio = Mathf.Clamp01(ratio);
		transform.localPosition = this.point(ratio);
		ratio = Mathf.Clamp01(ratio + 0.001f);
		if (ratio <= 1f)
		{
			transform.LookAt(transform.parent.TransformPoint(this.point(ratio)), worldUp);
		}
	}

	public void placeLocal2d(Transform transform, float ratio)
	{
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
		float single = 0f;
		for (int i = 0; i < (int)this.lengthRatio.Length; i++)
		{
			single += this.lengthRatio[i];
			if (single >= ratio)
			{
				return this.beziers[i].point((ratio - (single - this.lengthRatio[i])) / this.lengthRatio[i]);
			}
		}
		return this.beziers[(int)this.lengthRatio.Length - 1].point(1f);
	}

	public void setPoints(Vector3[] pts_)
	{
		int i;
		if ((int)pts_.Length < 4)
		{
			LeanTween.logError("LeanTween - When passing values for a vector path, you must pass four or more values!");
		}
		if ((int)pts_.Length % 4 != 0)
		{
			LeanTween.logError("LeanTween - When passing values for a vector path, they must be in sets of four: controlPoint1, controlPoint2, endPoint2, controlPoint2, controlPoint2...");
		}
		this.pts = pts_;
		int num = 0;
		this.beziers = new LTBezier[(int)this.pts.Length / 4];
		this.lengthRatio = new float[(int)this.beziers.Length];
		this.length = 0f;
		for (i = 0; i < (int)this.pts.Length; i += 4)
		{
			this.beziers[num] = new LTBezier(this.pts[i], this.pts[i + 2], this.pts[i + 1], this.pts[i + 3], 0.05f);
			this.length += this.beziers[num].length;
			num++;
		}
		for (i = 0; i < (int)this.beziers.Length; i++)
		{
			this.lengthRatio[i] = this.beziers[i].length / this.length;
		}
	}
}