using System;
using UnityEngine;

public class LightLOD : MonoBehaviour, ILOD, IClientComponent
{
	public float DistanceBias;

	public bool ToggleLight;

	public bool ToggleShadows = true;

	public LightLOD()
	{
	}

	protected void OnValidate()
	{
		LightEx.CheckConflict(base.gameObject);
	}
}