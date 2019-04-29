using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MusicClipLoader
{
	public List<MusicClipLoader.LoadedAudioClip> loadedClips = new List<MusicClipLoader.LoadedAudioClip>();

	public Dictionary<AudioClip, MusicClipLoader.LoadedAudioClip> loadedClipDict = new Dictionary<AudioClip, MusicClipLoader.LoadedAudioClip>();

	public List<AudioClip> clipsToLoad = new List<AudioClip>();

	public List<AudioClip> clipsToUnload = new List<AudioClip>();

	public MusicClipLoader()
	{
	}

	private MusicClipLoader.LoadedAudioClip FindLoadedClip(AudioClip clip)
	{
		if (!this.loadedClipDict.ContainsKey(clip))
		{
			return null;
		}
		return this.loadedClipDict[clip];
	}

	public void Refresh()
	{
		for (int i = 0; i < SingletonComponent<MusicManager>.Instance.activeMusicClips.Count; i++)
		{
			MusicTheme.PositionedClip item = SingletonComponent<MusicManager>.Instance.activeMusicClips[i];
			MusicClipLoader.LoadedAudioClip loadedAudioClip = this.FindLoadedClip(item.musicClip.audioClip);
			if (loadedAudioClip != null)
			{
				loadedAudioClip.unloadTime = (float)UnityEngine.AudioSettings.dspTime + loadedAudioClip.clip.length + 1f;
				this.clipsToUnload.Remove(loadedAudioClip.clip);
			}
			else
			{
				loadedAudioClip = Pool.Get<MusicClipLoader.LoadedAudioClip>();
				loadedAudioClip.clip = item.musicClip.audioClip;
				loadedAudioClip.unloadTime = (float)UnityEngine.AudioSettings.dspTime + loadedAudioClip.clip.length + 1f;
				this.loadedClips.Add(loadedAudioClip);
				this.loadedClipDict.Add(loadedAudioClip.clip, loadedAudioClip);
				this.clipsToLoad.Add(loadedAudioClip.clip);
			}
		}
		for (int j = this.loadedClips.Count - 1; j >= 0; j--)
		{
			MusicClipLoader.LoadedAudioClip item1 = this.loadedClips[j];
			if (UnityEngine.AudioSettings.dspTime > (double)item1.unloadTime)
			{
				this.clipsToUnload.Add(item1.clip);
				this.loadedClips.Remove(item1);
				this.loadedClipDict.Remove(item1.clip);
				Pool.Free<MusicClipLoader.LoadedAudioClip>(ref item1);
			}
		}
	}

	public void Update()
	{
		for (int i = this.clipsToLoad.Count - 1; i >= 0; i--)
		{
			AudioClip item = this.clipsToLoad[i];
			if (item.loadState != AudioDataLoadState.Loaded && item.loadState != AudioDataLoadState.Loading)
			{
				item.LoadAudioData();
				this.clipsToLoad.RemoveAt(i);
				return;
			}
		}
		for (int j = this.clipsToUnload.Count - 1; j >= 0; j--)
		{
			AudioClip audioClip = this.clipsToUnload[j];
			if (audioClip.loadState == AudioDataLoadState.Loaded)
			{
				audioClip.UnloadAudioData();
				this.clipsToUnload.RemoveAt(j);
				return;
			}
		}
	}

	public class LoadedAudioClip
	{
		public AudioClip clip;

		public float unloadTime;

		public LoadedAudioClip()
		{
		}
	}
}