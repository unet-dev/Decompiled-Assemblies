using System;
using UnityEngine;

public class PlacementTest : MonoBehaviour
{
	public MeshCollider myMeshCollider;

	public Transform testTransform;

	public Transform visualTest;

	public float hemisphere = 45f;

	public float clampTest = 45f;

	public float testDist = 2f;

	private float nextTest;

	public PlacementTest()
	{
	}

	public Vector3 ClampToHemisphere(Vector3 hemiInput, float degreesOffset, Vector3 inputVec)
	{
		degreesOffset = Mathf.Clamp(degreesOffset / 180f, -180f, 180f);
		Vector3 vector3 = hemiInput + (Vector3.one * degreesOffset);
		Vector3 vector31 = vector3.normalized;
		vector3 = hemiInput + (Vector3.one * -degreesOffset);
		Vector3 vector32 = vector3.normalized;
		for (int i = 0; i < 3; i++)
		{
			inputVec[i] = Mathf.Clamp(inputVec[i], vector32[i], vector31[i]);
		}
		return inputVec.normalized;
	}

	public void OnDrawGizmos()
	{
	}

	public Vector3 RandomCylinderPointAroundVector(Vector3 input, float distance, float minHeight = 0f, float maxHeight = 0f)
	{
		Vector2 vector2 = UnityEngine.Random.insideUnitCircle;
		Vector3 vector3 = new Vector3(vector2.x, 0f, vector2.y);
		Vector3 vector31 = vector3.normalized;
		return new Vector3(vector31.x * distance, UnityEngine.Random.Range(minHeight, maxHeight), vector31.z * distance);
	}

	public Vector3 RandomHemisphereDirection(Vector3 input, float degreesOffset)
	{
		degreesOffset = Mathf.Clamp(degreesOffset / 180f, -180f, 180f);
		Vector2 vector2 = UnityEngine.Random.insideUnitCircle;
		Vector3 vector3 = new Vector3(vector2.x * degreesOffset, UnityEngine.Random.Range(-1f, 1f) * degreesOffset, vector2.y * degreesOffset);
		return (input + vector3).normalized;
	}

	private void Update()
	{
		RaycastHit raycastHit;
		if (Time.realtimeSinceStartup < this.nextTest)
		{
			return;
		}
		this.nextTest = Time.realtimeSinceStartup + 0f;
		Vector3 vector3 = this.RandomCylinderPointAroundVector(Vector3.up, 0.5f, 0.25f, 0.5f);
		vector3 = base.transform.TransformPoint(vector3);
		this.testTransform.transform.position = vector3;
		if (this.testTransform != null && this.visualTest != null)
		{
			Vector3 vector31 = base.transform.position;
			if (!this.myMeshCollider.Raycast(new Ray(this.testTransform.position, (base.transform.position - this.testTransform.position).normalized), out raycastHit, 5f))
			{
				Debug.LogError("Missed");
			}
			else
			{
				vector31 = raycastHit.point;
			}
			this.visualTest.transform.position = vector31;
		}
	}
}