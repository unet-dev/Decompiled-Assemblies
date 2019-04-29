using System;
using UnityEngine;

public class Performance : SingletonComponent<Performance>
{
	public static Performance.Tick current;

	public static Performance.Tick report;

	private static long cycles;

	private static int[] frameRateHistory;

	private static float[] frameTimeHistory;

	private int frames;

	private float time;

	static Performance()
	{
		Performance.cycles = (long)0;
		Performance.frameRateHistory = new int[60];
		Performance.frameTimeHistory = new float[60];
	}

	public Performance()
	{
	}

	private float AverageFrameRate()
	{
		float single = 0f;
		for (int i = 0; i < (int)Performance.frameRateHistory.Length; i++)
		{
			single += (float)Performance.frameRateHistory[i];
		}
		return single / (float)((int)Performance.frameRateHistory.Length);
	}

	private float AverageFrameTime()
	{
		float single = 0f;
		for (int i = 0; i < (int)Performance.frameTimeHistory.Length; i++)
		{
			single += Performance.frameTimeHistory[i];
		}
		return single / (float)((int)Performance.frameTimeHistory.Length);
	}

	private void FPSTimer()
	{
		this.frames++;
		this.time += Time.unscaledDeltaTime;
		if (this.time < 1f)
		{
			return;
		}
		Performance.current.frameRate = this.frames;
		Performance.current.frameTime = this.time / (float)this.frames * 1000f;
		Performance.frameRateHistory[checked((IntPtr)(Performance.cycles % (long)((int)Performance.frameRateHistory.Length)))] = Performance.current.frameRate;
		Performance.frameTimeHistory[checked((IntPtr)(Performance.cycles % (long)((int)Performance.frameTimeHistory.Length)))] = Performance.current.frameTime;
		Performance.current.frameRateAverage = this.AverageFrameRate();
		Performance.current.frameTimeAverage = this.AverageFrameTime();
		Performance.current.memoryUsageSystem = (long)SystemInfoEx.systemMemoryUsed;
		Performance.current.memoryAllocations = GC.GetTotalMemory(false) / (long)1048576;
		Performance.current.memoryCollections = (long)GC.CollectionCount(0);
		Performance.current.loadBalancerTasks = (long)LoadBalancer.Count();
		Performance.current.invokeHandlerTasks = (long)InvokeHandler.Count();
		this.frames = 0;
		this.time = 0f;
		Performance.cycles += (long)1;
		Performance.report = Performance.current;
	}

	private void Update()
	{
		using (TimeWarning timeWarning = TimeWarning.New("FPSTimer", 0.1f))
		{
			this.FPSTimer();
		}
	}

	public struct Tick
	{
		public int frameRate;

		public float frameTime;

		public float frameRateAverage;

		public float frameTimeAverage;

		public long memoryUsageSystem;

		public long memoryAllocations;

		public long memoryCollections;

		public long loadBalancerTasks;

		public long invokeHandlerTasks;

		public int ping;
	}
}