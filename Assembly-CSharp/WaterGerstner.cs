using System;
using UnityEngine;

public class WaterGerstner
{
	public const int WaveCount = 6;

	public WaterGerstner()
	{
	}

	private static void GerstnerShoreWave(WaterGerstner.PrecomputedShoreWaves wave, Vector2 waveDir, Vector2 pos, Vector2 shoreVec, float variation_t, ref float outH)
	{
		float single = Mathf.Clamp01(waveDir.x * shoreVec.x + waveDir.y * shoreVec.y);
		single *= single;
		float k = wave.K * (waveDir.x * pos.x + waveDir.y * pos.y - wave.C + variation_t);
		Mathf.Cos(k);
		float single1 = Mathf.Sin(k);
		outH = outH + wave.A * wave.Amplitude * single1 * single;
	}

	private static void GerstnerShoreWave(WaterGerstner.PrecomputedShoreWaves wave, Vector2 waveDir, Vector2 pos, Vector2 shoreVec, float variation_t, ref Vector3 outP)
	{
		float single = Mathf.Clamp01(waveDir.x * shoreVec.x + waveDir.y * shoreVec.y);
		single *= single;
		float k = wave.K * (waveDir.x * pos.x + waveDir.y * pos.y - wave.C + variation_t);
		float single1 = Mathf.Cos(k);
		float single2 = Mathf.Sin(k);
		ref float singlePointer = ref outP.x;
		singlePointer = singlePointer + waveDir.x * wave.A * single1 * single;
		ref float a = ref outP.y;
		a = a + wave.A * wave.Amplitude * single2 * single;
		ref float singlePointer1 = ref outP.z;
		singlePointer1 = singlePointer1 + waveDir.y * wave.A * single1 * single;
	}

	private static void GerstnerWave(WaterGerstner.PrecomputedWave wave, Vector2 pos, Vector2 shoreVec, ref float outHeight)
	{
		Vector2 direction = wave.Direction;
		float single = Mathf.Sin(wave.K * (direction.x * pos.x + direction.y * pos.y - wave.C));
		outHeight = outHeight + wave.A * single;
	}

	private static void GerstnerWave(WaterGerstner.PrecomputedWave wave, Vector2 pos, Vector2 shoreVec, ref Vector3 outP)
	{
		Vector2 direction = wave.Direction;
		float k = wave.K * (direction.x * pos.x + direction.y * pos.y - wave.C);
		float single = Mathf.Cos(k);
		float single1 = Mathf.Sin(k);
		ref float a = ref outP.x;
		a = a + direction.x * wave.A * single;
		ref float singlePointer = ref outP.y;
		singlePointer = singlePointer + wave.A * single1;
		ref float a1 = ref outP.z;
		a1 = a1 + direction.y * wave.A * single;
	}

	public static Vector3 SampleDisplacement(WaterSystem instance, Vector3 location, Vector3 shore)
	{
		WaterGerstner.PrecomputedWave[] precomputedWaves = instance.PrecomputedWaves;
		WaterGerstner.PrecomputedShoreWaves precomputedShoreWaves = instance.PrecomputedShoreWaves;
		Vector2 vector2 = new Vector2(location.x, location.z);
		Vector2 vector21 = new Vector2(shore.x, shore.y);
		float single = 1f - Mathf.Clamp01(shore.z * instance.ShoreWavesRcpFadeDistance);
		float single1 = Mathf.Clamp01(shore.z * instance.TerrainRcpFadeDistance);
		float single2 = Mathf.Cos(vector2.x * precomputedShoreWaves.DirectionVarFreq) * precomputedShoreWaves.DirectionVarAmp;
		float single3 = Mathf.Cos(vector2.y * precomputedShoreWaves.DirectionVarFreq) * precomputedShoreWaves.DirectionVarAmp;
		float single4 = single2 + single3;
		Vector3 vector3 = Vector3.zero;
		Vector3 vector31 = Vector3.zero;
		for (int i = 0; i < 6; i++)
		{
			WaterGerstner.GerstnerWave(precomputedWaves[i], vector2, vector21, ref vector3);
			WaterGerstner.GerstnerShoreWave(precomputedShoreWaves, precomputedShoreWaves.Directions[i], vector2, vector21, single4, ref vector31);
		}
		return Vector3.Lerp(vector3, vector31, single) * single1;
	}

