using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Rust/MusicTheme")]
public class MusicTheme : ScriptableObject
{
	[Header("Basic info")]
	public float tempo = 80f;

	public int intensityHoldBars = 4;

	public int lengthInBars;

	[Header("Playback restrictions")]
	public bool canPlayInMenus = true;

	[Horizontal(2, -1)]
	public MusicTheme.ValueRange rain = new MusicTheme.ValueRange(0f, 1f);

	[Horizontal(2, -1)]
	public MusicTheme.ValueRange wind = new MusicTheme.ValueRange(0f, 1f);

	[Horizontal(2, -1)]
	public MusicTheme.ValueRange snow = new MusicTheme.ValueRange(0f, 1f);

	[InspectorFlags]
	public TerrainBiome.Enum biomes = TerrainBiome.Enum.Arid | TerrainBiome.Enum.Temperate | TerrainBiome.Enum.Tundra | TerrainBiome.Enum.Arctic;

	[InspectorFlags]
	public TerrainTopology.Enum topologies = TerrainTopology.Enum.Field | TerrainTopology.Enum.Cliff | TerrainTopology.Enum.Summit | TerrainTopology.Enum.Beachside | TerrainTopology.Enum.Beach | TerrainTopology.Enum.Forest | TerrainTopology.Enum.Forestside | TerrainTopology.Enum.Ocean | TerrainTopology.Enum.Oceanside | TerrainTopology.Enum.Decor | TerrainTopology.Enum.Monument | TerrainTopology.Enum.Road | TerrainTopology.Enum.Roadside | TerrainTopology.Enum.Swamp | TerrainTopology.Enum.River | TerrainTopology.Enum.Riverside | TerrainTopology.Enum.Lake | TerrainTopology.Enum.Lakeside | TerrainTopology.Enum.Offshore | TerrainTopology.Enum.Powerline | TerrainTopology.Enum.Runway | TerrainTopology.Enum.Building | TerrainTopology.Enum.Cliffside | TerrainTopology.Enum.Mountain | TerrainTopology.Enum.Clutter | TerrainTopology.Enum.Alt | TerrainTopology.Enum.Tier0 | TerrainTopology.Enum.Tier1 | TerrainTopology.Enum.Tier2 | TerrainTopology.Enum.Mainland | TerrainTopology.Enum.Hilltop;

	public AnimationCurve time = AnimationCurve.Linear(0f, 0f, 24f, 0f);

	[Header("Clip data")]
	public List<MusicTheme.PositionedClip> clips = new List<MusicTheme.PositionedClip>();

	public List<MusicTheme.Layer> layers = new List<MusicTheme.Layer>();

	private Dictionary<int, List<MusicTheme.PositionedClip>> activeClips = new Dictionary<int, List<MusicTheme.PositionedClip>>();

	private List<AudioClip> firstAudioClips = new List<AudioClip>();

	private Dictionary<AudioClip, bool> audioClipDict = new Dictionary<AudioClip, bool>();

	public int layerCount
	{
		get
		{
			return this.layers.Count;
		}
	}

	public int samplesPerBar
	{
		get
		{
			return MusicUtil.BarsToSamples(this.tempo, 1f, 44100);
		}
	}

	public MusicTheme()
	{
	}

	private int ActiveClipCollectionID(int bar)
	{
		return Mathf.FloorToInt(Mathf.Max((float)(bar / 4), 0f));
	}

	public void AddLayer()
	{
		MusicTheme.Layer layer = new MusicTheme.Layer()
		{
			name = string.Concat("layer ", this.layers.Count)
		};
		this.layers.Add(layer);
	}

	public bool CanPlayInEnvironment(int currentBiome, int currentTopology, float currentRain, float currentSnow, float currentWind)
	{
		if (TOD_Sky.Instance && this.time.Evaluate(TOD_Sky.Instance.Cycle.Hour) < 0f)
		{
			return false;
		}
		if (this.biomes != (TerrainBiome.Enum.Arid | TerrainBiome.Enum.Temperate | TerrainBiome.Enum.Tundra | TerrainBiome.Enum.Arctic) && ((int)this.biomes & currentBiome) == 0)
		{
			return false;
		}
		if (this.topologies != (TerrainTopology.Enum.Field | TerrainTopology.Enum.Cliff | TerrainTopology.Enum.Summit | TerrainTopology.Enum.Beachside | TerrainTopology.Enum.Beach | TerrainTopology.Enum.Forest | TerrainTopology.Enum.Forestside | TerrainTopology.Enum.Ocean | TerrainTopology.Enum.Oceanside | TerrainTopology.Enum.Decor | TerrainTopology.Enum.Monument | TerrainTopology.Enum.Road | TerrainTopology.Enum.Roadside | TerrainTopology.Enum.Swamp | TerrainTopology.Enum.River | TerrainTopology.Enum.Riverside | TerrainTopology.Enum.Lake | TerrainTopology.Enum.Lakeside | TerrainTopology.Enum.Offshore | TerrainTopology.Enum.Powerline | TerrainTopology.Enum.Runway | TerrainTopology.Enum.Building | TerrainTopology.Enum.Cliffside | TerrainTopology.Enum.Mountain | TerrainTopology.Enum.Clutter | TerrainTopology.Enum.Alt | TerrainTopology.Enum.Tier0 | TerrainTopology.Enum.Tier1 | TerrainTopology.Enum.Tier2 | TerrainTopology.Enum.Mainland | TerrainTopology.Enum.Hilltop) && ((int)this.topologies & currentTopology) != 0)
		{
			return false;
		}
		if ((this.rain.min > 0f || this.rain.max < 1f) && currentRain < this.rain.min || currentRain > this.rain.max)
		{
			return false;
		}
		if ((this.snow.min > 0f || this.snow.max < 1f) && currentSnow < this.snow.min || currentSnow > this.snow.max)
		{
			return false;
		}
		if ((this.wind.min <= 0f && this.wind.max >= 1f || currentWind >= this.wind.min) && currentWind <= this.wind.max)
		{
			return true;
		}
		return false;
	}

