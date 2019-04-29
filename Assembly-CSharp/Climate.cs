using System;
using UnityEngine;

public class Climate : SingletonComponent<Climate>
{
	private const float fadeAngle = 20f;

	private const float defaultTemp = 15f;

	private const int weatherDurationHours = 18;

	private const int weatherFadeHours = 6;

	[Range(0f, 1f)]
	public float BlendingSpeed = 1f;

	[Range(1f, 9f)]
	public float FogMultiplier = 5f;

	public float FogDarknessDistance = 200f;

	public bool DebugLUTBlending;

	public Climate.WeatherParameters Weather;

	public Climate.ClimateParameters Arid;

	public Climate.ClimateParameters Temperate;

	public Climate.ClimateParameters Tundra;

	public Climate.ClimateParameters Arctic;

	private Climate.ClimateParameters[] climates;

	private Climate.WeatherState state = new Climate.WeatherState()
	{
		Clouds = 0f,
		Fog = 0f,
		Wind = 0f,
		Rain = 0f
	};

	private Climate.WeatherState clamps = new Climate.WeatherState()
	{
		Clouds = -1f,
		Fog = -1f,
		Wind = -1f,
		Rain = -1f
	};

	public Climate.WeatherState Overrides = new Climate.WeatherState()
	{
		Clouds = -1f,
		Fog = -1f,
		Wind = -1f,
		Rain = -1f
	};

	public Climate()
	{
	}

	private float FindBlendParameters(Vector3 pos, out Climate.ClimateParameters src, out Climate.ClimateParameters dst)
	{
		if (this.climates == null)
		{
			this.climates = new Climate.ClimateParameters[] { this.Arid, this.Temperate, this.Tundra, this.Arctic };
		}
		if (TerrainMeta.BiomeMap == null)
		{
			src = null;
			dst = null;
			return 0.5f;
		}
		int biomeMaxType = TerrainMeta.BiomeMap.GetBiomeMaxType(pos, -1);
		int num = TerrainMeta.BiomeMap.GetBiomeMaxType(pos, ~biomeMaxType);
		src = this.climates[TerrainBiome.TypeToIndex(biomeMaxType)];
		dst = this.climates[TerrainBiome.TypeToIndex(num)];
		return TerrainMeta.BiomeMap.GetBiome(pos, num);
	}

	public static float GetCloudOpacity(Vector3 position)
	{
		if (!SingletonComponent<Climate>.Instance)
		{
			return 1f;
		}
		return Mathf.InverseLerp(0.9f, 0.8f, Climate.GetFog(position));
	}

	public static float GetClouds(Vector3 position)
	{
		if (!SingletonComponent<Climate>.Instance)
		{
			return 0f;
		}
		return Mathf.Max(SingletonComponent<Climate>.Instance.clamps.Clouds, SingletonComponent<Climate>.Instance.state.Clouds);
	}

	public static float GetFog(Vector3 position)
	{
		if (!SingletonComponent<Climate>.Instance)
		{
			return 0f;
		}
		return Mathf.Max(SingletonComponent<Climate>.Instance.clamps.Fog, SingletonComponent<Climate>.Instance.state.Fog);
	}

	public static float GetRain(Vector3 position)
	{
		if (!SingletonComponent<Climate>.Instance)
		{
			return 0f;
		}
		float single = (TerrainMeta.BiomeMap ? TerrainMeta.BiomeMap.GetBiome(position, 1) : 0f);
		float single1 = (TerrainMeta.BiomeMap ? TerrainMeta.BiomeMap.GetBiome(position, 8) : 0f);
		return Mathf.Max(SingletonComponent<Climate>.Instance.clamps.Rain, SingletonComponent<Climate>.Instance.state.Rain) * Mathf.Lerp(1f, 0.5f, single) * (1f - single1);
	}

	public static float GetSnow(Vector3 position)
	{
		if (!SingletonComponent<Climate>.Instance)
		{
			return 0f;
		}
		float single = (TerrainMeta.BiomeMap ? TerrainMeta.BiomeMap.GetBiome(position, 8) : 0f);
		return Mathf.Max(SingletonComponent<Climate>.Instance.clamps.Rain, SingletonComponent<Climate>.Instance.state.Rain) * single;
	}

	public static float GetTemperature(Vector3 position)
	{
		Climate.ClimateParameters climateParameter;
		Climate.ClimateParameters climateParameter1;
		if (!SingletonComponent<Climate>.Instance)
		{
			return 15f;
		}
		if (!TOD_Sky.Instance)
		{
			return 15f;
		}
		float single = SingletonComponent<Climate>.Instance.FindBlendParameters(position, out climateParameter, out climateParameter1);
		if (climateParameter == null || climateParameter1 == null)
		{
			return 15f;
		}
		float hour = TOD_Sky.Instance.Cycle.Hour;
		float single1 = climateParameter.Temperature.Evaluate(hour);
		float single2 = climateParameter1.Temperature.Evaluate(hour);
		return Mathf.Lerp(single1, single2, single);
	}

	private Climate.WeatherState GetWeatherState(uint seed)
	{
		object obj;
		object obj1;
		SeedRandom.Wanghash(ref seed);
		bool flag = SeedRandom.Value(ref seed) < this.Weather.CloudChance;
		bool flag1 = SeedRandom.Value(ref seed) < this.Weather.FogChance;
		bool flag2 = SeedRandom.Value(ref seed) < this.Weather.RainChance;
		bool flag3 = SeedRandom.Value(ref seed) < this.Weather.StormChance;
		float single = (flag ? SeedRandom.Value(ref seed) : 0f);
		if (flag1)
		{
			obj = 1;
		}
		else
		{
			obj = null;
		}
		float single1 = (float)obj;
		if (flag2)
		{
			obj1 = 1;
		}
		else
		{
			obj1 = null;
		}
		float single2 = (float)obj1;
		float single3 = (flag3 ? SeedRandom.Value(ref seed) : 0f);
		if (single2 > 0f)
		{
			single2 = Mathf.Max(single2, 0.5f);
			single1 = Mathf.Max(single1, single2);
			single = Mathf.Max(single, single2);
		}
		Climate.WeatherState weatherState = new Climate.WeatherState()
		{
			Clouds = single,
			Fog = single1,
			Wind = single3,
			Rain = single2
		};
		return weatherState;
	}

