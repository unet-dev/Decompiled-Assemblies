using System;
using System.Collections.Generic;
using UnityEngine;

public class LocalClock
{
	public List<LocalClock.TimedEvent> events = new List<LocalClock.TimedEvent>();

	public LocalClock()
	{
	}

	public void Add(float delta, float variance, Action action)
	{
		LocalClock.TimedEvent timedEvent = new LocalClock.TimedEvent()
		{
			time = Time.time + delta + UnityEngine.Random.Range(-variance, variance),
			delta = delta,
			variance = variance,
			action = action
		};
		this.events.Add(timedEvent);
	}

	public void Tick()
	{
		for (int i = 0; i < this.events.Count; i++)
		{
			LocalClock.TimedEvent item = this.events[i];
			if (Time.time > item.time)
			{
				float single = item.delta;
				float single1 = item.variance;
				item.action();
				item.time = Time.time + single + UnityEngine.Random.Range(-single1, single1);
				this.events[i] = item;
			}
		}
	}

	public struct TimedEvent
	{
		public float time;

		public float delta;

		public float variance;

		public Action action;
	}
}