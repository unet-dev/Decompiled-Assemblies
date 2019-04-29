using System;
using System.Text;
using UnityEngine;

namespace ConVar
{
	[Factory("music")]
	public class Music : ConsoleSystem
	{
		[ClientVar]
		public static bool enabled;

		[ClientVar]
		public static int songGapMin;

		[ClientVar]
		public static int songGapMax;

		static Music()
		{
			Music.enabled = true;
			Music.songGapMin = 240;
			Music.songGapMax = 480;
		}

		public Music()
		{
		}

		[ClientVar]
		public static void info(ConsoleSystem.Arg arg)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (SingletonComponent<MusicManager>.Instance != null)
			{
				stringBuilder.Append("Current music info: ");
				stringBuilder.AppendLine();
				stringBuilder.Append(string.Concat("  theme: ", SingletonComponent<MusicManager>.Instance.currentTheme));
				stringBuilder.AppendLine();
				stringBuilder.Append(string.Concat("  intensity: ", SingletonComponent<MusicManager>.Instance.intensity));
				stringBuilder.AppendLine();
				stringBuilder.Append(string.Concat("  next music: ", SingletonComponent<MusicManager>.Instance.nextMusic));
				stringBuilder.AppendLine();
				stringBuilder.Append(string.Concat("  current time: ", UnityEngine.Time.time));
				stringBuilder.AppendLine();
			}
			else
			{
				stringBuilder.Append("No music manager was found");
			}
			arg.ReplyWith(stringBuilder.ToString());
		}
	}
}