	public static float GetWind(Vector3 position)
	{
		if (!SingletonComponent<Climate>.Instance)
		{
			return 0f;
		}
		return Mathf.Max(SingletonComponent<Climate>.Instance.clamps.Wind, SingletonComponent<Climate>.Instance.state.Wind);
	}

	protected void Update()
	{
		if (!TerrainMeta.BiomeMap || !TOD_Sky.Instance)
		{
			return;
		}
		TOD_Sky instance = TOD_Sky.Instance;
		long num = 36000000000L;
		long seed = (long)((ulong)World.Seed + instance.Cycle.Ticks);
		long num1 = (long)18 * num;
		long num2 = (long)6 * num;
		long num3 = seed / num1;
		float single = Mathf.InverseLerp(0f, (float)num2, (float)(seed % num1));
		Climate.WeatherState weatherState = this.GetWeatherState((uint)(num3 % (ulong)-1));
		Climate.WeatherState weatherState1 = this.GetWeatherState((uint)((num3 + (long)1) % (ulong)-1));
		this.state = Climate.WeatherState.Fade(weatherState, weatherState1, single);
		this.state.Override(this.Overrides);
	}

	[Serializable]
	public class ClimateParameters
	{
		public AnimationCurve Temperature;

		[Horizontal(4, -1)]
		public Climate.Float4 AerialDensity;

		[Horizontal(4, -1)]
		public Climate.Float4 FogDensity;

		[Horizontal(4, -1)]
		public Climate.Texture2D4 LUT;

		public ClimateParameters()
		{
		}
	}

	[Serializable]
	public class Color4 : Climate.Value4<Color>
	{
		public Color4()
		{
		}
	}

	[Serializable]
	public class Float4 : Climate.Value4<float>
	{
		public Float4()
		{
		}
	}

	[Serializable]
	public class Texture2D4 : Climate.Value4<Texture2D>
	{
		public Texture2D4()
		{
		}
	}

	public class Value4<T>
	{
		public T Dawn;

		public T Noon;

		public T Dusk;

		public T Night;

		public Value4()
		{
		}

		public float FindBlendParameters(TOD_Sky sky, out T src, out T dst)
		{
			float single = Mathf.Abs(sky.SunriseTime - sky.Cycle.Hour);
			float single1 = Mathf.Abs(sky.SunsetTime - sky.Cycle.Hour);
			float sunZenith = (180f - sky.SunZenith) / 180f;
			float single2 = 0.111111112f;
			if (single < single1)
			{
				if (sunZenith < 0.5f)
				{
					src = this.Night;
					dst = this.Dawn;
					return Mathf.InverseLerp(0.5f - single2, 0.5f, sunZenith);
				}
				src = this.Dawn;
				dst = this.Noon;
				return Mathf.InverseLerp(0.5f, 0.5f + single2, sunZenith);
			}
			if (sunZenith > 0.5f)
			{
				src = this.Noon;
				dst = this.Dusk;
				return Mathf.InverseLerp(0.5f + single2, 0.5f, sunZenith);
			}
			src = this.Dusk;
			dst = this.Night;
			return Mathf.InverseLerp(0.5f, 0.5f - single2, sunZenith);
		}
	}

	[Serializable]
	public class WeatherParameters
	{
		[Range(0f, 1f)]
		public float RainChance;

		[Range(0f, 1f)]
		public float FogChance;

		[Range(0f, 1f)]
		public float CloudChance;

		[Range(0f, 1f)]
		public float StormChance;

		public WeatherParameters()
		{
		}
	}

	public struct WeatherState
	{
		public float Clouds;

		public float Fog;

		public float Wind;

		public float Rain;

		public static Climate.WeatherState Fade(Climate.WeatherState a, Climate.WeatherState b, float t)
		{
			Climate.WeatherState weatherState = new Climate.WeatherState()
			{
				Clouds = Mathf.SmoothStep(a.Clouds, b.Clouds, t),
				Fog = Mathf.SmoothStep(a.Fog, b.Fog, t),
				Wind = Mathf.SmoothStep(a.Wind, b.Wind, t),
				Rain = Mathf.SmoothStep(a.Rain, b.Rain, t)
			};
			return weatherState;
		}

		public void Max(Climate.WeatherState other)
		{
			this.Clouds = Mathf.Max(this.Clouds, other.Clouds);
			this.Fog = Mathf.Max(this.Fog, other.Fog);
			this.Wind = Mathf.Max(this.Wind, other.Wind);
			this.Rain = Mathf.Max(this.Rain, other.Rain);
		}

		public void Override(Climate.WeatherState other)
		{
			if (other.Clouds >= 0f)
			{
				this.Clouds = Mathf.Clamp01(other.Clouds);
			}
			if (other.Fog >= 0f)
			{
				this.Fog = Mathf.Clamp01(other.Fog);
			}
			if (other.Wind >= 0f)
			{
				this.Wind = Mathf.Clamp01(other.Wind);
			}
			if (other.Rain >= 0f)
			{
				this.Rain = Mathf.Clamp01(other.Rain);
			}
		}
	}
}