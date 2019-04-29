using System;
using System.Linq;
using UnityEngine;

namespace ConVar
{
	[Factory("stability")]
	public class Stability : ConsoleSystem
	{
		[ServerVar]
		public static int verbose;

		[ServerVar]
		public static int strikes;

		[ServerVar]
		public static float collapse;

		[ServerVar]
		public static float accuracy;

		[ServerVar]
		public static float stabilityqueue;

		[ServerVar]
		public static float surroundingsqueue;

		static Stability()
		{
			Stability.verbose = 0;
			Stability.strikes = 10;
			Stability.collapse = 0.05f;
			Stability.accuracy = 0.001f;
			Stability.stabilityqueue = 9f;
			Stability.surroundingsqueue = 3f;
		}

		public Stability()
		{
		}

		[ServerVar]
		public static void refresh_stability(ConsoleSystem.Arg args)
		{
			StabilityEntity[] array = BaseNetworkable.serverEntities.OfType<StabilityEntity>().ToArray<StabilityEntity>();
			Debug.Log(string.Concat("Refreshing stability on ", (int)array.Length, " entities..."));
			for (int i = 0; i < (int)array.Length; i++)
			{
				array[i].UpdateStability();
			}
		}
	}
}