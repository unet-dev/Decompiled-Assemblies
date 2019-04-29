using System;
using UnityEngine;

public class ch47Animator : MonoBehaviour
{
	public Animator animator;

	public bool bottomDoorOpen;

	public bool landingGearDown;

	public bool leftDoorOpen;

	public bool rightDoorOpen;

	public bool rearDoorOpen;

	public bool rearDoorExtensionOpen;

	public Transform rearRotorBlade;

	public Transform frontRotorBlade;

	public float rotorBladeSpeed;

	public float wheelTurnSpeed;

	public float wheelTurnAngle;

	public SkinnedMeshRenderer[] blurredRotorBlades;

	public SkinnedMeshRenderer[] RotorBlades;

	private bool blurredRotorBladesEnabled;

	public float blurSpeedThreshold = 100f;

	public ch47Animator()
	{
	}

	private void EnableBlurredRotorBlades(bool enabled)
	{
		int i;
		this.blurredRotorBladesEnabled = enabled;
		SkinnedMeshRenderer[] rotorBlades = this.blurredRotorBlades;
		for (i = 0; i < (int)rotorBlades.Length; i++)
		{
			rotorBlades[i].enabled = enabled;
		}
		rotorBlades = this.RotorBlades;
		for (i = 0; i < (int)rotorBlades.Length; i++)
		{
			rotorBlades[i].enabled = !enabled;
		}
	}

	private void LateUpdate()
	{
		float single = Time.deltaTime * this.rotorBladeSpeed * 15f;
		Vector3 vector3 = this.frontRotorBlade.localEulerAngles;
		this.frontRotorBlade.localEulerAngles = new Vector3(vector3.x, vector3.y + single, vector3.z);
		vector3 = this.rearRotorBlade.localEulerAngles;
		this.rearRotorBlade.localEulerAngles = new Vector3(vector3.x, vector3.y - single, vector3.z);
	}

	public void SetDropDoorOpen(bool isOpen)
	{
		this.bottomDoorOpen = isOpen;
	}

	private void Start()
	{
		this.EnableBlurredRotorBlades(false);
		this.animator.SetBool("rotorblade_stop", false);
	}

	private void Update()
	{
		this.animator.SetBool("bottomdoor", this.bottomDoorOpen);
		this.animator.SetBool("landinggear", this.landingGearDown);
		this.animator.SetBool("leftdoor", this.leftDoorOpen);
		this.animator.SetBool("rightdoor", this.rightDoorOpen);
		this.animator.SetBool("reardoor", this.rearDoorOpen);
		this.animator.SetBool("reardoor_extension", this.rearDoorExtensionOpen);
		if (this.rotorBladeSpeed >= this.blurSpeedThreshold && !this.blurredRotorBladesEnabled)
		{
			this.EnableBlurredRotorBlades(true);
		}
		else if (this.rotorBladeSpeed < this.blurSpeedThreshold && this.blurredRotorBladesEnabled)
		{
			this.EnableBlurredRotorBlades(false);
		}
		if (this.rotorBladeSpeed <= 0f)
		{
			this.animator.SetBool("rotorblade_stop", true);
			return;
		}
		this.animator.SetBool("rotorblade_stop", false);
	}
}