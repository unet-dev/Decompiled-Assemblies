using ConVar;
using System;
using UnityEngine;

public static class UISound
{
	private static AudioSource source;

	private static AudioSource GetAudioSource()
	{
		if (UISound.source != null)
		{
			return UISound.source;
		}
		UISound.source = (new GameObject("UISound")).AddComponent<AudioSource>();
		UISound.source.spatialBlend = 0f;
		UISound.source.volume = 1f;
		return UISound.source;
	}

	public static void Play(AudioClip clip, float volume = 1f)
	{
		if (clip == null)
		{
			return;
		}
		UISound.GetAudioSource().volume = volume * Audio.master * 0.4f;
		UISound.GetAudioSource().PlayOneShot(clip);
	}
}