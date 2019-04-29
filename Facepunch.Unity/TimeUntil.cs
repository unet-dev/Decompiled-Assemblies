using System;
using UnityEngine;

public struct TimeUntil
{
	private float time;

	public static implicit operator Single(TimeUntil ts)
	{
		return ts.time - Time.time;
	}

	public static implicit operator TimeUntil(float ts)
	{
		return new TimeUntil()
		{
			time = Time.time + ts
		};
	}
}