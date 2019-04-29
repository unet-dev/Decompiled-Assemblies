using System;
using System.Collections.Generic;

public class SynchronizedClock
{
	public List<SynchronizedClock.TimedEvent> events = new List<SynchronizedClock.TimedEvent>();

	private static float DayLengthInMinutes
	{
		get
		{
			if (!TOD_Sky.Instance)
			{
				return 30f;
			}
			return TOD_Sky.Instance.Components.Time.DayLengthInMinutes;
		}
	}

	private static long Ticks
	{
		get
		{
			if (TOD_Sky.Instance)
			{
				return TOD_Sky.Instance.Cycle.Ticks;
			}
			return DateTime.Now.Ticks;
		}
	}

	public SynchronizedClock()
	{
	}

	public void Add(float delta, float variance, Action<uint> action)
	{
		SynchronizedClock.TimedEvent timedEvent = new SynchronizedClock.TimedEvent()
		{
			ticks = SynchronizedClock.Ticks,
			delta = delta,
			variance = variance,
			action = action
		};
		this.events.Add(timedEvent);
	}

	public void Tick()
	{
		long num = (long)10000000;
		double dayLengthInMinutes = 1440 / (double)SynchronizedClock.DayLengthInMinutes;
		double num1 = (double)num * dayLengthInMinutes;
		for (int i = 0; i < this.events.Count; i++)
		{
			SynchronizedClock.TimedEvent item = this.events[i];
			long num2 = item.ticks;
			long ticks = SynchronizedClock.Ticks;
			long num3 = (long)((double)item.delta * num1);
			long num4 = num2 / num3 * num3;
			uint num5 = (uint)(num4 % (ulong)-1);
			SeedRandom.Wanghash(ref num5);
			long num6 = (long)((double)SeedRandom.Range(ref num5, -item.variance, item.variance) * num1);
			long num7 = num4 + num3 + num6;
			if (num2 < num7 && ticks >= num7)
			{
				item.action(num5);
				item.ticks = ticks;
			}
			else if (ticks > num2 || ticks < num4)
			{
				item.ticks = ticks;
			}
			this.events[i] = item;
		}
	}

	public struct TimedEvent
	{
		public long ticks;

		public float delta;

		public float variance;

		public Action<uint> action;
	}
}