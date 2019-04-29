using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Apex.Utilities
{
	public static class DebugTimer
	{
		private static Stack<Stopwatch> _watches;

		private static Stopwatch _avgWatch;

		private static float _iterations;

		private static int _count;

		private static float _avg;

		static DebugTimer()
		{
			DebugTimer._watches = new Stack<Stopwatch>();
		}

		[Conditional("UNITY_EDITOR")]
		public static void EndAverageMilliseconds(string label)
		{
			DebugTimer._avgWatch.Stop();
			float elapsedMilliseconds = (float)DebugTimer._avgWatch.ElapsedMilliseconds / DebugTimer._iterations;
			if ((float)DebugTimer._count < DebugTimer._iterations)
			{
				DebugTimer._avg += elapsedMilliseconds;
			}
			int num = DebugTimer._count - 1;
			DebugTimer._count = num;
			if (num == 0)
			{
				UnityEngine.Debug.Log(string.Format(label, DebugTimer._avg));
			}
		}

		[Conditional("UNITY_EDITOR")]
		public static void EndAverageTicks(string label)
		{
			DebugTimer._avgWatch.Stop();
			float elapsedTicks = (float)DebugTimer._avgWatch.ElapsedTicks / DebugTimer._iterations;
			if ((float)DebugTimer._count < DebugTimer._iterations)
			{
				DebugTimer._avg += elapsedTicks;
			}
			int num = DebugTimer._count - 1;
			DebugTimer._count = num;
			if (num == 0)
			{
				UnityEngine.Debug.Log(string.Format(label, DebugTimer._avg));
			}
		}

		[Conditional("UNITY_EDITOR")]
		public static void EndMilliseconds(string label)
		{
			DebugTimer._watches.Peek().Stop();
			Stopwatch stopwatch = DebugTimer._watches.Pop();
			UnityEngine.Debug.Log(string.Format(label, stopwatch.ElapsedMilliseconds));
		}

		[Conditional("UNITY_EDITOR")]
		public static void EndTicks(string label)
		{
			DebugTimer._watches.Peek().Stop();
			Stopwatch stopwatch = DebugTimer._watches.Pop();
			UnityEngine.Debug.Log(string.Format(label, stopwatch.ElapsedTicks));
		}

		[Conditional("UNITY_EDITOR")]
		public static void Start()
		{
			Stopwatch stopwatch = new Stopwatch();
			DebugTimer._watches.Push(stopwatch);
			stopwatch.Start();
		}

		[Conditional("UNITY_EDITOR")]
		public static void StartAverage(int iterations)
		{
			if (DebugTimer._count > 0)
			{
				DebugTimer._avgWatch.Reset();
				DebugTimer._avgWatch.Start();
				return;
			}
			DebugTimer._avg = 0f;
			int num = iterations;
			DebugTimer._count = num;
			DebugTimer._iterations = (float)num;
			DebugTimer._avgWatch = Stopwatch.StartNew();
		}
	}
}