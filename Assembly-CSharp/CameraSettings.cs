using ConVar;
using System;
using UnityEngine;

public class CameraSettings : MonoBehaviour, IClientComponent
{
	private Camera cam;

	public CameraSettings()
	{
	}

	private void OnEnable()
	{
		this.cam = base.GetComponent<Camera>();
	}

	private void Update()
	{
		this.cam.farClipPlane = Mathf.Clamp(ConVar.Graphics.drawdistance, 500f, 2500f);
	}
}