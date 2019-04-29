using System;
using UnityEngine;

public class RANDSwitch : ElectricalBlocker
{
	private bool rand;

	public RANDSwitch()
	{
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		return this.GetCurrentEnergy() * (base.IsOn() ? 1 : 0);
	}

	public bool RandomRoll()
	{
		return UnityEngine.Random.Range(0, 2) == 1;
	}

	public override void UpdateBlocked()
	{
		bool flag = base.IsOn();
		base.SetFlag(BaseEntity.Flags.On, this.rand, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved8, this.rand, false, false);
		this.UpdateHasPower(this.input1Amount + this.input2Amount, 1);
		if (flag != base.IsOn())
		{
			this.MarkDirty();
		}
	}

	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		if (inputSlot == 1 && inputAmount > 0)
		{
			this.input1Amount = inputAmount;
			this.rand = this.RandomRoll();
			this.UpdateBlocked();
		}
		if (inputSlot != 2 || inputAmount <= 0)
		{
			base.UpdateFromInput(inputAmount, inputSlot);
			return;
		}
		this.rand = false;
		this.UpdateBlocked();
	}
}