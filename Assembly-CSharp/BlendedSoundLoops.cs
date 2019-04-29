using System;
using System.Collections.Generic;
using UnityEngine;

public class BlendedSoundLoops : MonoBehaviour, IClientComponent
{
	[Range(0f, 1f)]
	public float blend;

	public float blendSmoothing = 1f;

	public float loopFadeOutTime = 0.5f;

	public float loopFadeInTime = 0.5f;

	public float gainModSmoothing = 1f;

	public float pitchModSmoothing = 1f;

	public bool shouldPlay = true;

	public List<BlendedSoundLoops.Loop> loops = new List<BlendedSoundLoops.Loop>();

	public float maxDistance;

	public BlendedSoundLoops()
	{
	}

	private void OnValidate()
	{
		this.maxDistance = 0f;
		foreach (BlendedSoundLoops.Loop loop in this.loops)
		{
			if (loop.soundDef.maxDistance <= this.maxDistance)
			{
				continue;
			}
			this.maxDistance = loop.soundDef.maxDistance;
		}
	}

	[Serializable]
	public class Loop
	{
		public SoundDefinition soundDef;

		public AnimationCurve gainCurve;

		public AnimationCurve pitchCurve;

		[HideInInspector]
		public Sound sound;

		[HideInInspector]
		public SoundModulation.Modulator gainMod;

		[HideInInspector]
		public SoundModulation.Modulator pitchMod;

		public Loop()
		{
		}
	}
}