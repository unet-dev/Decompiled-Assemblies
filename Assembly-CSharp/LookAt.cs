using System;
using UnityEngine;

[ExecuteInEditMode]
public class LookAt : MonoBehaviour, IClientComponent
{
	public Transform target;

	public LookAt()
	{
	}

	private void Update()
	{
		if (this.target == null)
		{
			return;
		}
		base.transform.LookAt(this.target);
	}
}