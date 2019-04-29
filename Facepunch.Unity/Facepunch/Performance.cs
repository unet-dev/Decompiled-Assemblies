using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Facepunch
{
	public static class Performance
	{
		public static Func<int> GetMemoryUsage;

		public static Func<int> GetGarbageCollections;

		private static System.Diagnostics.Stopwatch Stopwatch;

		private static int frames;

		public static int TargetFrameRate;

		private static int[] frameBuckets;

		private static float[] frameBucketFractions;

		public static double AvgFrameTimeLastSecond
		{
			get
			{
				return (double)((Performance.FrameCountLastSecond > 0 ? (float)(1000 / Performance.FrameCountLastSecond) : 1f));
			}
		}

		public static int[] CategorizedFrameCount
		{
			get
			{
				return Performance.frameBuckets;
			}
		}

		public static int FrameCountLastSecond
		{
			get;
			private set;
		}

		public static Facepunch.FrameRateCategory FrameRateCategory
		{
			get
			{
				return Performance.CategorizeFrameRate(Performance.FrameCountLastSecond);
			}
		}

		public static int GarbageCollections
		{
			get;
			private set;
		}

		public static int MemoryUsage
		{
			get;
			private set;
		}

		public static float SecondsSinceLastConnection
		{
			get;
			private set;
		}

		static Performance()
		{
			Performance.GetMemoryUsage = null;
			Performance.GetGarbageCollections = null;
			Performance.Stopwatch = System.Diagnostics.Stopwatch.StartNew();
			Performance.TargetFrameRate = 60;
			Performance.frameBuckets = new int[6];
			Performance.frameBucketFractions = new float[6];
		}

		private static Facepunch.FrameRateCategory CategorizeFrameRate(int i)
		{
			if (i < Performance.TargetFrameRate / 4)
			{
				return Facepunch.FrameRateCategory.Unplayable;
			}
			if (i < Performance.TargetFrameRate / 2)
			{
				return Facepunch.FrameRateCategory.VeryBad;
			}
			if (i < Performance.TargetFrameRate - 10)
			{
				return Facepunch.FrameRateCategory.Bad;
			}
			if (i < Performance.TargetFrameRate + 10)
			{
				return Facepunch.FrameRateCategory.Average;
			}
			if (i < Performance.TargetFrameRate + 30)
			{
				return Facepunch.FrameRateCategory.Good;
			}
			return Facepunch.FrameRateCategory.VeryGood;
		}

		internal static void Frame()
		{
			Performance.frames++;
			if (Performance.Stopwatch.Elapsed.TotalSeconds >= 1)
			{
				Performance.OneSecond(Performance.Stopwatch.Elapsed.TotalSeconds);
				Performance.Stopwatch.Reset();
				Performance.Stopwatch.Start();
			}
		}

		public static int GetFrameCount(Facepunch.FrameRateCategory category)
		{
			return Performance.frameBuckets[(int)category];
		}

		public static float GetFrameFraction(Facepunch.FrameRateCategory category)
		{
			return Performance.frameBucketFractions[(int)category];
		}

		private static void OneSecond(double timelapse)
		{
			Performance.FrameCountLastSecond = Performance.frames;
			Performance.frames = 0;
			Performance.MemoryUsage = (Performance.GetMemoryUsage != null ? Performance.GetMemoryUsage() : (int)(GC.GetTotalMemory(false) / (long)1024 / (long)1024));
			Performance.GarbageCollections = (Performance.GetGarbageCollections != null ? Performance.GetGarbageCollections() : GC.CollectionCount(0));
			Performance.UpdateFrameBuckets();
		}

		private static void UpdateFrameBuckets()
		{
			Performance.frameBuckets[(int)Performance.FrameRateCategory]++;
			int num = 0;
			for (int i = 0; i < (int)Performance.frameBuckets.Length; i++)
			{
				num += Performance.frameBuckets[i];
			}
			for (int j = 0; j < (int)Performance.frameBuckets.Length; j++)
			{
				Performance.frameBucketFractions[j] = (float)((float)Performance.frameBuckets[j] / (float)num);
			}
		}
	}
}