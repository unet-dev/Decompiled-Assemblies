using System;
using UnityEngine;

public class WaterGerstner
{
	public WaterGerstner()
	{
	}

	public static float SampleHeight(WaterGerstner.Wave[] waves, Vector3 location)
	{
		Vector2 vector2 = new Vector2(location.x, location.z);
		float waveTime = WaterSystem.WaveTime;
		float single = 0f;
		for (uint i = 0; (ulong)i < (long)((int)waves.Length); i++)
		{
			float single1 = waves[i].wi;
			float single2 = waves[i].phi;
			Vector2 di = waves[i].Di;
			float ai = waves[i].Ai;
			float single3 = Mathf.Sin(single1 * Vector2.Dot(di, vector2) + single2 * waveTime);
			single = single + ai * single3;
		}
		return single;
	}

	public static void SampleHeightArray(WaterGerstner.Wave[] waves, Vector2[] location, float[] height)
	{
		Debug.Assert((int)location.Length == (int)height.Length);
		float waveTime = WaterSystem.WaveTime;
		for (uint i = 0; (ulong)i < (long)((int)waves.Length); i++)
		{
			float single = waves[i].wi;
			float single1 = waves[i].phi;
			Vector2 di = waves[i].Di;
			float ai = waves[i].Ai;
			for (int j = 0; j < (int)location.Length; j++)
			{
				float single2 = Mathf.Sin(single * (di.x * location[j].x + di.y * location[j].y) + single1 * waveTime);
				float single3 = ai * single2;
				height[j] = (i > 0 ? height[j] + single3 : single3);
			}
		}
	}

	public static void SampleWaves(WaterGerstner.Wave[] waves, Vector3 location, out Vector3 position, out Vector3 normal)
	{
		Vector2 vector2 = new Vector2(location.x, location.z);
		float waveTime = WaterSystem.WaveTime;
		Vector3 vector3 = Vector3.zero;
		Vector3 vector31 = Vector3.zero;
		for (uint i = 0; (ulong)i < (long)((int)waves.Length); i++)
		{
			float single = waves[i].wi;
			float single1 = waves[i].phi;
			float wA = waves[i].WA;
			Vector2 di = waves[i].Di;
			float ai = waves[i].Ai;
			float qi = waves[i].Qi;
			float single2 = single * Vector2.Dot(di, vector2) + single1 * waveTime;
			float single3 = Mathf.Sin(single2);
			float single4 = Mathf.Cos(single2);
			ref float singlePointer = ref vector3.x;
			singlePointer = singlePointer + qi * ai * di.x * single4;
			ref float singlePointer1 = ref vector3.y;
			singlePointer1 = singlePointer1 + qi * ai * di.y * single4;
			ref float singlePointer2 = ref vector3.z;
			singlePointer2 = singlePointer2 + ai * single3;
			ref float singlePointer3 = ref vector31.x;
			singlePointer3 = singlePointer3 + di.x * wA * single4;
			ref float singlePointer4 = ref vector31.y;
			singlePointer4 = singlePointer4 + di.y * wA * single4;
			ref float singlePointer5 = ref vector31.z;
			singlePointer5 = singlePointer5 + qi * wA * single3;
		}
		position = new Vector3(vector3.x, vector3.z, vector3.y);
		normal = new Vector3(-vector31.x, 1f - vector31.z, -vector31.y);
	}

	public static WaterGerstner.Wave[] SetupWaves(Vector3 wind, WaterGerstner.WaveSettings settings)
	{
		UnityEngine.Random.State state = UnityEngine.Random.state;
		UnityEngine.Random.InitState(settings.RandomSeed);
		int waveCount = settings.WaveCount;
		float single = Mathf.Atan2(wind.z, wind.x);
		float single1 = (float)(1 / waveCount);
		float amplitude = settings.Amplitude;
		float length = settings.Length;
		float steepness = settings.Steepness;
		WaterGerstner.Wave[] wave = new WaterGerstner.Wave[waveCount];
		for (int i = 0; i < waveCount; i++)
		{
			float single2 = Mathf.Lerp(0.5f, 1.5f, (float)i * single1);
			float single3 = single + 0.0174532924f * UnityEngine.Random.Range(-settings.AngleSpread, settings.AngleSpread);
			Vector2 vector2 = new Vector2(-Mathf.Cos(single3), -Mathf.Sin(single3));
			float single4 = amplitude * single2 * UnityEngine.Random.Range(0.8f, 1.2f);
			float single5 = length * single2 * UnityEngine.Random.Range(0.6f, 1.4f);
			float single6 = Mathf.Clamp01(steepness * single2 * UnityEngine.Random.Range(0.6f, 1.4f));
			wave[i] = new WaterGerstner.Wave(waveCount, vector2, single4, single5, single6);
			UnityEngine.Random.InitState(settings.RandomSeed + i + 1);
		}
		UnityEngine.Random.state = state;
		return wave;
	}

	[Serializable]
	public struct Wave
	{
		private const float MaxFrequency = 5f;

		public float wi;

		public float phi;

		public float WA;

		public Vector2 Di;

		public float Ai;

		public float Qi;

		public Wave(int waveCount, Vector2 direction, float amplitude, float length, float steepness)
		{
			this.wi = 2f / length;
			this.phi = Mathf.Min(5f, Mathf.Sqrt(30.8190269f * this.wi)) * this.wi;
			this.WA = this.wi * amplitude;
			this.Di = direction;
			this.Ai = amplitude;
			this.Qi = steepness / (this.WA * (float)waveCount);
		}
	}

	[Serializable]
	public class WaveSettings
	{
		[Range(1f, 8f)]
		public int WaveCount;

		public float Amplitude;

		public float Length;

		public float AngleSpread;

		[NonSerialized]
		public float Steepness;

		public int RandomSeed;

		public WaveSettings()
		{
		}
	}
}