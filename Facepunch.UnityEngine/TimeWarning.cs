using Facepunch;
using System;
using System.Diagnostics;
using UnityEngine;

public class TimeWarning : IDisposable
{
	public static bool Enabled;

	public static bool EnableScopeCalls;

	public static Action<string> OnBegin;

	public static Action OnEnd;

	private Stopwatch stopwatch = new Stopwatch();

	private string warningName;

	private double warningMS;

	private int gcCount;

	private bool disposed;

	static TimeWarning()
	{
	}

	public TimeWarning()
	{
	}

	public static void BeginSample(string name)
	{
		if (TimeWarning.OnBegin != null)
		{
			TimeWarning.OnBegin(name);
		}
	}

	public static void EndSample()
	{
		if (TimeWarning.OnEnd != null)
		{
			TimeWarning.OnEnd();
		}
	}

	public static TimeWarning New(string name, float maxSeconds = 0.1f)
	{
		if (!TimeWarning.Enabled && !TimeWarning.EnableScopeCalls)
		{
			return null;
		}
		TimeWarning timeWarning = Pool.Get<TimeWarning>();
		timeWarning.Start(name, maxSeconds);
		return timeWarning;
	}

	public static TimeWarning New(string name, long maxMilliseconds)
	{
		if (!TimeWarning.Enabled && !TimeWarning.EnableScopeCalls)
		{
			return null;
		}
		TimeWarning timeWarning = Pool.Get<TimeWarning>();
		timeWarning.Start(name, maxMilliseconds);
		return timeWarning;
	}

	private void Start(string name, float maxSeconds = 0.1f)
	{
		if (TimeWarning.Enabled)
		{
			this.warningName = name;
			this.warningMS = (double)(maxSeconds * 1000f);
			this.stopwatch.Reset();
			this.stopwatch.Start();
			this.gcCount = GC.CollectionCount(0);
		}
		this.disposed = false;
		if (TimeWarning.OnBegin != null)
		{
			TimeWarning.OnBegin(name);
		}
	}

	private void Start(string name, long maxMilliseconds)
	{
		if (TimeWarning.Enabled)
		{
			this.warningName = name;
			this.warningMS = (double)maxMilliseconds;
			this.stopwatch.Reset();
			this.stopwatch.Start();
			this.gcCount = GC.CollectionCount(0);
		}
		this.disposed = false;
		if (TimeWarning.OnBegin != null)
		{
			TimeWarning.OnBegin(name);
		}
	}

	void System.IDisposable.Dispose()
	{
		if (this.disposed)
		{
			return;
		}
		this.disposed = true;
		if (TimeWarning.OnEnd != null)
		{
			TimeWarning.OnEnd();
		}
		if (TimeWarning.Enabled && this.stopwatch.Elapsed.TotalMilliseconds > this.warningMS)
		{
			bool flag = this.gcCount != GC.CollectionCount(0);
			object[] totalSeconds = new object[] { this.warningName, null, null, null };
			TimeSpan elapsed = this.stopwatch.Elapsed;
			totalSeconds[1] = elapsed.TotalSeconds;
			elapsed = this.stopwatch.Elapsed;
			totalSeconds[2] = elapsed.TotalMilliseconds;
			totalSeconds[3] = (flag ? " [GARBAGE COLLECT]" : "");
			UnityEngine.Debug.LogWarningFormat("TimeWarning: {0} took {1:0.00} seconds ({2:0} ms){3}", totalSeconds);
		}
		TimeWarning timeWarning = this;
		Pool.Free<TimeWarning>(ref timeWarning);
	}
}