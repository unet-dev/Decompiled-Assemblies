using System;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
	public float Speed = 2f;

	public MoveForward()
	{
	}

	protected void Update()
	{
		base.GetComponent<Rigidbody>().velocity = this.Speed * base.transform.forward;
	}
}