using ConVar;
using System;
using UnityEngine;

public class SunSettings : MonoBehaviour, IClientComponent
{
	private Light light;

	public SunSettings()
	{
	}

	private void OnEnable()
	{
		this.light = base.GetComponent<Light>();
	}

	private void Update()
	{
		LightShadows lightShadow = (LightShadows)Mathf.Clamp(ConVar.Graphics.shadowmode, 1, 2);
		if (this.light.shadows != lightShadow)
		{
			this.light.shadows = lightShadow;
		}
	}
}