	public static float SampleHeight(WaterSystem instance, Vector3 location, Vector3 shore)
	{
		WaterGerstner.PrecomputedWave[] precomputedWaves = instance.PrecomputedWaves;
		WaterGerstner.PrecomputedShoreWaves precomputedShoreWaves = instance.PrecomputedShoreWaves;
		Vector2 vector2 = new Vector2(location.x, location.z);
		Vector2 vector21 = new Vector2(shore.x, shore.y);
		float single = 1f - Mathf.Clamp01(shore.z * instance.ShoreWavesRcpFadeDistance);
		float single1 = Mathf.Clamp01(shore.z * instance.TerrainRcpFadeDistance);
		float single2 = Mathf.Cos(vector2.x * precomputedShoreWaves.DirectionVarFreq) * precomputedShoreWaves.DirectionVarAmp;
		float single3 = Mathf.Cos(vector2.y * precomputedShoreWaves.DirectionVarFreq) * precomputedShoreWaves.DirectionVarAmp;
		float single4 = single2 + single3;
		float single5 = 0f;
		float single6 = 0f;
		for (int i = 0; i < 6; i++)
		{
			WaterGerstner.GerstnerWave(precomputedWaves[i], vector2, vector21, ref single5);
			WaterGerstner.GerstnerShoreWave(precomputedShoreWaves, precomputedShoreWaves.Directions[i], vector2, vector21, single4, ref single6);
		}
		return Mathf.Lerp(single5, single6, single) * single1;
	}

	public static void SampleHeightArray(WaterSystem instance, Vector2[] location, Vector3[] shore, float[] height)
	{
		Debug.Assert((int)location.Length == (int)height.Length);
		for (int i = 0; i < (int)location.Length; i++)
		{
			Vector3 vector3 = new Vector3(location[i].x, 0f, location[i].y);
			Vector3 vector31 = WaterGerstner.SampleDisplacement(instance, vector3, shore[i]);
			vector31.x = vector3.x - vector31.x;
			vector31.z = vector3.z - vector31.z;
			height[i] = WaterGerstner.SampleHeight(instance, vector3, shore[i]);
		}
	}

	public static void UpdatePrecomputedShoreWaves(WaterGerstner.ShoreWaveParams shoreWaves, ref WaterGerstner.PrecomputedShoreWaves precomputed)
	{
		if (precomputed.Directions != null || (int)precomputed.Directions.Length != 6)
		{
			precomputed.Directions = new Vector2[6];
		}
		Debug.Assert((int)precomputed.Directions.Length == (int)shoreWaves.DirectionAngles.Length);
		for (int i = 0; i < 6; i++)
		{
			float directionAngles = shoreWaves.DirectionAngles[i] * 0.0174532924f;
			precomputed.Directions[i] = new Vector2(Mathf.Cos(directionAngles), Mathf.Sin(directionAngles));
		}
		precomputed.Steepness = shoreWaves.Steepness;
		precomputed.Amplitude = shoreWaves.Amplitude;
		precomputed.K = 6.28318548f / shoreWaves.Length;
		precomputed.C = Mathf.Sqrt(9.8f / precomputed.K) * shoreWaves.Speed * WaterSystem.WaveTime;
		precomputed.A = shoreWaves.Steepness / precomputed.K;
		precomputed.DirectionVarFreq = shoreWaves.DirectionVarFreq;
		precomputed.DirectionVarAmp = shoreWaves.DirectionVarAmp;
	}

