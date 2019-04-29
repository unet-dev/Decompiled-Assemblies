using System;
using UnityEngine;

public class ScreenFov : BaseScreenShake
{
	public AnimationCurve FovAdjustment;

	public ScreenFov()
	{
	}

	public override void Run(float delta, ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm)
	{
		if (cam)
		{
			T t = cam.component;
			t.fieldOfView = t.fieldOfView + this.FovAdjustment.Evaluate(delta);
		}
	}

	public override void Setup()
	{
	}
}