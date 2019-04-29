using System;
using UnityEngine;

public class Sound : MonoBehaviour, IClientComponent
{
	public static float volumeExponent;

	public SoundDefinition definition;

	public SoundModifier[] modifiers;

	public SoundSource soundSource;

	public AudioSource[] audioSources = new AudioSource[2];

	[SerializeField]
	private SoundFade _fade;

	[SerializeField]
	private SoundModulation _modulation;

	[SerializeField]
	private SoundOcclusion _occlusion;

	public SoundFade fade
	{
		get
		{
			return this._fade;
		}
	}

	public SoundModulation modulation
	{
		get
		{
			return this._modulation;
		}
	}

	public SoundOcclusion occlusion
	{
		get
		{
			return this._occlusion;
		}
	}

	static Sound()
	{
		Sound.volumeExponent = Mathf.Log(Mathf.Sqrt(10f), 2f);
	}

	public Sound()
	{
	}
}