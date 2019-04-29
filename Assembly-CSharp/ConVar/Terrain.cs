using System;

namespace ConVar
{
	[Factory("terrain")]
	public class Terrain : ConsoleSystem
	{
		[ClientVar(Saved=true)]
		public static float quality;

		static Terrain()
		{
			Terrain.quality = 100f;
		}

		public Terrain()
		{
		}
	}
}