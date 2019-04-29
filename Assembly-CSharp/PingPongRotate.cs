using System;
using UnityEngine;

public class PingPongRotate : MonoBehaviour
{
	public Vector3 rotationSpeed = Vector3.zero;

	public Vector3 offset = Vector3.zero;

	public Vector3 rotationAmount = Vector3.zero;

	public PingPongRotate()
	{
	}

	public Quaternion GetRotation(int index)
	{
		Vector3 vector3 = Vector3.zero;
		if (index == 0)
		{
			vector3 = Vector3.right;
		}
		else if (index == 1)
		{
			vector3 = Vector3.up;
		}
		else if (index == 2)
		{
			vector3 = Vector3.forward;
		}
		return Quaternion.AngleAxis(Mathf.Sin((this.offset[index] + Time.time) * this.rotationSpeed[index]) * this.rotationAmount[index], vector3);
	}

	private void Update()
	{
		Quaternion rotation = Quaternion.identity;
		for (int i = 0; i < 3; i++)
		{
			rotation *= this.GetRotation(i);
		}
		base.transform.rotation = rotation;
	}
}