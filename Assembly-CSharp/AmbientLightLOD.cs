using System;
using UnityEngine;

public class AmbientLightLOD : FacepunchBehaviour, ILOD, IClientComponent
{
	public bool isDynamic;

	public float enabledRadius = 20f;

	public bool toggleFade;

	public float toggleFadeDuration = 0.5f;

	public bool StickyGizmos;

	public AmbientLightLOD()
	{
	}

	protected void OnValidate()
	{
		LightEx.CheckConflict(base.gameObject);
	}
}