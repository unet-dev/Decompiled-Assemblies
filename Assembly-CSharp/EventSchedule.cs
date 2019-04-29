using ConVar;
using Rust;
using System;
using UnityEngine;

public class EventSchedule : BaseMonoBehaviour
{
	[Tooltip("The minimum amount of hours between events")]
	public float minimumHoursBetween = 12f;

	[Tooltip("The maximum amount of hours between events")]
	public float maxmumHoursBetween = 24f;

	private float hoursRemaining;

	private long lastRun;

	public EventSchedule()
	{
	}

	private void CountHours()
	{
		DateTime dateTime;
		if (!TOD_Sky.Instance)
		{
			return;
		}
		if (this.lastRun != 0)
		{
			dateTime = TOD_Sky.Instance.Cycle.DateTime;
			TimeSpan timeSpan = dateTime.Subtract(DateTime.FromBinary(this.lastRun));
			this.hoursRemaining = this.hoursRemaining - (float)timeSpan.TotalSeconds / 60f / 60f;
		}
		dateTime = TOD_Sky.Instance.Cycle.DateTime;
		this.lastRun = dateTime.ToBinary();
	}

	private void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		base.CancelInvoke(new Action(this.RunSchedule));
	}

	private void OnEnable()
	{
		this.hoursRemaining = UnityEngine.Random.Range(this.minimumHoursBetween, this.maxmumHoursBetween);
		base.InvokeRepeating(new Action(this.RunSchedule), 1f, 1f);
	}

	private void RunSchedule()
	{
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (!ConVar.Server.events)
		{
			return;
		}
		this.CountHours();
		if (this.hoursRemaining > 0f)
		{
			return;
		}
		this.Trigger();
	}

	private void Trigger()
	{
		this.hoursRemaining = UnityEngine.Random.Range(this.minimumHoursBetween, this.maxmumHoursBetween);
		TriggeredEvent[] components = base.GetComponents<TriggeredEvent>();
		if (components.Length == 0)
		{
			return;
		}
		TriggeredEvent triggeredEvent = components[UnityEngine.Random.Range(0, (int)components.Length)];
		if (triggeredEvent == null)
		{
			return;
		}
		triggeredEvent.SendMessage("RunEvent", SendMessageOptions.DontRequireReceiver);
	}
}