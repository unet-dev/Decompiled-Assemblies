using System;
using System.IO;

namespace ConVar
{
	[Factory("profile")]
	public class Profile : ConsoleSystem
	{
		public Profile()
		{
		}

		private static void NeedProfileFolder()
		{
			if (!Directory.Exists("profile"))
			{
				Directory.CreateDirectory("profile");
			}
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