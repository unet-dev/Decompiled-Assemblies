using System;
using UnityEngine;

public class MuzzleFlash_Flamelet : MonoBehaviour
{
	public ParticleSystem flameletParticle;

	public MuzzleFlash_Flamelet()
	{
	}

	private void OnEnable()
	{
		ParticleSystem.ShapeModule shapeModule = this.flameletParticle.shape;
		shapeModule.angle = (float)UnityEngine.Random.Range(6, 13);
		float single = UnityEngine.Random.Range(7f, 9f);
		this.flameletParticle.startSpeed = UnityEngine.Random.Range(2.5f, single);
		this.flameletParticle.startSize = UnityEngine.Random.Range(0.05f, single * 0.015f);
	}
}