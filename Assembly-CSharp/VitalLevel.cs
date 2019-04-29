using System;
using UnityEngine;

[Serializable]
public struct VitalLevel
{
	public float Level;

	private float lastUsedTime;

	public float TimeSinceUsed
	{
		get
		{
			return Time.time - this.lastUsedTime;
		}
	}

	internal void Add(float f)
	{
		this.Level += f;
		if (this.Level > 1f)
		{
			this.Level = 1f;
		}
		if (this.Level < 0f)
		{
			this.Level = 0f;
		}
	}

	internal void Use(float f)
	{
		if (Mathf.Approximately(f, 0f))
		{
			return;
		}
		this.Level -= Mathf.Abs(f);
		if (this.Level < 0f)
		{
			this.Level = 0f;
		}
		this.lastUsedTime = Time.time;
	}
}