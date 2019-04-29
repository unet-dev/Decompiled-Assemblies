using System;
using UnityEngine;

public class LeanAudio
{
	public static float MIN_FREQEUNCY_PERIOD;

	public static int PROCESSING_ITERATIONS_MAX;

	public static float[] generatedWaveDistances;

	public static int generatedWaveDistancesCount;

	private static float[] longList;

	static LeanAudio()
	{
		LeanAudio.MIN_FREQEUNCY_PERIOD = 0.000115f;
		LeanAudio.PROCESSING_ITERATIONS_MAX = 50000;
		LeanAudio.generatedWaveDistancesCount = 0;
	}

	public LeanAudio()
	{
	}

	public static AudioClip createAudio(AnimationCurve volume, AnimationCurve frequency, LeanAudioOptions options = null)
	{
		if (options == null)
		{
			options = new LeanAudioOptions();
		}
		return LeanAudio.createAudioFromWave(LeanAudio.createAudioWave(volume, frequency, options), options);
	}

	private static AudioClip createAudioFromWave(int waveLength, LeanAudioOptions options)
	{
		float single = LeanAudio.longList[waveLength - 2];
		float[] singleArray = new float[(int)((float)options.frequencyRate * single)];
		int num = 0;
		float single1 = LeanAudio.longList[num];
		float single2 = 0f;
		float single3 = LeanAudio.longList[num];
		float single4 = LeanAudio.longList[num + 1];
		for (int i = 0; i < (int)singleArray.Length; i++)
		{
			float single5 = (float)i / (float)options.frequencyRate;
			if (single5 > LeanAudio.longList[num])
			{
				single2 = LeanAudio.longList[num];
				num += 2;
				single1 = LeanAudio.longList[num] - LeanAudio.longList[num - 2];
				single4 = LeanAudio.longList[num + 1];
			}
			float single6 = (single5 - single2) / single1;
			float single7 = Mathf.Sin(single6 * 3.14159274f);
			if (options.waveStyle == LeanAudioOptions.LeanAudioWaveStyle.Square)
			{
				if (single7 > 0f)
				{
					single7 = 1f;
				}
				if (single7 < 0f)
				{
					single7 = -1f;
				}
			}
			else if (options.waveStyle == LeanAudioOptions.LeanAudioWaveStyle.Sawtooth)
			{
				float single8 = (single7 > 0f ? 1f : -1f);
				single7 = (single6 >= 0.5f ? (1f - single6) * 2f * single8 : single6 * 2f * single8);
			}
			else if (options.waveStyle == LeanAudioOptions.LeanAudioWaveStyle.Noise)
			{
				float single9 = 1f - options.waveNoiseInfluence + Mathf.PerlinNoise(0f, single5 * options.waveNoiseScale) * options.waveNoiseInfluence;
				single7 *= single9;
			}
			single7 *= single4;
			if (options.modulation != null)
			{
				for (int j = 0; j < (int)options.modulation.Length; j++)
				{
					float item = Mathf.Abs(Mathf.Sin(1.5708f + single5 * (1f / options.modulation[j][0]) * 3.14159274f));
					float item1 = 1f - options.modulation[j][1];
					item = options.modulation[j][1] + item1 * item;
					single7 *= item;
				}
			}
			singleArray[i] = single7;
		}
		int length = (int)singleArray.Length;
		AudioClip audioClip = null;
		if (!options.useSetData)
		{
			options.stream = new LeanAudioStream(singleArray);
			audioClip = AudioClip.Create("Generated Audio", length, 1, options.frequencyRate, false, new AudioClip.PCMReaderCallback(options.stream.OnAudioRead), new AudioClip.PCMSetPositionCallback(options.stream.OnAudioSetPosition));
			options.stream.audioClip = audioClip;
		}
		else
		{
			audioClip = AudioClip.Create("Generated Audio", length, 1, options.frequencyRate, false, null, new AudioClip.PCMSetPositionCallback(LeanAudio.OnAudioSetPosition));
			audioClip.SetData(singleArray, 0);
		}
		return audioClip;
	}

	public static LeanAudioStream createAudioStream(AnimationCurve volume, AnimationCurve frequency, LeanAudioOptions options = null)
	{
		if (options == null)
		{
			options = new LeanAudioOptions();
		}
		options.useSetData = false;
		LeanAudio.createAudioFromWave(LeanAudio.createAudioWave(volume, frequency, options), options);
		return options.stream;
	}

