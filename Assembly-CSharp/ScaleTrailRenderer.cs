using System;
using UnityEngine;

public class ScaleTrailRenderer : ScaleRenderer
{
	private TrailRenderer trailRenderer;

	[NonSerialized]
	private float startWidth;

	[NonSerialized]
	private float endWidth;

	[NonSerialized]
	private float duration;

	[NonSerialized]
	private float startMultiplier;

	public ScaleTrailRenderer()
	{
	}

	public override void GatherInitialValues()
	{
		base.GatherInitialValues();
		if (!this.myRenderer)
		{
			this.trailRenderer = base.GetComponentInChildren<TrailRenderer>();
		}
		else
		{
			this.trailRenderer = this.myRenderer.GetComponent<TrailRenderer>();
		}
		this.startWidth = this.trailRenderer.startWidth;
		this.endWidth = this.trailRenderer.endWidth;
		this.duration = this.trailRenderer.time;
		this.startMultiplier = this.trailRenderer.widthMultiplier;
	}

	public override void SetScale_Internal(float scale)
	{
		if (scale == 0f)
		{
			this.trailRenderer.emitting = false;
			this.trailRenderer.enabled = false;
			this.trailRenderer.time = 0f;
			this.trailRenderer.Clear();
			return;
		}
		if (!this.trailRenderer.emitting)
		{
			this.trailRenderer.Clear();
		}
		this.trailRenderer.emitting = true;
		this.trailRenderer.enabled = true;
		base.SetScale_Internal(scale);
		this.trailRenderer.widthMultiplier = this.startMultiplier * scale;
		this.trailRenderer.time = this.duration * scale;
	}
}