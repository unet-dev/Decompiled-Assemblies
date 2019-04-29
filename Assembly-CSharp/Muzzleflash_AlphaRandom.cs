using System;
using UnityEngine;

public class Muzzleflash_AlphaRandom : MonoBehaviour
{
	public ParticleSystem[] muzzleflashParticles;

	private Gradient grad = new Gradient();

	private GradientColorKey[] gck = new GradientColorKey[3];

	private GradientAlphaKey[] gak = new GradientAlphaKey[3];

	public Muzzleflash_AlphaRandom()
	{
	}

	private void OnEnable()
	{
		this.gck[0].color = Color.white;
		this.gck[0].time = 0f;
		this.gck[1].color = Color.white;
		this.gck[1].time = 0.6f;
		this.gck[2].color = Color.black;
		this.gck[2].time = 0.75f;
		float single = UnityEngine.Random.Range(0.2f, 0.85f);
		this.gak[0].alpha = single;
		this.gak[0].time = 0f;
		this.gak[1].alpha = single;
		this.gak[1].time = 0.45f;
		this.gak[2].alpha = 0f;
		this.gak[2].time = 0.5f;
		this.grad.SetKeys(this.gck, this.gak);
		ParticleSystem[] particleSystemArray = this.muzzleflashParticles;
		for (int i = 0; i < (int)particleSystemArray.Length; i++)
		{
			ParticleSystem particleSystem = particleSystemArray[i];
			if (particleSystem != null)
			{
				particleSystem.colorOverLifetime.color = this.grad;
			}
			else
			{
				Debug.LogWarning(string.Concat("Muzzleflash_AlphaRandom : null particle system in ", base.gameObject.name));
			}
		}
	}

	private void Start()
	{
	}
}