	public static void UpdatePrecomputedWaves(WaterGerstner.WaveParams[] waves, ref WaterGerstner.PrecomputedWave[] precomputed)
	{
		if (precomputed == null || (int)precomputed.Length != 6)
		{
			precomputed = new WaterGerstner.PrecomputedWave[6];
		}
		Debug.Assert((int)precomputed.Length == (int)waves.Length);
		for (int i = 0; i < 6; i++)
		{
			float angle = waves[i].Angle * 0.0174532924f;
			precomputed[i].Angle = angle;
			precomputed[i].Direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
			precomputed[i].Steepness = waves[i].Steepness;
			precomputed[i].K = 6.28318548f / waves[i].Length;
			precomputed[i].C = Mathf.Sqrt(9.8f / precomputed[i].K) * waves[i].Speed * WaterSystem.WaveTime;
			precomputed[i].A = waves[i].Steepness / precomputed[i].K;
		}
	}

	public static void UpdateShoreWaveArray(WaterGerstner.PrecomputedShoreWaves precomputed, ref Vector4[] array)
	{
		Debug.Assert((int)precomputed.Directions.Length == 6);
		if (array == null || (int)array.Length != 6)
		{
			array = new Vector4[3];
		}
		Debug.Assert((int)array.Length == 3);
		Vector2[] directions = precomputed.Directions;
		array[0] = new Vector4(directions[0].x, directions[0].y, directions[1].x, directions[1].y);
		array[1] = new Vector4(directions[2].x, directions[2].y, directions[3].x, directions[3].y);
		array[2] = new Vector4(directions[4].x, directions[4].y, directions[5].x, directions[5].y);
	}

	public static void UpdateWaveArray(WaterGerstner.PrecomputedWave[] precomputed, ref Vector4[] array)
	{
		if (array == null || (int)array.Length != 6)
		{
			array = new Vector4[6];
		}
		Debug.Assert((int)array.Length == (int)precomputed.Length);
		for (int i = 0; i < 6; i++)
		{
			array[i] = new Vector4(precomputed[i].Angle, precomputed[i].Steepness, precomputed[i].K, precomputed[i].C);
		}
	}

	public struct PrecomputedShoreWaves
	{
		public Vector2[] Directions;

		public float Steepness;

		public float Amplitude;

		public float K;

		public float C;

		public float A;

		public float DirectionVarFreq;

		public float DirectionVarAmp;

		public static WaterGerstner.PrecomputedShoreWaves Default;

		static PrecomputedShoreWaves()
		{
			WaterGerstner.PrecomputedShoreWaves precomputedShoreWafe = new WaterGerstner.PrecomputedShoreWaves()
			{
				Directions = new Vector2[] { Vector2.right, Vector2.right, Vector2.right, Vector2.right, Vector2.right, Vector2.right },
				Steepness = 0.75f,
				Amplitude = 0.2f,
				K = 1f,
				C = 1f,
				A = 1f,
				DirectionVarFreq = 0.1f,
				DirectionVarAmp = 3f
			};
			WaterGerstner.PrecomputedShoreWaves.Default = precomputedShoreWafe;
		}
	}

	public struct PrecomputedWave
	{
		public float Angle;

		public Vector2 Direction;

		public float Steepness;

		public float K;

		public float C;

		public float A;

		public static WaterGerstner.PrecomputedWave Default;

		static PrecomputedWave()
		{
			WaterGerstner.PrecomputedWave precomputedWave = new WaterGerstner.PrecomputedWave()
			{
				Angle = 0f,
				Direction = Vector2.right,
				Steepness = 0.4f,
				K = 1f,
				C = 1f,
				A = 1f
			};
			WaterGerstner.PrecomputedWave.Default = precomputedWave;
		}
	}

	[Serializable]
	public class ShoreWaveParams
	{
		[Range(0f, 2f)]
		public float Steepness;

		[Range(0f, 1f)]
		public float Amplitude;

		[Range(0.01f, 1000f)]
		public float Length;

		[Range(-10f, 10f)]
		public float Speed;

		public float[] DirectionAngles;

		public float DirectionVarFreq;

		public float DirectionVarAmp;

		public ShoreWaveParams()
		{
		}
	}

	[Serializable]
	public class WaveParams
	{
		[Range(0f, 360f)]
		public float Angle;

		[Range(0f, 0.99f)]
		public float Steepness;

		[Range(0.01f, 1000f)]
		public float Length;

		[Range(-10f, 10f)]
		public float Speed;

		public WaveParams()
		{
		}
	}
}