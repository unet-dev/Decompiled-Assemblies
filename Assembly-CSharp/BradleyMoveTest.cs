using System;
using UnityEngine;

public class BradleyMoveTest : MonoBehaviour
{
	public WheelCollider[] leftWheels;

	public WheelCollider[] rightWheels;

	public float moveForceMax = 2000f;

	public float brakeForce = 100f;

	public float throttle = 1f;

	public float turnForce = 2000f;

	public float sideStiffnessMax = 1f;

	public float sideStiffnessMin = 0.5f;

	public Transform centerOfMass;

	public float turning;

	public bool brake;

	public Rigidbody myRigidBody;

	public Vector3 destination;

	public float stoppingDist = 5f;

	public GameObject followTest;

	public BradleyMoveTest()
	{
	}

	public void AdjustFriction()
	{
	}

	public void ApplyBrakes(float amount)
	{
		this.ApplyBrakeTorque(amount, true);
		this.ApplyBrakeTorque(amount, false);
	}

	public void ApplyBrakeTorque(float amount, bool rightSide)
	{
		WheelCollider[] wheelColliderArray = (rightSide ? this.rightWheels : this.leftWheels);
		for (int i = 0; i < (int)wheelColliderArray.Length; i++)
		{
			wheelColliderArray[i].brakeTorque = this.brakeForce * amount;
		}
	}

	public void Awake()
	{
		this.Initialize();
	}

	public void FixedUpdate()
	{
		Vector3 vector3 = this.myRigidBody.velocity;
		this.SetDestination(this.followTest.transform.position);
		float single = Vector3.Distance(base.transform.position, this.destination);
		if (single > this.stoppingDist)
		{
			Vector3 vector31 = Vector3.zero;
			float single1 = Vector3.Dot(vector31, base.transform.right);
			float single2 = Vector3.Dot(vector31, -base.transform.right);
			float single3 = Vector3.Dot(vector31, base.transform.right);
			if (Vector3.Dot(vector31, -base.transform.forward) <= single3)
			{
				this.turning = single3;
			}
			else if (single1 < single2)
			{
				this.turning = -1f;
			}
			else
			{
				this.turning = 1f;
			}
			this.throttle = Mathf.InverseLerp(this.stoppingDist, 30f, single);
		}
		this.throttle = Mathf.Clamp(this.throttle, -1f, 1f);
		float single4 = this.throttle;
		float single5 = this.throttle;
		if (this.turning > 0f)
		{
			single5 = -this.turning;
			single4 = this.turning;
		}
		else if (this.turning < 0f)
		{
			single4 = this.turning;
			single5 = this.turning * -1f;
		}
		this.ApplyBrakes((this.brake ? 1f : 0f));
		float single6 = this.throttle;
		single4 = Mathf.Clamp(single4 + single6, -1f, 1f);
		single5 = Mathf.Clamp(single5 + single6, -1f, 1f);
		this.AdjustFriction();
		float single7 = Mathf.InverseLerp(3f, 1f, vector3.magnitude * Mathf.Abs(Vector3.Dot(vector3.normalized, base.transform.forward)));
		float single8 = Mathf.Lerp(this.moveForceMax, this.turnForce, single7);
		this.SetMotorTorque(single4, false, single8);
		this.SetMotorTorque(single5, true, single8);
	}

	public float GetMotorTorque(bool rightSide)
	{
		float length = 0f;
		WheelCollider[] wheelColliderArray = (rightSide ? this.rightWheels : this.leftWheels);
		for (int i = 0; i < (int)wheelColliderArray.Length; i++)
		{
			length += wheelColliderArray[i].motorTorque;
		}
		length /= (float)((int)this.rightWheels.Length);
		return length;
	}

	public void Initialize()
	{
		this.myRigidBody.centerOfMass = this.centerOfMass.localPosition;
		this.destination = base.transform.position;
	}

	public void SetDestination(Vector3 dest)
	{
		this.destination = dest;
	}

	public void SetMotorTorque(float newThrottle, bool rightSide, float torqueAmount)
	{
		newThrottle = Mathf.Clamp(newThrottle, -1f, 1f);
		float single = torqueAmount * newThrottle;
		WheelCollider[] wheelColliderArray = (rightSide ? this.rightWheels : this.leftWheels);
		for (int i = 0; i < (int)wheelColliderArray.Length; i++)
		{
			wheelColliderArray[i].motorTorque = single;
		}
	}
}