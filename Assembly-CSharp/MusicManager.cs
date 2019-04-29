using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : SingletonComponent<MusicManager>, IClientComponent
{
	public AudioMixerGroup mixerGroup;

	public List<MusicTheme> themes;

	public MusicTheme currentTheme;

	public List<AudioSource> sources = new List<AudioSource>();

	public double nextMusic;

	public double nextMusicFromIntensityRaise;

	[Range(0f, 1f)]
	public float intensity;

	public Dictionary<MusicTheme.PositionedClip, MusicManager.ClipPlaybackData> clipPlaybackData = new Dictionary<MusicTheme.PositionedClip, MusicManager.ClipPlaybackData>();

	public int holdIntensityUntilBar;

	public bool musicPlaying;

	public bool loadingFirstClips;

	public MusicTheme nextTheme;

	public double lastClipUpdate;

	public float clipUpdateInterval = 0.1f;

	public double themeStartTime;

	public int lastActiveClipRefresh = -10;

	public int activeClipRefreshInterval = 1;

	public bool forceThemeChange;

	public float randomIntensityJumpChance;

	public int clipScheduleBarsEarly = 1;

	public List<MusicTheme.PositionedClip> activeClips = new List<MusicTheme.PositionedClip>();

	public List<MusicTheme.PositionedClip> activeMusicClips = new List<MusicTheme.PositionedClip>();

	public List<MusicTheme.PositionedClip> activeControlClips = new List<MusicTheme.PositionedClip>();

	public List<MusicZone> currentMusicZones = new List<MusicZone>();

	public int currentBar;

	public int barOffset;

	public double currentThemeTime
	{
		get
		{
			return UnityEngine.AudioSettings.dspTime - this.themeStartTime;
		}
	}

	public int themeBar
	{
		get
		{
			return this.currentBar + this.barOffset;
		}
	}

	public MusicManager()
	{
	}

	public static void RaiseIntensityTo(float amount, int holdLengthBars = 0)
	{
	}

	public void StopMusic()
	{
	}

	[Serializable]
	public class ClipPlaybackData
	{
		public AudioSource source;

		public MusicTheme.PositionedClip positionedClip;

		public bool isActive;

		public bool fadingIn;

		public bool fadingOut;

		public double fadeStarted;

		public bool needsSync;

		public ClipPlaybackData()
		{
		}
	}
}