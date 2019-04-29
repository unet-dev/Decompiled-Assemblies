using System;
using UnityEngine;

public class LifeScale : BaseMonoBehaviour
{
	[NonSerialized]
	private bool initialized;

	[NonSerialized]
	private Vector3 initialScale;

	public Vector3 finalScale = Vector3.one;

	private Vector3 targetLerpScale = Vector3.zero;

	private Action updateScaleAction;

	public LifeScale()
	{
	}

	protected void Awake()
	{
		this.updateScaleAction = new Action(this.UpdateScale);
	}

	public void Init()
	{
		if (!this.initialized)
		{
			this.initialScale = base.transform.localScale;
			this.initialized = true;
		}
	}

	public void OnEnable()
	{
		this.Init();
		base.transform.localScale = this.initialScale;
	}

	public void SetProgress(float progress)
	{
		this.Init();
		this.targetLerpScale = Vector3.Lerp(this.initialScale, this.finalScale, progress);
		base.InvokeRepeating(this.updateScaleAction, 0f, 0.015f);
	}

	public void UpdateScale()
	{
		base.transform.localScale = Vector3.Lerp(base.transform.localScale, this.targetLerpScale, Time.deltaTime);
		if (base.transform.localScale == this.targetLerpScale)
		{
			this.targetLerpScale = Vector3.zero;
			base.CancelInvoke(this.updateScaleAction);
		}
	}
}