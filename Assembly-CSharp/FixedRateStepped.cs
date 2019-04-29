using System;
using UnityEngine;

public class FixedRateStepped
{
	public float rate = 0.1f;

	public int maxSteps = 3;

	internal float nextCall;

	public FixedRateStepped()
	{
	}

	public bool ShouldStep()
	{
		if (this.nextCall > Time.time)
		{
			return false;
		}
		if (this.nextCall == 0f)
		{
			this.nextCall = Time.time;
		}
		if (this.nextCall + this.rate * (float)this.maxSteps < Time.time)
		{
			this.nextCall = Time.time - this.rate * (float)this.maxSteps;
		}
		this.nextCall += this.rate;
		return true;
	}
}