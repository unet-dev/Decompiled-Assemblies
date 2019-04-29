using System;

namespace ConVar
{
	[Factory("voice")]
	public class Voice : ConsoleSystem
	{
		[ClientVar(Saved=true)]
		public static bool loopback;

		[ClientVar]
		public static float ui_scale;

		[ClientVar]
		public static float ui_cut;

		[ClientVar]
		public static int ui_samples;

		[ClientVar]
		public static float ui_lerp;

		static Voice()
		{
			Voice.loopback = false;
			Voice.ui_scale = 1f;
			Voice.ui_cut = 0f;
			Voice.ui_samples = 20;
			Voice.ui_lerp = 0.2f;
		}

		public Voice()
		{
		}
	}
}