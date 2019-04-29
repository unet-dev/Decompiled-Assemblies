using System;
using UnityEngine;

public class ScreenBounce : BaseScreenShake
{
	public AnimationCurve bounceScale;

	public AnimationCurve bounceSpeed;

	public AnimationCurve bounceViewmodel;

	private float bounceTime;

	private Vector3 bounceVelocity = Vector3.zero;

	public ScreenBounce()
	{
	}

	public override void Run(float delta, ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm)
	{
		this.bounceTime = this.bounceTime + Time.deltaTime * this.bounceSpeed.Evaluate(delta);
		float single = this.bounceScale.Evaluate(delta) * 0.1f;
		this.bounceVelocity.x = Mathf.Sin(this.bounceTime * 20f) * single;
		this.bounceVelocity.y = Mathf.Cos(this.bounceTime * 25f) * single;
		this.bounceVelocity.z = 0f;
		Vector3 vector3 = Vector3.zero;
		vector3 = vector3 + (this.bounceVelocity.x * cam.right);
		vector3 = vector3 + (this.bounceVelocity.y * cam.up);
		if (cam)
		{
			cam.position += vector3;
		}
		if (vm)
		{
			ref Vector3 vector3Pointer = ref vm.position;
			vector3Pointer = vector3Pointer + ((vector3 * -1f) * this.bounceViewmodel.Evaluate(delta));
		}
	}

	public override void Setup()
	{
		this.bounceTime = UnityEngine.Random.Range(0f, 1000f);
	}
}