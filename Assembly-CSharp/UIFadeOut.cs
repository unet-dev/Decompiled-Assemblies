using System;
using UnityEngine;

public class UIFadeOut : MonoBehaviour
{
	public float secondsToFadeOut = 3f;

	public bool destroyOnFaded = true;

	public CanvasGroup targetGroup;

	private float timeStarted;

	public UIFadeOut()
	{
	}

	private void Start()
	{
		this.timeStarted = Time.realtimeSinceStartup;
	}

	private void Update()
	{
		this.targetGroup.alpha = Mathf.InverseLerp(this.timeStarted + this.secondsToFadeOut, this.timeStarted, Time.realtimeSinceStartup);
		if (this.destroyOnFaded && Time.realtimeSinceStartup > this.timeStarted + this.secondsToFadeOut)
		{
			GameManager.Destroy(base.gameObject, 0f);
		}
	}
}