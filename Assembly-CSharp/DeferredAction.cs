using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DeferredAction
{
	private UnityEngine.Object sender;

	private Action action;

	private ActionPriority priority = ActionPriority.Medium;

	public bool Idle
	{
		get;
		private set;
	}

	public int Index
	{
		get
		{
			return (int)this.priority;
		}
	}

	public DeferredAction(UnityEngine.Object sender, Action action, ActionPriority priority = 2)
	{
		this.sender = sender;
		this.action = action;
		this.priority = priority;
		this.Idle = true;
	}

	public void Action()
	{
		if (this.Idle)
		{
			throw new Exception("Double invocation of a deferred action.");
		}
		this.Idle = true;
		if (this.sender)
		{
			this.action();
		}
	}

	public void Invoke()
	{
		if (!this.Idle)
		{
			throw new Exception("Double invocation of a deferred action.");
		}
		LoadBalancer.Enqueue(this);
		this.Idle = false;
	}

	public static void Invoke(UnityEngine.Object sender, Action action, ActionPriority priority = 2)
	{
		(new DeferredAction(sender, action, priority)).Invoke();
	}

	public static implicit operator Boolean(DeferredAction obj)
	{
		return obj != null;
	}
}