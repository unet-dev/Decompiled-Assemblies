using System;
using UnityEngine;

public class ANDSwitch : IOEntity
{
	private int input1Amount;

	private int input2Amount;

	public ANDSwitch()
	{
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (this.input1Amount <= 0 || this.input2Amount <= 0)
		{
			return 0;
		}
		return Mathf.Max(this.input1Amount, this.input2Amount);
	}

	public override void UpdateFromInput(int inputAmount, int slot)
	{
		if (slot == 0)
		{
			this.input1Amount = inputAmount;
		}
		else if (slot == 1)
		{
			this.input2Amount = inputAmount;
		}
		int num = (this.input1Amount <= 0 || this.input2Amount <= 0 ? 0 : this.input1Amount + this.input2Amount);
		bool flag = num > 0;
		base.SetFlag(BaseEntity.Flags.Reserved1, this.input1Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved2, this.input2Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved3, flag, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved4, (this.input1Amount <= 0 ? false : this.input2Amount > 0), false, false);
		base.SetFlag(BaseEntity.Flags.On, num > 0, false, true);
		base.UpdateFromInput(inputAmount, slot);
	}

	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.SetFlag(BaseEntity.Flags.Reserved8, (this.input1Amount > 0 ? true : this.input2Amount > 0), false, false);
	}
}