using System;
using UnityEngine;

public struct TimeSince
{
	private float time;

	public static implicit operator Single(TimeSince ts)
	{
		return Time.time - ts.time;
	}

	public static implicit operator TimeSince(float ts)
	{
		return new TimeSince()
		{
			time = Time.time - ts
		};
	}
}