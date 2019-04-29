using System;
using UnityEngine;

namespace ConVar
{
	[Factory("weather")]
	public class Weather : ConsoleSystem
	{
		public Weather()
		{
		}

		[ServerVar]
		public static void clouds(ConsoleSystem.Arg args)
		{
			if (SingletonComponent<Climate>.Instance == null)
			{
				return;
			}
			float clouds = SingletonComponent<Climate>.Instance.Overrides.Clouds;
			float num = args.GetFloat(0, -1f);
			string str = (clouds < 0f ? "automatic" : string.Concat(Mathf.RoundToInt(100f * clouds), "%"));
			string str1 = (num < 0f ? "automatic" : string.Concat(Mathf.RoundToInt(100f * num), "%"));
			args.ReplyWith(string.Concat(new string[] { "Clouds: ", str1, " (was ", str, ")" }));
			SingletonComponent<Climate>.Instance.Overrides.Clouds = num;
		}

		[ServerVar]
		public static void fog(ConsoleSystem.Arg args)
		{
			if (SingletonComponent<Climate>.Instance == null)
			{
				return;
			}
			float fog = SingletonComponent<Climate>.Instance.Overrides.Fog;
			float num = args.GetFloat(0, -1f);
			string str = (fog < 0f ? "automatic" : string.Concat(Mathf.RoundToInt(100f * fog), "%"));
			string str1 = (num < 0f ? "automatic" : string.Concat(Mathf.RoundToInt(100f * num), "%"));
			args.ReplyWith(string.Concat(new string[] { "Fog: ", str1, " (was ", str, ")" }));
			SingletonComponent<Climate>.Instance.Overrides.Fog = num;
		}

		[ServerVar]
		public static void rain(ConsoleSystem.Arg args)
		{
			if (SingletonComponent<Climate>.Instance == null)
			{
				return;
			}
			float rain = SingletonComponent<Climate>.Instance.Overrides.Rain;
			float num = args.GetFloat(0, -1f);
			string str = (rain < 0f ? "automatic" : string.Concat(Mathf.RoundToInt(100f * rain), "%"));
			string str1 = (num < 0f ? "automatic" : string.Concat(Mathf.RoundToInt(100f * num), "%"));
			args.ReplyWith(string.Concat(new string[] { "Rain: ", str1, " (was ", str, ")" }));
			SingletonComponent<Climate>.Instance.Overrides.Rain = num;
		}

		[ServerVar]
		public static void wind(ConsoleSystem.Arg args)
		{
			if (SingletonComponent<Climate>.Instance == null)
			{
				return;
			}
			float wind = SingletonComponent<Climate>.Instance.Overrides.Wind;
			float num = args.GetFloat(0, -1f);
			string str = (wind < 0f ? "automatic" : string.Concat(Mathf.RoundToInt(100f * wind), "%"));
			string str1 = (num < 0f ? "automatic" : string.Concat(Mathf.RoundToInt(100f * num), "%"));
			args.ReplyWith(string.Concat(new string[] { "Wind: ", str1, " (was ", str, ")" }));
			SingletonComponent<Climate>.Instance.Overrides.Wind = num;
		}
	}
}