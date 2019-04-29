using System;
using UnityEngine;

public class ScaleByIntensity : MonoBehaviour
{
	public Vector3 initialScale = Vector3.zero;

	public Light intensitySource;

	public float maxIntensity = 1f;

	public ScaleByIntensity()
	{
	}

	private void Start()
	{
		this.initialScale = base.transform.localScale;
	}

	private void Update()
	{
		base.transform.localScale = (this.intensitySource.enabled ? (this.initialScale * this.intensitySource.intensity) / this.maxIntensity : Vector3.zero);
	}
}