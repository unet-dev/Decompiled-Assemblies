using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WaterVisibilityTrigger : EnvironmentVolumeTrigger
{
	private long enteredTick;

	private static long ticks;

	private static SortedList<long, WaterVisibilityTrigger> tracker;

	static WaterVisibilityTrigger()
	{
		WaterVisibilityTrigger.ticks = (long)1;
		WaterVisibilityTrigger.tracker = new SortedList<long, WaterVisibilityTrigger>();
	}

	public WaterVisibilityTrigger()
	{
	}

	private int GetVisibilityMask()
	{
		return 0;
	}

	protected void OnDestroy()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		WaterVisibilityTrigger.tracker.Remove(this.enteredTick);
	}

	protected void OnTriggerEnter(Collider other)
	{
		bool component = other.gameObject.GetComponent<PlayerWalkMovement>() != null;
		bool flag = other.gameObject.CompareTag("MainCamera");
		if (component | flag && !WaterVisibilityTrigger.tracker.ContainsValue(this))
		{
			long num = WaterVisibilityTrigger.ticks;
			WaterVisibilityTrigger.ticks = num + (long)1;
			this.enteredTick = num;
			WaterVisibilityTrigger.tracker.Add(this.enteredTick, this);
			this.ToggleVisibility();
		}
		if (!flag && !other.isTrigger)
		{
			this.ToggleCollision(other);
		}
	}

	protected void OnTriggerExit(Collider other)
	{
		bool component = other.gameObject.GetComponent<PlayerWalkMovement>() != null;
		bool flag = other.gameObject.CompareTag("MainCamera");
		if (component | flag && WaterVisibilityTrigger.tracker.ContainsValue(this))
		{
			WaterVisibilityTrigger.tracker.Remove(this.enteredTick);
			if (WaterVisibilityTrigger.tracker.Count <= 0)
			{
				this.ResetVisibility();
			}
			else
			{
				WaterVisibilityTrigger.tracker.Values[WaterVisibilityTrigger.tracker.Count - 1].ToggleVisibility();
			}
		}
		if (!flag && !other.isTrigger)
		{
			this.ResetCollision(other);
		}
	}

	public static void Reset()
	{
		WaterVisibilityTrigger.ticks = (long)1;
		WaterVisibilityTrigger.tracker.Clear();
	}

	private void ResetCollision(Collider other)
	{
		if (WaterSystem.Collision != null)
		{
			WaterSystem.Collision.SetIgnore(other, base.volume.trigger, false);
		}
	}

	private void ResetVisibility()
	{
	}

	private void ToggleCollision(Collider other)
	{
		if (WaterSystem.Collision != null)
		{
			WaterSystem.Collision.SetIgnore(other, base.volume.trigger, true);
		}
	}

	private void ToggleVisibility()
	{
	}
}