using System;

namespace ConVar
{
	[Factory("profile")]
	public class Profile : ConsoleSystem
	{
		public Profile()
		{
		}

		[ClientVar]
		[ServerVar]
		public static void start(ConsoleSystem.Arg arg)
		{
		}

		[ClientVar]
		[ServerVar]
		public static void stop(ConsoleSystem.Arg arg)
		{
		}
	}
}