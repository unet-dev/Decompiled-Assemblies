using System;
using UnityEngine;

public class ScaleRenderer : MonoBehaviour
{
	public bool useRandomScale;

	public float scaleMin = 1f;

	public float scaleMax = 1f;

	private float lastScale = -1f;

	protected bool hasInitialValues;

	public Renderer myRenderer;

	public ScaleRenderer()
	{
	}

	public virtual void GatherInitialValues()
	{
		this.hasInitialValues = true;
	}

	private bool ScaleDifferent(float newScale)
	{
		return newScale != this.lastScale;
	}

	public virtual void SetRendererEnabled(bool isEnabled)
	{
		if (this.myRenderer && this.myRenderer.enabled != isEnabled)
		{
			this.myRenderer.enabled = isEnabled;
		}
	}

	public void SetScale(float scale)
	{
		if (!this.hasInitialValues)
		{
			this.GatherInitialValues();
		}
		if (this.ScaleDifferent(scale))
		{
			this.SetRendererEnabled(scale != 0f);
			this.SetScale_Internal(scale);
		}
	}

	public virtual void SetScale_Internal(float scale)
	{
		this.lastScale = scale;
	}

	public void Start()
	{
		if (this.useRandomScale)
		{
			this.SetScale(UnityEngine.Random.Range(this.scaleMin, this.scaleMax));
		}
	}
}