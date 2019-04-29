using System;
using UnityEngine;

public class ScreenRotate : BaseScreenShake
{
	public AnimationCurve Pitch;

	public AnimationCurve Yaw;

	public AnimationCurve Roll;

	public AnimationCurve ViewmodelEffect;

	public bool useViewModelEffect = true;

	public ScreenRotate()
	{
	}

	public override void Run(float delta, ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm)
	{
		Vector3 vector3 = Vector3.zero;
		vector3.x = this.Pitch.Evaluate(delta);
		vector3.y = this.Yaw.Evaluate(delta);
		vector3.z = this.Roll.Evaluate(delta);
		if (cam)
		{
			cam.rotation *= Quaternion.Euler(vector3);
		}
		if (vm && this.useViewModelEffect)
		{
			vm.rotation *= Quaternion.Euler((vector3 * -1f) * (1f - this.ViewmodelEffect.Evaluate(delta)));
		}
	}

	public override void Setup()
	{
	}
}