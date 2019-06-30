using System;
using UnityEngine;

public class ChippyMoveTest : MonoBehaviour
{
	public Vector3 heading = new Vector3(0f, 1f, 0f);

	public float speed = 0.2f;

	public float maxSpeed = 1f;

	public ChippyMoveTest()
	{
	}

	private void FixedUpdate()
	{
		float single = (Mathf.Abs(this.heading.magnitude) > 0f ? 1f : 0f);
		this.speed = Mathf.MoveTowards(this.speed, this.maxSpeed * single, Time.fixedDeltaTime * (single == 0f ? 2f : 2f));
		Vector3 vector3 = base.transform.position;
		Vector3 vector31 = new Vector3(this.heading.x, this.heading.y, 0f);
		Ray ray = new Ray(vector3, vector31.normalized);
		if (!Physics.Raycast(ray, this.speed * Time.fixedDeltaTime, 16777216))
		{
			Transform transforms = base.transform;
			transforms.position = transforms.position + ((ray.direction * Time.fixedDeltaTime) * this.speed);
			if (Mathf.Abs(this.heading.magnitude) > 0f)
			{
				Transform transforms1 = base.transform;
				Vector3 vector32 = base.transform.forward;
				vector31 = new Vector3(this.heading.x, this.heading.y, 0f);
				transforms1.rotation = QuaternionEx.LookRotationForcedUp(vector32, vector31.normalized);
			}
		}
	}
}