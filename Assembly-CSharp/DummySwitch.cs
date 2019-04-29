using System;
using UnityEngine;

public class DummySwitch : IOEntity
{
	public string listenString = "";

	public float duration = -1f;

	public DummySwitch()
	{
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return this.GetCurrentEnergy();
	}

	public override void OnEntityMessage(BaseEntity from, string msg)
	{
		if (msg == this.listenString)
		{
			if (base.IsOn())
			{
				this.SetOn(false);
			}
			this.SetOn(true);
		}
	}

	public override void ResetIOState()
	{
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		Debug.Log("Resetting");
	}

	public void SetOff()
	{
		this.SetOn(false);
	}

	public void SetOn(bool wantsOn)
	{
		base.SetFlag(BaseEntity.Flags.On, wantsOn, false, true);
		this.MarkDirty();
		if (base.IsOn() && this.duration != -1f)
		{
			base.Invoke(new Action(this.SetOff), this.duration);
		}
	}

	public override bool WantsPower()
	{
		return base.IsOn();
	}
}