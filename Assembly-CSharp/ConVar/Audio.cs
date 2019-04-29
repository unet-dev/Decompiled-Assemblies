using System;
using UnityEngine;

namespace ConVar
{
	[Factory("audio")]
	public class Audio : ConsoleSystem
	{
		[ClientVar(Help="Volume", Saved=true)]
		public static float master;

		[ClientVar(Help="Volume", Saved=true)]
		public static float musicvolume;

		[ClientVar(Help="Volume", Saved=true)]
		public static float musicvolumemenu;

		[ClientVar(Help="Volume", Saved=true)]
		public static float game;

		[ClientVar(Help="Volume", Saved=true)]
		public static float voices;

		[ClientVar(Help="Ambience System")]
		public static bool ambience;

		[ClientVar(Help="Max ms per frame to spend updating sounds")]
		public static float framebudget;

		[ClientVar(Help="Use more advanced sound occlusion", Saved=true)]
		public static bool advancedocclusion;

		[ClientVar(Help="Volume", Saved=true)]
		public static int speakers
		{
			get
			{
				return (int)AudioSettings.speakerMode;
			}
			set
			{
				value = Mathf.Clamp(value, 2, 7);
				AudioConfiguration configuration = AudioSettings.GetConfiguration();
				configuration.speakerMode = (AudioSpeakerMode)value;
				using (TimeWarning timeWarning = TimeWarning.New("Audio Settings Reset", 0.25f))
				{
					AudioSettings.Reset(configuration);
				}
			}
		}

		static Audio()
		{
			Audio.master = 1f;
			Audio.musicvolume = 1f;
			Audio.musicvolumemenu = 1f;
			Audio.game = 1f;
			Audio.voices = 1f;
			Audio.ambience = true;
			Audio.framebudget = 0.3f;
			Audio.advancedocclusion = false;
		}

		public Audio()
		{
		}
	}
}