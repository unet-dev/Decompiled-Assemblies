using System;
using UnityEngine;

public class RotateCameraAroundObject : MonoBehaviour
{
	public GameObject m_goObjectToRotateAround;

	public float m_flRotateSpeed = 10f;

	public RotateCameraAroundObject()
	{
	}

	private void FixedUpdate()
	{
		if (this.m_goObjectToRotateAround != null)
		{
			base.transform.LookAt(this.m_goObjectToRotateAround.transform.position + (Vector3.up * 0.75f));
			base.transform.Translate((Vector3.right * this.m_flRotateSpeed) * Time.deltaTime);
		}
	}
}