	private static int createAudioWave(AnimationCurve volume, AnimationCurve frequency, LeanAudioOptions options)
	{
		float item = volume[volume.length - 1].time;
		int num = 0;
		float single = 0f;
		int num1 = 0;
		while (num1 < LeanAudio.PROCESSING_ITERATIONS_MAX)
		{
			float mINFREQEUNCYPERIOD = frequency.Evaluate(single);
			if (mINFREQEUNCYPERIOD < LeanAudio.MIN_FREQEUNCY_PERIOD)
			{
				mINFREQEUNCYPERIOD = LeanAudio.MIN_FREQEUNCY_PERIOD;
			}
			float single1 = volume.Evaluate(single + 0.5f * mINFREQEUNCYPERIOD);
			if (options.vibrato != null)
			{
				for (int i = 0; i < (int)options.vibrato.Length; i++)
				{
					float item1 = Mathf.Abs(Mathf.Sin(1.5708f + single * (1f / options.vibrato[i][0]) * 3.14159274f));
					float item2 = 1f - options.vibrato[i][1];
					item1 = options.vibrato[i][1] + item2 * item1;
					single1 *= item1;
				}
			}
			if (single + 0.5f * mINFREQEUNCYPERIOD >= item)
			{
				break;
			}
			if (num < LeanAudio.PROCESSING_ITERATIONS_MAX - 1)
			{
				int num2 = num / 2;
				single += mINFREQEUNCYPERIOD;
				LeanAudio.generatedWaveDistances[num2] = single;
				LeanAudio.longList[num] = single;
				LeanAudio.longList[num + 1] = (num1 % 2 == 0 ? -single1 : single1);
				num += 2;
				num1++;
			}
			else
			{
				Debug.LogError(string.Concat("LeanAudio has reached it's processing cap. To avoid this error increase the number of iterations ex: LeanAudio.PROCESSING_ITERATIONS_MAX = ", LeanAudio.PROCESSING_ITERATIONS_MAX * 2));
				break;
			}
		}
		num += -2;
		LeanAudio.generatedWaveDistancesCount = num / 2;
		return num;
	}

	public static AudioClip generateAudioFromCurve(AnimationCurve curve, int frequencyRate = 44100)
	{
		float item = curve[curve.length - 1].time;
		float[] singleArray = new float[(int)((float)frequencyRate * item)];
		for (int i = 0; i < (int)singleArray.Length; i++)
		{
			float single = (float)i / (float)frequencyRate;
			singleArray[i] = curve.Evaluate(single);
		}
		int length = (int)singleArray.Length;
		AudioClip audioClip = AudioClip.Create("Generated Audio", length, 1, frequencyRate, false);
		audioClip.SetData(singleArray, 0);
		return audioClip;
	}

	private static void OnAudioSetPosition(int newPosition)
	{
	}

	public static LeanAudioOptions options()
	{
		if (LeanAudio.generatedWaveDistances == null)
		{
			LeanAudio.generatedWaveDistances = new float[LeanAudio.PROCESSING_ITERATIONS_MAX];
			LeanAudio.longList = new float[LeanAudio.PROCESSING_ITERATIONS_MAX];
		}
		return new LeanAudioOptions();
	}

	public static AudioSource play(AudioClip audio, float volume)
	{
		AudioSource audioSource = LeanAudio.playClipAt(audio, Vector3.zero);
		audioSource.volume = volume;
		return audioSource;
	}

	public static AudioSource play(AudioClip audio)
	{
		return LeanAudio.playClipAt(audio, Vector3.zero);
	}

	public static AudioSource play(AudioClip audio, Vector3 pos)
	{
		return LeanAudio.playClipAt(audio, pos);
	}

	public static AudioSource play(AudioClip audio, Vector3 pos, float volume)
	{
		AudioSource audioSource = LeanAudio.playClipAt(audio, pos);
		audioSource.minDistance = 1f;
		audioSource.volume = volume;
		return audioSource;
	}

	public static AudioSource playClipAt(AudioClip clip, Vector3 pos)
	{
		GameObject gameObject = new GameObject();
		gameObject.transform.position = pos;
		AudioSource audioSource = gameObject.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.Play();
		UnityEngine.Object.Destroy(gameObject, clip.length);
		return audioSource;
	}

	public static void printOutAudioClip(AudioClip audioClip, ref AnimationCurve curve, float scaleX = 1f)
	{
		float[] singleArray = new float[audioClip.samples * audioClip.channels];
		audioClip.GetData(singleArray, 0);
		int num = 0;
		Keyframe[] keyframe = new Keyframe[(int)singleArray.Length];
		while (num < (int)singleArray.Length)
		{
			keyframe[num] = new Keyframe((float)num * scaleX, singleArray[num]);
			num++;
		}
		curve = new AnimationCurve(keyframe);
	}
}