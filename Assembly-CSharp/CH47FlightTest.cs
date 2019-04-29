using System;
using UnityEngine;

public class CH47FlightTest : MonoBehaviour
{
	public Rigidbody rigidBody;

	public float engineThrustMax;

	public Vector3 torqueScale;

	public Transform com;

	public Transform[] GroundPoints;

	public Transform[] GroundEffects;

	public Transform AIMoveTarget;

	private static float altitudeTolerance;

	public float currentThrottle;

	public float avgThrust;

	public float liftDotMax = 0.75f;

	static CH47FlightTest()
	{
		CH47FlightTest.altitudeTolerance = 1f;
	}

	public CH47FlightTest()
	{
	}

	public void Awake()
	{
		this.rigidBody.centerOfMass = this.com.localPosition;
	}

	public void FixedUpdate()
	{
		RaycastHit raycastHit;
		CH47FlightTest.HelicopterInputState_t aIInputState = this.GetAIInputState();
		this.currentThrottle = Mathf.Lerp(this.currentThrottle, aIInputState.throttle, 2f * Time.fixedDeltaTime);
		this.currentThrottle = Mathf.Clamp(this.currentThrottle, -0.2f, 1f);
		this.rigidBody.AddRelativeTorque(new Vector3(aIInputState.pitch * this.torqueScale.x, aIInputState.yaw * this.torqueScale.y, aIInputState.roll * this.torqueScale.z) * Time.fixedDeltaTime, ForceMode.Force);
		this.avgThrust = Mathf.Lerp(this.avgThrust, this.engineThrustMax * this.currentThrottle, Time.fixedDeltaTime);
		float single = Mathf.Clamp01(Vector3.Dot(base.transform.up, Vector3.up));
		float single1 = Mathf.InverseLerp(this.liftDotMax, 1f, single);
		Vector3 vector3 = (((Vector3.up * this.engineThrustMax) * 0.5f) * this.currentThrottle) * single1;
		Vector3 vector31 = base.transform.up - Vector3.up;
		Vector3 vector32 = ((vector31.normalized * this.engineThrustMax) * this.currentThrottle) * (1f - single1);
		float single2 = this.rigidBody.mass * -Physics.gravity.y;
		this.rigidBody.AddForce(((base.transform.up * single2) * single1) * 0.99f, ForceMode.Force);
		this.rigidBody.AddForce(vector3, ForceMode.Force);
		this.rigidBody.AddForce(vector32, ForceMode.Force);
		for (int i = 0; i < (int)this.GroundEffects.Length; i++)
		{
			Transform groundPoints = this.GroundPoints[i];
			Transform groundEffects = this.GroundEffects[i];
			if (!Physics.Raycast(groundPoints.transform.position, Vector3.down, out raycastHit, 50f, 8388608))
			{
				groundEffects.gameObject.SetActive(false);
			}
			else
			{
				groundEffects.gameObject.SetActive(true);
				groundEffects.transform.position = raycastHit.point + new Vector3(0f, 1f, 0f);
			}
		}
	}

	public CH47FlightTest.HelicopterInputState_t GetAIInputState()
	{
		CH47FlightTest.HelicopterInputState_t helicopterInputStateT = new CH47FlightTest.HelicopterInputState_t();
		Vector3 vector3 = Vector3.Cross(Vector3.up, base.transform.right);
		float single = Vector3.Dot(Vector3.Cross(Vector3.up, vector3), Vector3Ex.Direction2D(this.AIMoveTarget.position, base.transform.position));
		helicopterInputStateT.yaw = (single < 0f ? 1f : 0f);
		ref float singlePointer = ref helicopterInputStateT.yaw;
		singlePointer = singlePointer - (single > 0f ? 1f : 0f);
		float single1 = Vector3.Dot(Vector3.up, base.transform.right);
		helicopterInputStateT.roll = (single1 < 0f ? 1f : 0f);
		ref float singlePointer1 = ref helicopterInputStateT.roll;
		singlePointer1 = singlePointer1 - (single1 > 0f ? 1f : 0f);
		float single2 = Vector3Ex.Distance2D(base.transform.position, this.AIMoveTarget.position);
		float single3 = Vector3.Dot(vector3, Vector3Ex.Direction2D(this.AIMoveTarget.position, base.transform.position));
		float single4 = Vector3.Dot(Vector3.up, base.transform.forward);
		if (single2 > 10f)
		{
			helicopterInputStateT.pitch = (single3 > 0.8f ? -0.25f : 0f);
			ref float singlePointer2 = ref helicopterInputStateT.pitch;
			singlePointer2 = singlePointer2 - (single3 < -0.8f ? -0.25f : 0f);
			if (single4 < -0.35f)
			{
				helicopterInputStateT.pitch = -1f;
			}
			else if (single4 > 0.35f)
			{
				helicopterInputStateT.pitch = 1f;
			}
		}
		else if (single4 < 0f)
		{
			helicopterInputStateT.pitch = -1f;
		}
		else if (single4 > 0f)
		{
			helicopterInputStateT.pitch = 1f;
		}
		float idealAltitude = this.GetIdealAltitude();
		float single5 = base.transform.position.y;
		float single6 = 0f;
		if (single5 > idealAltitude + CH47FlightTest.altitudeTolerance)
		{
			single6 = -1f;
		}
		else if (single5 >= idealAltitude - CH47FlightTest.altitudeTolerance)
		{
			single6 = (single2 <= 20f ? 0f : Mathf.Lerp(0f, 1f, single2 / 20f));
		}
		else
		{
			single6 = 1f;
		}
		Debug.Log(string.Concat("desiredThrottle : ", single6));
		helicopterInputStateT.throttle = single6 * 1f;
		return helicopterInputStateT;
	}

	public CH47FlightTest.HelicopterInputState_t GetHelicopterInputState()
	{
		CH47FlightTest.HelicopterInputState_t axis = new CH47FlightTest.HelicopterInputState_t()
		{
			throttle = (Input.GetKey(KeyCode.W) ? 1f : 0f)
		};
		ref float singlePointer = ref axis.throttle;
		singlePointer = singlePointer - (Input.GetKey(KeyCode.S) ? 1f : 0f);
		axis.pitch = Input.GetAxis("Mouse Y");
		axis.roll = -Input.GetAxis("Mouse X");
		axis.yaw = (Input.GetKey(KeyCode.D) ? 1f : 0f);
		ref float singlePointer1 = ref axis.yaw;
		singlePointer1 = singlePointer1 - (Input.GetKey(KeyCode.A) ? 1f : 0f);
		axis.pitch = (float)Mathf.RoundToInt(axis.pitch);
		axis.roll = (float)Mathf.RoundToInt(axis.roll);
		return axis;
	}

	public float GetIdealAltitude()
	{
		return this.AIMoveTarget.transform.position.y;
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(this.AIMoveTarget.transform.position, 1f);
		Vector3 vector3 = Vector3.Cross(base.transform.right, Vector3.up);
		Vector3 vector31 = Vector3.Cross(vector3, Vector3.up);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(base.transform.position, base.transform.position + (vector3 * 10f));
		Gizmos.color = Color.red;
		Gizmos.DrawLine(base.transform.position, base.transform.position + (vector31 * 10f));
	}

	public struct HelicopterInputState_t
	{
		public float throttle;

		public float roll;

		public float yaw;

		public float pitch;
	}
}