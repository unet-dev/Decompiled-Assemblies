using System;
using UnityEngine;

public class flamethrowerFire : MonoBehaviour
{
	public ParticleSystem pilotLightFX;

	public ParticleSystem[] flameFX;

	public FlameJet jet;

	public AudioSource oneShotSound;

	public AudioSource loopSound;

	public AudioClip pilotlightIdle;

	public AudioClip flameLoop;

	public AudioClip flameStart;

	public flamethrowerState flameState;

	private flamethrowerState previousflameState;

	public flamethrowerFire()
	{
	}

	public void FlameOn()
	{
		this.pilotLightFX.enableEmission = false;
		this.SetFlameStatus(true);
	}

	public void PilotLightOn()
	{
		this.pilotLightFX.enableEmission = true;
		this.SetFlameStatus(false);
	}

	public void SetFlameStatus(bool status)
	{
		ParticleSystem[] particleSystemArray = this.flameFX;
		for (int i = 0; i < (int)particleSystemArray.Length; i++)
		{
			particleSystemArray[i].enableEmission = status;
		}
	}

	public void ShutOff()
	{
		this.pilotLightFX.enableEmission = false;
		this.SetFlameStatus(false);
	}

	private void Start()
	{
		int num = 0;
		flamethrowerState _flamethrowerState = (flamethrowerState)num;
		this.flameState = (flamethrowerState)num;
		this.previousflameState = _flamethrowerState;
	}

	private void Update()
	{
		if (this.previousflameState != this.flameState)
		{
			switch (this.flameState)
			{
				case flamethrowerState.OFF:
				{
					this.ShutOff();
					break;
				}
				case flamethrowerState.PILOT_LIGHT:
				{
					this.PilotLightOn();
					break;
				}
				case flamethrowerState.FLAME_ON:
				{
					this.FlameOn();
					break;
				}
			}
			this.previousflameState = this.flameState;
			this.jet.SetOn(this.flameState == flamethrowerState.FLAME_ON);
		}
	}
}