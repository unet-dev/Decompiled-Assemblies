using System;
using UnityEngine;

public class ParticleRandomLifetime : MonoBehaviour
{
	public ParticleSystem mySystem;

	public float minScale = 0.5f;

	public float maxScale = 1f;

	public ParticleRandomLifetime()
	{
	}

	public void Awake()
	{
		if (!this.mySystem)
		{
			return;
		}
		float single = UnityEngine.Random.Range(this.minScale, this.maxScale);
		this.mySystem.startLifetime = single;
	}
}