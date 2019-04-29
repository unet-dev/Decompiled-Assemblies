using System;
using UnityEngine;

[Serializable]
public struct StateTimer
{
	public float ReleaseTime;

	public Action OnFinished;

	public bool IsActive
	{
		get
		{
			bool releaseTime = this.ReleaseTime > Time.time;
			if (!releaseTime && this.OnFinished != null)
			{
				this.OnFinished();
				this.OnFinished = null;
			}
			return releaseTime;
		}
	}

	public void Activate(float seconds, Action onFinished = null)
	{
		this.ReleaseTime = Time.time + seconds;
		this.OnFinished = onFinished;
	}
}