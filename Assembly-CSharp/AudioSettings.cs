using ConVar;
using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSettings : MonoBehaviour
{
	public AudioMixer mixer;

	public AudioSettings()
	{
	}

	private float LinearToDecibel(float linear)
	{
		float single;
		single = (linear <= 0f ? -144f : 20f * Mathf.Log10(linear));
		return single;
	}

	private void Update()
	{
		float single;
		if (this.mixer == null)
		{
			return;
		}
		this.mixer.SetFloat("MasterVol", this.LinearToDecibel(Audio.master));
		this.mixer.GetFloat("MusicVol", out single);
		if (!LevelManager.isLoaded || !MainCamera.isValid)
		{
			this.mixer.SetFloat("MusicVol", Mathf.Lerp(single, this.LinearToDecibel(Audio.musicvolumemenu), UnityEngine.Time.deltaTime));
		}
		else
		{
			this.mixer.SetFloat("MusicVol", Mathf.Lerp(single, this.LinearToDecibel(Audio.musicvolume), UnityEngine.Time.deltaTime));
		}
		this.mixer.SetFloat("WorldVol", this.LinearToDecibel(Audio.game));
		this.mixer.SetFloat("VoiceVol", this.LinearToDecibel(Audio.voices));
	}
}