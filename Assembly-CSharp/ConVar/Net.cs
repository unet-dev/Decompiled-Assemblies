using System;

namespace ConVar
{
	[Factory("net")]
	public class Net : ConsoleSystem
	{
		[ServerVar]
		public static bool visdebug;

		[ClientVar]
		public static bool debug;

		static Net()
		{
		}

		public Net()
		{
		}
	}
}