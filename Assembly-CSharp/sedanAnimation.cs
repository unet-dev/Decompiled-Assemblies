using System;
using UnityEngine;

public class sedanAnimation : MonoBehaviour
{
	public Transform[] frontAxles;

	public Transform FL_shock;

	public Transform FL_wheel;

	public Transform FR_shock;

	public Transform FR_wheel;

	public Transform RL_shock;

	public Transform RL_wheel;

	public Transform RR_shock;

	public Transform RR_wheel;

	public WheelCollider FL_wheelCollider;

	public WheelCollider FR_wheelCollider;

	public WheelCollider RL_wheelCollider;

	public WheelCollider RR_wheelCollider;

	public Transform steeringWheel;

	public float motorForceConstant = 150f;

	public float brakeForceConstant = 500f;

	public float brakePedal;

	public float gasPedal;

	public float steering;

	private Rigidbody myRigidbody;

	public float GasLerpTime = 20f;

	public float SteeringLerpTime = 20f;

	private float wheelSpinConstant = 120f;

	private float shockRestingPosY = -0.27f;

	private float shockDistance = 0.3f;

	private float traceDistanceNeutralPoint = 0.7f;

	public sedanAnimation()
	{
	}

	private void ApplyForceAtWheels()
	{
		if (this.FL_wheelCollider.isGrounded)
		{
			this.FL_wheelCollider.motorTorque = this.gasPedal * this.motorForceConstant;
			this.FL_wheelCollider.brakeTorque = this.brakePedal * this.brakeForceConstant;
		}
		if (this.FR_wheelCollider.isGrounded)
		{
			this.FR_wheelCollider.motorTorque = this.gasPedal * this.motorForceConstant;
			this.FR_wheelCollider.brakeTorque = this.brakePedal * this.brakeForceConstant;
		}
		if (this.RL_wheelCollider.isGrounded)
		{
			this.RL_wheelCollider.motorTorque = this.gasPedal * this.motorForceConstant;
			this.RL_wheelCollider.brakeTorque = this.brakePedal * this.brakeForceConstant;
		}
		if (this.RR_wheelCollider.isGrounded)
		{
			this.RR_wheelCollider.motorTorque = this.gasPedal * this.motorForceConstant;
			this.RR_wheelCollider.brakeTorque = this.brakePedal * this.brakeForceConstant;
		}
	}

	private void DoSteering()
	{
		this.FL_wheelCollider.steerAngle = this.steering;
		this.FR_wheelCollider.steerAngle = this.steering;
	}

	private float GetShockHeightDelta(WheelCollider wheel)
	{
		RaycastHit raycastHit;
		int mask = LayerMask.GetMask(new string[] { "Terrain", "World", "Construction" });
		Physics.Linecast(wheel.transform.position, wheel.transform.position - (Vector3.up * 10f), out raycastHit, mask);
		return Mathx.RemapValClamped(raycastHit.distance, this.traceDistanceNeutralPoint - this.shockDistance, this.traceDistanceNeutralPoint + this.shockDistance, this.shockDistance * 0.75f, -0.75f * this.shockDistance);
	}

	private void InputPlayer()
	{
		if (Input.GetKey(KeyCode.W))
		{
			this.gasPedal = Mathf.Clamp(this.gasPedal + Time.deltaTime * this.GasLerpTime, -100f, 100f);
			this.brakePedal = Mathf.Lerp(this.brakePedal, 0f, Time.deltaTime * this.GasLerpTime);
		}
		else if (!Input.GetKey(KeyCode.S))
		{
			this.gasPedal = Mathf.Lerp(this.gasPedal, 0f, Time.deltaTime * this.GasLerpTime);
			this.brakePedal = Mathf.Lerp(this.brakePedal, 100f, Time.deltaTime * this.GasLerpTime / 5f);
		}
		else
		{
			this.gasPedal = Mathf.Clamp(this.gasPedal - Time.deltaTime * this.GasLerpTime, -100f, 100f);
			this.brakePedal = Mathf.Lerp(this.brakePedal, 0f, Time.deltaTime * this.GasLerpTime);
		}
		if (Input.GetKey(KeyCode.A))
		{
			this.steering = Mathf.Clamp(this.steering - Time.deltaTime * this.SteeringLerpTime, -60f, 60f);
			return;
		}
		if (!Input.GetKey(KeyCode.D))
		{
			this.steering = Mathf.Lerp(this.steering, 0f, Time.deltaTime * this.SteeringLerpTime);
			return;
		}
		this.steering = Mathf.Clamp(this.steering + Time.deltaTime * this.SteeringLerpTime, -60f, 60f);
	}

