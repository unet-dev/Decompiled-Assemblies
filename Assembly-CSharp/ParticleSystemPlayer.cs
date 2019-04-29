using System;
using UnityEngine;

public class ParticleSystemPlayer : MonoBehaviour, IOnParentDestroying
{
	public ParticleSystemPlayer()
	{
	}

	protected void OnEnable()
	{
		base.GetComponent<ParticleSystem>().enableEmission = true;
	}

	public void OnParentDestroying()
	{
		base.GetComponent<ParticleSystem>().enableEmission = false;
	}
}