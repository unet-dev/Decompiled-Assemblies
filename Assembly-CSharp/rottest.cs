using System;
using UnityEngine;

public class rottest : MonoBehaviour
{
	public Transform turretBase;

	public Vector3 aimDir;

	public rottest()
	{
	}

	private void Start()
	{
	}

	private void Update()
	{
		this.aimDir = new Vector3(0f, 45f * Mathf.Sin(Time.time * 6f), 0f);
		this.UpdateAiming();
	}

	public void UpdateAiming()
	{
		if (this.aimDir == Vector3.zero)
		{
			return;
		}
		Quaternion quaternion = Quaternion.Euler(0f, this.aimDir.y, 0f);
		if (base.transform.localRotation != quaternion)
		{
			base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, quaternion, Time.deltaTime * 8f);
		}
	}
}