	private void Start()
	{
		this.myRigidbody = base.GetComponent<Rigidbody>();
	}

	private void Update()
	{
		this.DoSteering();
		this.ApplyForceAtWheels();
		this.UpdateTireAnimation();
		this.InputPlayer();
	}

	private void UpdateTireAnimation()
	{
		float single = Vector3.Dot(this.myRigidbody.velocity, this.myRigidbody.transform.forward);
		if (!this.FL_wheelCollider.isGrounded)
		{
			this.FL_shock.localPosition = Vector3.Lerp(this.FL_shock.localPosition, new Vector3(this.FL_shock.localPosition.x, this.shockRestingPosY, this.FL_shock.localPosition.z), Time.deltaTime * 2f);
		}
		else
		{
			this.FL_shock.localPosition = new Vector3(this.FL_shock.localPosition.x, this.shockRestingPosY + this.GetShockHeightDelta(this.FL_wheelCollider), this.FL_shock.localPosition.z);
			this.FL_wheel.localEulerAngles = new Vector3(this.FL_wheel.localEulerAngles.x, this.FL_wheel.localEulerAngles.y, this.FL_wheel.localEulerAngles.z - single * Time.deltaTime * this.wheelSpinConstant);
		}
		if (!this.FR_wheelCollider.isGrounded)
		{
			this.FR_shock.localPosition = Vector3.Lerp(this.FR_shock.localPosition, new Vector3(this.FR_shock.localPosition.x, this.shockRestingPosY, this.FR_shock.localPosition.z), Time.deltaTime * 2f);
		}
		else
		{
			this.FR_shock.localPosition = new Vector3(this.FR_shock.localPosition.x, this.shockRestingPosY + this.GetShockHeightDelta(this.FR_wheelCollider), this.FR_shock.localPosition.z);
			this.FR_wheel.localEulerAngles = new Vector3(this.FR_wheel.localEulerAngles.x, this.FR_wheel.localEulerAngles.y, this.FR_wheel.localEulerAngles.z - single * Time.deltaTime * this.wheelSpinConstant);
		}
		if (!this.RL_wheelCollider.isGrounded)
		{
			this.RL_shock.localPosition = Vector3.Lerp(this.RL_shock.localPosition, new Vector3(this.RL_shock.localPosition.x, this.shockRestingPosY, this.RL_shock.localPosition.z), Time.deltaTime * 2f);
		}
		else
		{
			this.RL_shock.localPosition = new Vector3(this.RL_shock.localPosition.x, this.shockRestingPosY + this.GetShockHeightDelta(this.RL_wheelCollider), this.RL_shock.localPosition.z);
			this.RL_wheel.localEulerAngles = new Vector3(this.RL_wheel.localEulerAngles.x, this.RL_wheel.localEulerAngles.y, this.RL_wheel.localEulerAngles.z - single * Time.deltaTime * this.wheelSpinConstant);
		}
		if (!this.RR_wheelCollider.isGrounded)
		{
			this.RR_shock.localPosition = Vector3.Lerp(this.RR_shock.localPosition, new Vector3(this.RR_shock.localPosition.x, this.shockRestingPosY, this.RR_shock.localPosition.z), Time.deltaTime * 2f);
		}
		else
		{
			this.RR_shock.localPosition = new Vector3(this.RR_shock.localPosition.x, this.shockRestingPosY + this.GetShockHeightDelta(this.RR_wheelCollider), this.RR_shock.localPosition.z);
			this.RR_wheel.localEulerAngles = new Vector3(this.RR_wheel.localEulerAngles.x, this.RR_wheel.localEulerAngles.y, this.RR_wheel.localEulerAngles.z - single * Time.deltaTime * this.wheelSpinConstant);
		}
		Transform[] transformArrays = this.frontAxles;
		for (int i = 0; i < (int)transformArrays.Length; i++)
		{
			Transform vector3 = transformArrays[i];
			vector3.localEulerAngles = new Vector3(this.steering, vector3.localEulerAngles.y, vector3.localEulerAngles.z);
		}
	}
}