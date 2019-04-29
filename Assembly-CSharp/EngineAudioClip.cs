using JSON;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EngineAudioClip : MonoBehaviour, IClientComponent
{
	public AudioClip granularClip;

	public AudioClip accelerationClip;

	public TextAsset accelerationCyclesJson;

	public List<EngineAudioClip.EngineCycle> accelerationCycles = new List<EngineAudioClip.EngineCycle>();

	public List<EngineAudioClip.EngineCycleBucket> cycleBuckets = new List<EngineAudioClip.EngineCycleBucket>();

	public Dictionary<int, EngineAudioClip.EngineCycleBucket> accelerationCyclesByRPM = new Dictionary<int, EngineAudioClip.EngineCycleBucket>();

	public Dictionary<int, int> rpmBucketLookup = new Dictionary<int, int>();

	public int sampleRate = 44100;

	public int samplesUntilNextGrain;

	public int lastCycleId;

	public List<EngineAudioClip.Grain> grains = new List<EngineAudioClip.Grain>();

	public int currentRPM;

	public int targetRPM = 1500;

	public int minRPM;

	public int maxRPM;

	public int cyclePadding;

	[Range(0f, 1f)]
	public float RPMControl;

	public AudioSource source;

	public EngineAudioClip()
	{
	}

	private int GetBucketRPM(int RPM)
	{
		return Mathf.RoundToInt((float)(RPM / 25)) * 25;
	}

	[Serializable]
	public class EngineCycle
	{
		public int RPM;

		public int startSample;

		public int endSample;

		public float period;

		public int id;

		public EngineCycle(int RPM, int startSample, int endSample, float period, int id)
		{
			this.RPM = RPM;
			this.startSample = startSample;
			this.endSample = endSample;
			this.period = period;
			this.id = id;
		}
	}

	public class EngineCycleBucket
	{
		public int RPM;

		public List<EngineAudioClip.EngineCycle> cycles;

		public List<int> remainingCycles;

		public EngineCycleBucket(int RPM)
		{
			this.RPM = RPM;
		}

		public void Add(EngineAudioClip.EngineCycle cycle)
		{
			if (!this.cycles.Contains(cycle))
			{
				this.cycles.Add(cycle);
			}
		}

		public EngineAudioClip.EngineCycle GetCycle(System.Random random, int lastCycleId)
		{
			if (this.remainingCycles.Count == 0)
			{
				this.ResetRemainingCycles(random);
			}
			int num = this.remainingCycles.Pop<int>();
			if (this.cycles[num].id == lastCycleId)
			{
				if (this.remainingCycles.Count == 0)
				{
					this.ResetRemainingCycles(random);
				}
				num = this.remainingCycles.Pop<int>();
			}
			return this.cycles[num];
		}

		private void ResetRemainingCycles(System.Random random)
		{
			for (int i = 0; i < this.cycles.Count; i++)
			{
				this.remainingCycles.Add(i);
			}
			this.remainingCycles.Shuffle<int>((uint)random.Next());
		}
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
				if (this.gain > 0.8f)
				{
					this.gain = 0.8f;
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

		public void Init(float[] source, EngineAudioClip.EngineCycle cycle, int cyclePadding)
		{
			this.sourceData = source;
			this.startSample = cycle.startSample - cyclePadding;
			this.currentSample = this.startSample;
			this.attackTimeSamples = cyclePadding;
			this.sustainTimeSamples = cycle.endSample - cycle.startSample;
			this.releaseTimeSamples = cyclePadding;
			this.gainPerSampleAttack = 1f / (float)this.attackTimeSamples;
			this.gainPerSampleRelease = -1f / (float)this.releaseTimeSamples;
			this.attackEndSample = this.startSample + this.attackTimeSamples;
			this.releaseStartSample = this.attackEndSample + this.sustainTimeSamples;
			this.endSample = this.releaseStartSample + this.releaseTimeSamples;
			this.gain = 0f;
		}
	}
}