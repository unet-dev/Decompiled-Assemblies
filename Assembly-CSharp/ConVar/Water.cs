using System;

namespace ConVar
{
	[Factory("water")]
	public class Water : ConsoleSystem
	{
		[ClientVar(Saved=true)]
		public static int quality;

		[ClientVar(Saved=true)]
		public static int reflections;

		static Water()
		{
			Water.quality = 1;
			Water.reflections = 1;
		}

		public Water()
		{
		}
	}
}