	public bool ContainsAudioClip(AudioClip clip)
	{
		return this.audioClipDict.ContainsKey(clip);
	}

	public bool FirstClipsLoaded()
	{
		for (int i = 0; i < this.firstAudioClips.Count; i++)
		{
			if (this.firstAudioClips[i].loadState != AudioDataLoadState.Loaded)
			{
				return false;
			}
		}
		return true;
	}

	public List<MusicTheme.PositionedClip> GetActiveClipsForBar(int bar)
	{
		int num = this.ActiveClipCollectionID(bar);
		if (!this.activeClips.ContainsKey(num))
		{
			return null;
		}
		return this.activeClips[num];
	}

	public MusicTheme.Layer LayerById(int id)
	{
		if (this.layers.Count <= id)
		{
			return null;
		}
		return this.layers[id];
	}

	private void OnValidate()
	{
		this.audioClipDict.Clear();
		this.activeClips.Clear();
		this.UpdateLengthInBars();
		for (int i = 0; i < this.clips.Count; i++)
		{
			MusicTheme.PositionedClip item = this.clips[i];
			int num = this.ActiveClipCollectionID(item.startingBar - 8);
			int num1 = this.ActiveClipCollectionID(item.endingBar);
			for (int j = num; j <= num1; j++)
			{
				if (!this.activeClips.ContainsKey(j))
				{
					this.activeClips.Add(j, new List<MusicTheme.PositionedClip>());
				}
				if (!this.activeClips[j].Contains(item))
				{
					this.activeClips[j].Add(item);
				}
			}
			if (item.musicClip != null)
			{
				AudioClip audioClip = item.musicClip.audioClip;
				if (!this.audioClipDict.ContainsKey(audioClip))
				{
					this.audioClipDict.Add(audioClip, true);
				}
				if (item.startingBar < 8 && !this.firstAudioClips.Contains(audioClip))
				{
					this.firstAudioClips.Add(audioClip);
				}
				item.musicClip.lengthInBarsWithTail = Mathf.CeilToInt(MusicUtil.SecondsToBars(this.tempo, (double)item.musicClip.audioClip.length));
			}
		}
	}

	private void UpdateLengthInBars()
	{
		int num = 0;
		for (int i = 0; i < this.clips.Count; i++)
		{
			MusicTheme.PositionedClip item = this.clips[i];
			if (item.musicClip != null)
			{
				int num1 = item.startingBar + item.musicClip.lengthInBars;
				if (num1 > num)
				{
					num = num1;
				}
			}
		}
		this.lengthInBars = num;
	}

	[Serializable]
	public class Layer
	{
		public string name;

		public Layer()
		{
		}
	}

	[Serializable]
	public class PositionedClip
	{
		public MusicTheme theme;

		public MusicClip musicClip;

		public int startingBar;

		public int layerId;

		public float minIntensity;

		public float maxIntensity;

		public bool allowFadeIn;

		public bool allowFadeOut;

		public float fadeInTime;

		public float fadeOutTime;

		public float intensityReduction;

		public int jumpBarCount;

		public float jumpMinimumIntensity;

		public float jumpMaximumIntensity;

		public int endingBar
		{
			get
			{
				if (this.musicClip == null)
				{
					return this.startingBar;
				}
				return this.startingBar + this.musicClip.lengthInBarsWithTail;
			}
		}

		public bool isControlClip
		{
			get
			{
				return this.musicClip == null;
			}
		}

		public PositionedClip()
		{
		}

		public bool CanPlay(float intensity)
		{
			if (intensity <= this.minIntensity && (this.minIntensity != 0f || intensity != 0f))
			{
				return false;
			}
			return intensity <= this.maxIntensity;
		}

		public void CopySettingsFrom(MusicTheme.PositionedClip otherClip)
		{
			if (this.isControlClip != otherClip.isControlClip)
			{
				return;
			}
			if (otherClip == this)
			{
				return;
			}
			this.allowFadeIn = otherClip.allowFadeIn;
			this.fadeInTime = otherClip.fadeInTime;
			this.allowFadeOut = otherClip.allowFadeOut;
			this.fadeOutTime = otherClip.fadeOutTime;
			this.maxIntensity = otherClip.maxIntensity;
			this.minIntensity = otherClip.minIntensity;
			this.intensityReduction = otherClip.intensityReduction;
		}
	}

	[Serializable]
	public class ValueRange
	{
		public float min;

		public float max;

		public ValueRange(float min, float max)
		{
			this.min = min;
			this.max = max;
		}
	}
}