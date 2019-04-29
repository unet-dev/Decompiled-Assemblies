using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[ExecuteInEditMode]
public class WaterSystem : MonoBehaviour
{
	public WaterQuality Quality = WaterQuality.High;

	public bool ShowDebug;

	public bool ShowGizmos;

	public bool ProgressTime = true;

	public WaterSystem.SimulationSettings Simulation = new WaterSystem.SimulationSettings();

	public WaterSystem.RenderingSettings Rendering = new WaterSystem.RenderingSettings();

	[HideInInspector]
	public WaterGerstner.Wave[] GerstnerWaves;

	private static WaterSystem instance;

	public static WaterCollision Collision
	{
		get;
		private set;
	}

	public static WaterDynamics Dynamics
	{
		get;
		private set;
	}

	public static WaterSystem Instance
	{
		get
		{
			return WaterSystem.instance;
		}
	}

	public bool IsInitialized
	{
		get;
		private set;
	}

	public static WaterBody Ocean
	{
		get;
		private set;
	}

	public static float OceanLevel
	{
		get;
		private set;
	}

	public static HashSet<WaterBody> WaterBodies
	{
		get;
		private set;
	}

	public static float WaveTime
	{
		get;
		private set;
	}

	static WaterSystem()
	{
		WaterSystem.Ocean = null;
		WaterSystem.WaterBodies = new HashSet<WaterBody>();
		WaterSystem.OceanLevel = 0f;
		WaterSystem.WaveTime = 0f;
	}

	public WaterSystem()
	{
	}

	public void Awake()
	{
		this.CheckInstance();
	}

	private void CheckInstance()
	{
		WaterSystem.instance = (WaterSystem.instance != null ? WaterSystem.instance : this);
		WaterSystem.Collision = (WaterSystem.Collision != null ? WaterSystem.Collision : base.GetComponent<WaterCollision>());
		WaterSystem.Dynamics = (WaterSystem.Dynamics != null ? WaterSystem.Dynamics : base.GetComponent<WaterDynamics>());
	}

	public void GenerateWaves()
	{
		this.GerstnerWaves = WaterGerstner.SetupWaves(this.Simulation.Wind, this.Simulation.GerstnerWaves);
	}

	public static float GetHeight(Vector3 pos)
	{
		float single;
		return WaterSystem.GetHeight(pos, out single);
	}

	public static float GetHeight(Vector3 pos, out float terrainHeight)
	{
		Vector2 vector2 = new Vector2();
		vector2.x = (pos.x - TerrainMeta.Position.x) * TerrainMeta.OneOverSize.x;
		vector2.y = (pos.z - TerrainMeta.Position.z) * TerrainMeta.OneOverSize.z;
		return WaterSystem.GetHeight(pos, vector2, out terrainHeight);
	}

	public static float GetHeight(Vector3 pos, Vector2 posUV, out float terrainHeight)
	{
		float single = (TerrainMeta.WaterMap != null ? TerrainMeta.WaterMap.GetHeightFast(posUV) : 0f);
		float single1 = (WaterSystem.Instance != null ? WaterSystem.Ocean.Transform.position.y : 0f);
		terrainHeight = (TerrainMeta.HeightMap != null ? TerrainMeta.HeightMap.GetHeight(pos) : 0f);
		if (WaterSystem.instance != null && WaterSystem.instance.GerstnerWaves != null && (double)single <= (double)single1 + 0.01)
		{
			float single2 = Mathf.Clamp01(Mathf.Abs(single1 - terrainHeight) * 0.1f);
			single = WaterGerstner.SampleHeight(WaterSystem.instance.GerstnerWaves, pos) * single2;
		}
		return single;
	}

	public static void GetHeight(Vector2[] pos, Vector2[] posUV, float[] terrainHeight, float[] waterHeight)
	{
		Debug.Assert((int)pos.Length == (int)posUV.Length);
		Debug.Assert((int)pos.Length == (int)terrainHeight.Length);
		Debug.Assert((int)pos.Length == (int)waterHeight.Length);
		float single = (WaterSystem.Instance != null ? WaterSystem.Ocean.Transform.position.y : 0f);
		bool flag = (WaterSystem.instance == null ? false : WaterSystem.instance.GerstnerWaves != null);
		if (flag)
		{
			WaterGerstner.SampleHeightArray(WaterSystem.instance.GerstnerWaves, pos, waterHeight);
		}
		for (int i = 0; i < (int)pos.Length; i++)
		{
			Vector2 vector2 = posUV[i];
			terrainHeight[i] = (TerrainMeta.HeightMap != null ? TerrainMeta.HeightMap.GetHeightFast(vector2) : 0f);
			float single1 = (TerrainMeta.WaterMap != null ? TerrainMeta.WaterMap.GetHeightFast(vector2) : 0f);
			if (!flag || (double)single1 > (double)single + 0.01)
			{
				waterHeight[i] = single1;
			}
			else
			{
				float single2 = Mathf.Clamp01(Mathf.Abs(single - terrainHeight[i]) * 0.1f);
				waterHeight[i] *= single2;
			}
		}
	}

	public static Vector3 GetNormal(Vector3 pos)
	{
		return ((TerrainMeta.WaterMap != null ? TerrainMeta.WaterMap.GetNormal(pos) : Vector3.up)).normalized;
	}

	public static void RegisterBody(WaterBody body)
	{
		if (body.Type == WaterBodyType.Ocean)
		{
			if (WaterSystem.Ocean == null)
			{
				WaterSystem.Ocean = body;
				WaterSystem.OceanLevel = body.Transform.position.y;
			}
			else if (WaterSystem.Ocean != body)
			{
				Debug.LogWarning("[Water] Ocean body is already registered. Ignoring call because only one is allowed.");
				return;
			}
		}
		WaterSystem.WaterBodies.Add(body);
	}

	public static void UnregisterBody(WaterBody body)
	{
		WaterSystem.WaterBodies.Remove(body);
	}

	private void Update()
	{
		this.UpdateWaveTime();
	}

	private void UpdateWaveTime()
	{
		WaterSystem.WaveTime = (this.ProgressTime ? Time.realtimeSinceStartup : WaterSystem.WaveTime);
	}

	[Serializable]
	public class RenderingSettings
	{
		public float MaxDisplacementDistance;

		public WaterSystem.RenderingSettings.SkyProbe SkyReflections;

		public WaterSystem.RenderingSettings.SSR ScreenSpaceReflections;

		public WaterSystem.RenderingSettings.Caustics CausticsAnimation;

		public RenderingSettings()
		{
		}

		[Serializable]
		public class Caustics
		{
			public float FrameRate;

			public Texture2D[] FramesShallow;

			public Texture2D[] FramesDeep;

			public Caustics()
			{
			}
		}

		[Serializable]
		public class SkyProbe
		{
			public float ProbeUpdateInterval;

			public bool TimeSlicing;

			public SkyProbe()
			{
			}
		}

		[Serializable]
		public class SSR
		{
			public float FresnelCutoff;

			public float ThicknessMin;

			public float ThicknessMax;

			public float ThicknessStartDist;

			public float ThicknessEndDist;

			public SSR()
			{
			}
		}
	}

	[Serializable]
	public class SimulationSettings
	{
		public Vector3 Wind;

		public int SolverResolution;

		public float SolverSizeInWorld;

		public float Gravity;

		public float Amplitude;

		public TextAsset PerlinNoiseData;

		public WaterGerstner.WaveSettings GerstnerWaves;

		public SimulationSettings()
		{
		}
	}

	public struct WaveSample
	{
		public Vector3 position;

		public Vector3 normal;
	}
}