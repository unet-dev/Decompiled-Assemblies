using System;
using UnityEngine;

public class AIHelicopterAnimation : MonoBehaviour
{
	public PatrolHelicopterAI _ai;

	public float swayAmount = 1f;

	public float lastStrafeScalar;

	public float lastForwardBackScalar;

	public float degreeMax = 90f;

	public Vector3 lastPosition = Vector3.zero;

	public float oldMoveSpeed;

	public float smoothRateOfChange;

	public float flareAmount;

	public AIHelicopterAnimation()
	{
	}

	public void Awake()
	{
		this.lastPosition = base.transform.position;
	}

	public Vector3 GetMoveDirection()
	{
		return this._ai.GetMoveDirection();
	}

	public float GetMoveSpeed()
	{
		return this._ai.GetMoveSpeed();
	}

	public void Update()
	{
		this.lastPosition = base.transform.position;
		Vector3 moveDirection = this.GetMoveDirection();
		float moveSpeed = this.GetMoveSpeed();
		float single = 0.25f + Mathf.Clamp01(moveSpeed / this._ai.maxSpeed) * 0.75f;
		this.smoothRateOfChange = Mathf.Lerp(this.smoothRateOfChange, moveSpeed - this.oldMoveSpeed, Time.deltaTime * 5f);
		this.oldMoveSpeed = moveSpeed;
		float single1 = Vector3.Angle(moveDirection, base.transform.forward);
		float single2 = Vector3.Angle(moveDirection, -base.transform.forward);
		float single3 = 1f - Mathf.Clamp01(single1 / this.degreeMax);
		float single4 = 1f - Mathf.Clamp01(single2 / this.degreeMax);
		float single5 = (single3 - single4) * single;
		float single6 = Mathf.Lerp(this.lastForwardBackScalar, single5, Time.deltaTime * 2f);
		this.lastForwardBackScalar = single6;
		float single7 = Vector3.Angle(moveDirection, base.transform.right);
		float single8 = Vector3.Angle(moveDirection, -base.transform.right);
		float single9 = 1f - Mathf.Clamp01(single7 / this.degreeMax);
		float single10 = 1f - Mathf.Clamp01(single8 / this.degreeMax);
		float single11 = (single9 - single10) * single;
		float single12 = Mathf.Lerp(this.lastStrafeScalar, single11, Time.deltaTime * 2f);
		this.lastStrafeScalar = single12;
		Vector3 vector3 = Vector3.zero;
		ref float singlePointer = ref vector3.x;
		singlePointer = singlePointer + single6 * this.swayAmount;
		ref float singlePointer1 = ref vector3.z;
		singlePointer1 = singlePointer1 - single12 * this.swayAmount;
		Quaternion quaternion = Quaternion.identity;
		quaternion = Quaternion.Euler(vector3.x, vector3.y, vector3.z);
		this._ai.helicopterBase.rotorPivot.transform.localRotation = quaternion;
	}
}