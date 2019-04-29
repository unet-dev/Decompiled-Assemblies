using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseScreenShake : MonoBehaviour
{
	public static List<BaseScreenShake> list;

	public float length = 2f;

	internal float timeTaken;

	private int currentFrame = -1;

	static BaseScreenShake()
	{
		BaseScreenShake.list = new List<BaseScreenShake>();
	}

	protected BaseScreenShake()
	{
	}

	public static void Apply(Camera cam, BaseViewModel vm)
	{
		CachedTransform<Camera> cachedTransform = new CachedTransform<Camera>(cam);
		CachedTransform<BaseViewModel> cachedTransform1 = new CachedTransform<BaseViewModel>(vm);
		for (int i = 0; i < BaseScreenShake.list.Count; i++)
		{
			BaseScreenShake.list[i].Run(ref cachedTransform, ref cachedTransform1);
		}
		cachedTransform.Apply();
		cachedTransform1.Apply();
	}

	protected void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		BaseScreenShake.list.Remove(this);
	}

	protected void OnEnable()
	{
		BaseScreenShake.list.Add(this);
		this.timeTaken = 0f;
		this.Setup();
	}

	public void Run(ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm)
	{
		if (this.timeTaken > this.length)
		{
			return;
		}
		if (Time.frameCount != this.currentFrame)
		{
			this.timeTaken += Time.deltaTime;
			this.currentFrame = Time.frameCount;
		}
		float single = Mathf.InverseLerp(0f, this.length, this.timeTaken);
		this.Run(single, ref cam, ref vm);
	}

	public abstract void Run(float delta, ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm);

	public abstract void Setup();
}