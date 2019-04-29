using System;
using System.Diagnostics;
using UnityEngine;

public class Profile
{
	public Stopwatch watch = new Stopwatch();

	public string category;

	public string name;

	public float warnTime;

	public Profile(string cat, string nam, float WarnTime = 1f)
	{
		this.category = cat;
		this.name = nam;
		this.warnTime = WarnTime;
	}

	public void Start()
	{
		this.watch.Reset();
		this.watch.Start();
	}

	public void Stop()
	{
		this.watch.Stop();
		if ((float)this.watch.Elapsed.Seconds > this.warnTime)
		{
			object[] seconds = new object[] { this.category, ".", this.name, ": Took ", null, null };
			seconds[4] = this.watch.Elapsed.Seconds;
			seconds[5] = " seconds";
			UnityEngine.Debug.Log(string.Concat(seconds));
		}
	}
}