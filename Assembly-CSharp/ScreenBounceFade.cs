using System;
using UnityEngine;

public class ScreenBounceFade : BaseScreenShake
{
	public AnimationCurve bounceScale;

	public AnimationCurve bounceSpeed;

	public AnimationCurve bounceViewmodel;

	public AnimationCurve distanceFalloff;

	public AnimationCurve timeFalloff;

	private float bounceTime;

	private Vector3 bounceVelocity = Vector3.zero;

	public float maxDistance = 10f;

	public float scale = 1f;

	public ScreenBounceFade()
	{
	}

	public override void Run(float delta, ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm)
	{
		float single = Vector3.Distance(cam.position, base.transform.position);
		float single1 = 1f - Mathf.InverseLerp(0f, this.maxDistance, single);
		this.bounceTime = this.bounceTime + Time.deltaTime * this.bounceSpeed.Evaluate(delta);
		float single2 = this.distanceFalloff.Evaluate(single1);
		float single3 = this.bounceScale.Evaluate(delta) * 0.1f * single2 * this.scale * this.timeFalloff.Evaluate(delta);
		this.bounceVelocity.x = Mathf.Sin(this.bounceTime * 20f) * single3;
		this.bounceVelocity.y = Mathf.Cos(this.bounceTime * 25f) * single3;
		this.bounceVelocity.z = 0f;
		Vector3 vector3 = Vector3.zero;
		vector3 = vector3 + (this.bounceVelocity.x * cam.right);
		vector3 = vector3 + (this.bounceVelocity.y * cam.up);
		vector3 *= single1;
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