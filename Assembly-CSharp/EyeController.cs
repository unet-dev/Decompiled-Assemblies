using System;
using UnityEngine;

public class EyeController : MonoBehaviour
{
	public const float MaxLookDot = 0.8f;

	public bool debug;

	public Transform LeftEye;

	public Transform RightEye;

	public Transform EyeTransform;

	public Vector3 Fudge = new Vector3(0f, 90f, 0f);

	public Vector3 FlickerRange;

	private Transform Focus;

	private float FocusUpdateTime;

	private Vector3 Flicker;

	private Vector3 FlickerTarget;

	private float TimeToUpdateFlicker;

	private float FlickerSpeed;

	public EyeController()
	{
	}

	private void UpdateEye(Transform eye, Vector3 LookAt)
	{
		Vector3 lookAt = LookAt - eye.position;
		eye.rotation = (Quaternion.LookRotation(lookAt.normalized, this.EyeTransform.up) * Quaternion.Euler(this.Fudge)) * Quaternion.Euler(this.Flicker);
	}

	public void UpdateEyes()
	{
		Vector3 eyeTransform = this.EyeTransform.position + (this.EyeTransform.forward * 100f);
		Vector3 focus = eyeTransform;
		this.UpdateFocus(eyeTransform);
		this.UpdateFlicker();
		if (this.Focus != null)
		{
			focus = this.Focus.position;
			Vector3 vector3 = this.EyeTransform.position - eyeTransform;
			Vector3 eyeTransform1 = this.EyeTransform.position - focus;
			if (Vector3.Dot(vector3.normalized, eyeTransform1.normalized) < 0.8f)
			{
				this.Focus = null;
			}
		}
		this.UpdateEye(this.LeftEye, focus);
		this.UpdateEye(this.RightEye, focus);
	}

	private void UpdateFlicker()
	{
		this.TimeToUpdateFlicker -= Time.deltaTime;
		this.Flicker = Vector3.Lerp(this.Flicker, this.FlickerTarget, Time.deltaTime * this.FlickerSpeed);
		if (this.TimeToUpdateFlicker < 0f)
		{
			this.TimeToUpdateFlicker = UnityEngine.Random.Range(0.2f, 2f);
			this.FlickerTarget = new Vector3(UnityEngine.Random.Range(-this.FlickerRange.x, this.FlickerRange.x), UnityEngine.Random.Range(-this.FlickerRange.y, this.FlickerRange.y), UnityEngine.Random.Range(-this.FlickerRange.z, this.FlickerRange.z)) * (this.Focus ? 0.01f : 1f);
			this.FlickerSpeed = UnityEngine.Random.Range(10f, 30f);
		}
	}

	private void UpdateFocus(Vector3 defaultLookAtPos)
	{
	}
}