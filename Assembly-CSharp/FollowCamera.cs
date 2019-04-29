using System;
using UnityEngine;

public class FollowCamera : MonoBehaviour, IClientComponent
{
	public FollowCamera()
	{
	}

	private void LateUpdate()
	{
		if (MainCamera.mainCamera == null)
		{
			return;
		}
		base.transform.position = MainCamera.position;
	}
}