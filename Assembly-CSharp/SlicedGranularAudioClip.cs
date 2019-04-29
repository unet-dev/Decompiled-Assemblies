using System;
using System.Collections.Generic;
using UnityEngine;

public class SlicedGranularAudioClip : MonoBehaviour, IClientComponent
{
	public AudioClip sourceClip;

	public AudioClip granularClip;

	public int sampleRate = 44100;

	public float grainAttack = 0.1f;

	public float grainSustain = 0.1f;

	public float grainRelease = 0.1f;

	public float grainFrequency = 0.1f;

	public int grainAttackSamples;

	public int grainSustainSamples;

	public int grainReleaseSamples;

	public int grainFrequencySamples;

	public int samplesUntilNextGrain;

	public List<SlicedGranularAudioClip.Grain> grains = new List<SlicedGranularAudioClip.Grain>();

	public List<int> startPositions = new List<int>();

	public int lastStartPositionIdx = 2147483647;

	public SlicedGranularAudioClip()
	{
	}

	public class Grain
	{
		private float[] sourceData;

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

		public void FadeOut()
		{
			this.releaseStartSample = this.currentSample;
			this.endSample = this.releaseStartSample + this.releaseTimeSamples;
		}

		public float GetSample()
		{
			if (this.currentSample >= (int)this.sourceData.Length)
			{
				return 0f;
			}
			float single = this.sourceData[this.currentSample];
			if (this.currentSample <= this.attackEndSample)
			{
				this.gain += this.gainPerSampleAttack;
				if (this.gain > 0.5f)
				{
					this.gain = 0.5f;
				}
			}
			else if (this.currentSample >= this.releaseStartSample)
			{
				this.gain += this.gainPerSampleRelease;
				if (this.gain < 0f)
				{
					this.gain = 0f;
				}
			}
			this.currentSample++;
			return single * this.gain;
		}

		public void Init(float[] source, int start, int attack, int sustain, int release)
		{
			this.sourceData = source;
			this.startSample = start;
			this.currentSample = start;
			this.attackTimeSamples = attack;
			this.sustainTimeSamples = sustain;
			this.releaseTimeSamples = release;
			this.gainPerSampleAttack = 0.5f / (float)this.attackTimeSamples;
			this.gainPerSampleRelease = -0.5f / (float)this.releaseTimeSamples;
			this.attackEndSample = this.startSample + this.attackTimeSamples;
			this.releaseStartSample = this.attackEndSample + this.sustainTimeSamples;
			this.endSample = this.releaseStartSample + this.releaseTimeSamples;
			this.gain = 0f;
		}
	}
}