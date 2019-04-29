using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(WindZone))]
public class WindZoneExManager : MonoBehaviour
{
	public float maxAccumMain = 4f;

	public float maxAccumTurbulence = 4f;

	public float globalMainScale = 1f;

	public float globalTurbulenceScale = 1f;

	public Transform testPosition;

	public WindZoneExManager()
	{
	}

	private enum TestMode
	{
		Disabled,
		Low
	}
}