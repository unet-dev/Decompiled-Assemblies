using System;
using System.Diagnostics;
using UnityEngine;

public struct Timing
{
	private Stopwatch sw;

	private string name;

	public Timing(string name)
	{
		this.sw = Stopwatch.StartNew();
		this.name = name;
	}

	public void End()
	{
		if (this.sw.Elapsed.TotalSeconds > 0.300000011920929)
		{
			double totalSeconds = this.sw.Elapsed.TotalSeconds;
			UnityEngine.Debug.Log(string.Concat("[", totalSeconds.ToString("0.0"), "s] ", this.name));
		}
	}

	public static Timing Start(string name)
	{
		return new Timing(name);
	}
}