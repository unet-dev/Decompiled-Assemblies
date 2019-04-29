using System;
using UnityEngine;

public class ExplosionsScaleCurves : MonoBehaviour
{
	public AnimationCurve ScaleCurveX = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	public AnimationCurve ScaleCurveY = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	public AnimationCurve ScaleCurveZ = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	public Vector3 GraphTimeMultiplier = Vector3.one;

	public Vector3 GraphScaleMultiplier = Vector3.one;

	private float startTime;

	private Transform t;

	private float evalX;

	private float evalY;

	private float evalZ;

	public ExplosionsScaleCurves()
	{
	}

	private void Awake()
	{
		this.t = base.transform;
	}

	private void OnEnable()
	{
		this.startTime = Time.time;
		this.evalX = 0f;
		this.evalY = 0f;
		this.evalZ = 0f;
	}

	private void Update()
	{
		float single = Time.time - this.startTime;
		if (single <= this.GraphTimeMultiplier.x)
		{
			this.evalX = this.ScaleCurveX.Evaluate(single / this.GraphTimeMultiplier.x) * this.GraphScaleMultiplier.x;
		}
		if (single <= this.GraphTimeMultiplier.y)
		{
			this.evalY = this.ScaleCurveY.Evaluate(single / this.GraphTimeMultiplier.y) * this.GraphScaleMultiplier.y;
		}
		if (single <= this.GraphTimeMultiplier.z)
		{
			this.evalZ = this.ScaleCurveZ.Evaluate(single / this.GraphTimeMultiplier.z) * this.GraphScaleMultiplier.z;
		}
		this.t.localScale = new Vector3(this.evalX, this.evalY, this.evalZ);
	}
}