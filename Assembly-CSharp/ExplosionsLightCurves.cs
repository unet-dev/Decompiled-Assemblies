using System;
using UnityEngine;

public class ExplosionsLightCurves : MonoBehaviour
{
	public AnimationCurve LightCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	public float GraphTimeMultiplier = 1f;

	public float GraphIntensityMultiplier = 1f;

	private bool canUpdate;

	private float startTime;

	private Light lightSource;

	public ExplosionsLightCurves()
	{
	}

	private void Awake()
	{
		this.lightSource = base.GetComponent<Light>();
		this.lightSource.intensity = this.LightCurve.Evaluate(0f);
	}

	private void OnEnable()
	{
		this.startTime = Time.time;
		this.canUpdate = true;
	}

	private void Update()
	{
		float single = Time.time - this.startTime;
		if (this.canUpdate)
		{
			float single1 = this.LightCurve.Evaluate(single / this.GraphTimeMultiplier) * this.GraphIntensityMultiplier;
			this.lightSource.intensity = single1;
		}
		if (single >= this.GraphTimeMultiplier)
		{
			this.canUpdate = false;
		}
	}
}