using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GranularAudioClip : MonoBehaviour
{
	public AudioClip sourceClip;

	private float[] sourceAudioData;

	private int sourceChannels = 1;

	public AudioClip granularClip;

	public int sampleRate = 44100;

	public float sourceTime = 0.5f;

	public float sourceTimeVariation = 0.1f;

	public float grainAttack = 0.1f;

	public float grainSustain = 0.1f;

	public float grainRelease = 0.1f;

	public float grainFrequency = 0.1f;

	public int grainAttackSamples;

	public int grainSustainSamples;

	public int grainReleaseSamples;

	public int grainFrequencySamples;

	public int samplesUntilNextGrain;

	public List<GranularAudioClip.Grain> grains = new List<GranularAudioClip.Grain>();

	private System.Random random = new System.Random();

	private bool inited;

	public GranularAudioClip()
	{
	}

	private void CleanupFinishedGrains()
	{
		for (int i = this.grains.Count - 1; i >= 0; i--)
		{
			GranularAudioClip.Grain item = this.grains[i];
			if (item.finished)
			{
				Pool.Free<GranularAudioClip.Grain>(ref item);
				this.grains.RemoveAt(i);
			}
		}
	}

	private void InitAudioClip()
	{
		int num = 1;
		int num1 = 1;
		UnityEngine.AudioSettings.GetDSPBufferSize(out num, out num1);
		this.granularClip = AudioClip.Create(string.Concat(this.sourceClip.name, " (granular)"), num, this.sourceClip.channels, this.sampleRate, true, new AudioClip.PCMReaderCallback(this.OnAudioRead));
		this.sourceChannels = this.sourceClip.channels;
	}

	private void OnAudioRead(float[] data)
	{
		for (int i = 0; i < (int)data.Length; i++)
		{
			if (this.samplesUntilNextGrain <= 0)
			{
				this.SpawnGrain();
			}
			float sample = 0f;
			for (int j = 0; j < this.grains.Count; j++)
			{
				sample += this.grains[j].GetSample();
			}
			data[i] = sample;
			this.samplesUntilNextGrain--;
		}
		this.CleanupFinishedGrains();
	}

	private void RefreshCachedData()
	{
		this.grainAttackSamples = Mathf.FloorToInt(this.grainAttack * (float)this.sampleRate * (float)this.sourceChannels);
		this.grainSustainSamples = Mathf.FloorToInt(this.grainSustain * (float)this.sampleRate * (float)this.sourceChannels);
		this.grainReleaseSamples = Mathf.FloorToInt(this.grainRelease * (float)this.sampleRate * (float)this.sourceChannels);
		this.grainFrequencySamples = Mathf.FloorToInt(this.grainFrequency * (float)this.sampleRate * (float)this.sourceChannels);
	}

	private void SpawnGrain()
	{
		if (this.grainFrequencySamples == 0)
		{
			return;
		}
		float single = (float)(this.random.NextDouble() * (double)this.sourceTimeVariation * 2) - this.sourceTimeVariation;
		int num = Mathf.FloorToInt((this.sourceTime + single) * (float)this.sampleRate / (float)this.sourceChannels);
		GranularAudioClip.Grain grain = Pool.Get<GranularAudioClip.Grain>();
		grain.Init(this.sourceAudioData, num, this.grainAttackSamples, this.grainSustainSamples, this.grainReleaseSamples);
		this.grains.Add(grain);
		this.samplesUntilNextGrain = this.grainFrequencySamples;
	}

	private void Update()
	{
		if (!this.inited && this.sourceClip.loadState == AudioDataLoadState.Loaded)
		{
			this.sampleRate = this.sourceClip.frequency;
			this.sourceAudioData = new float[this.sourceClip.samples * this.sourceClip.channels];
			this.sourceClip.GetData(this.sourceAudioData, 0);
			this.InitAudioClip();
			AudioSource component = base.GetComponent<AudioSource>();
			component.clip = this.granularClip;
			component.loop = true;
			component.Play();
			this.inited = true;
		}
		this.RefreshCachedData();
	}

	public class Grain
	{
		private float[] sourceData;

		private int sourceDataLength;

		private int startSample;

		private int currentSample;

		private int attackTimeSamples;

		private int sustainTimeSamples;

		private int releaseTimeSamples;

		private float gain;

		private float gainPerSampleAttack;

		private float gainPerSampleRelease;

		private int attackEndSample;

		private int releaseStartSample;

		private int endSample;

		public bool finished
		{
			get
			{
				return this.currentSample >= this.endSample;
			}
		}

		public Grain()
		{
		}

		public float GetSample()
		{
			int num = this.currentSample % this.sourceDataLength;
			if (num < 0)
			{
				num += this.sourceDataLength;
			}
			float single = this.sourceData[num];
			if (this.currentSample <= this.attackEndSample)
			{
				this.gain += this.gainPerSampleAttack;
			}
			else if (this.currentSample >= this.releaseStartSample)
			{
				this.gain += this.gainPerSampleRelease;
			}
			this.currentSample++;
			return single * this.gain;
		}

		public void Init(float[] source, int start, int attack, int sustain, int release)
		{
			this.sourceData = source;
			this.sourceDataLength = (int)this.sourceData.Length;
			this.startSample = start;
			this.currentSample = start;
			this.attackTimeSamples = attack;
			this.sustainTimeSamples = sustain;
			this.releaseTimeSamples = release;
			this.gainPerSampleAttack = 1f / (float)this.attackTimeSamples;
			this.gainPerSampleRelease = -1f / (float)this.releaseTimeSamples;
			this.attackEndSample = this.startSample + this.attackTimeSamples;
			this.releaseStartSample = this.attackEndSample + this.sustainTimeSamples;
			this.endSample = this.releaseStartSample + this.releaseTimeSamples;
			this.gain = 0f;
		}
	}
}