using System;

namespace ConVar
{
	[Factory("SSS")]
	public class SSS : ConsoleSystem
	{
		[ClientVar(Saved=true)]
		public static bool enabled;

		[ClientVar(Saved=true)]
		public static int quality;

		[ClientVar(Saved=true)]
		public static bool halfres;

		[ClientVar(Saved=true)]
		public static float scale;

		static SSS()
		{
			SSS.enabled = true;
			SSS.quality = 0;
			SSS.halfres = true;
			SSS.scale = 1f;
		}

		public SSS()
		{
		}
	}
}