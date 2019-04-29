using System;

namespace Oxide.Core
{
	public static class Random
	{
		private readonly static System.Random random;

		static Random()
		{
			Oxide.Core.Random.random = new System.Random();
		}

		public static int Range(int min, int max)
		{
			return Oxide.Core.Random.random.Next(min, max);
		}

		public static int Range(int max)
		{
			return Oxide.Core.Random.random.Next(max);
		}

		public static double Range(double min, double max)
		{
			return min + Oxide.Core.Random.random.NextDouble() * (max - min);
		}

		public static float Range(float min, float max)
		{
			return (float)Oxide.Core.Random.Range((double)min, (double)max);
		}
	}
}