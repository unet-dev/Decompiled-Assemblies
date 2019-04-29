using System;
using UnityEngine;

public class ElectricalCombiner : IOEntity
{
	public int input1Amount;

	public int input2Amount;

	private bool wasShorted;

	public ElectricalCombiner()
	{
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		int num = this.input1Amount + this.input2Amount;
		Mathf.Clamp(num - 1, 0, num);
		return num;
	}

	public override bool IsRootEntity()
	{
		return true;
	}

	public override void UpdateFromInput(int inputAmount, int slot)
	{
		bool flag = false;
		if (!base.IsConnectedTo(this, slot, IOEntity.backtracking * 2, true) || inputAmount <= 0)
		{
			flag = false;
		}
		else
		{
			inputAmount = 0;
			flag = true;
		}
		if (this.wasShorted != flag)
		{
			base.SetFlag(BaseEntity.Flags.Reserved7, flag, false, true);
			this.wasShorted = flag;
		}
		if (slot == 0)
		{
			this.input1Amount = inputAmount;
		}
		else if (slot == 1)
		{
			this.input2Amount = inputAmount;
		}
		int num = this.input1Amount + this.input2Amount;
		bool flag1 = num > 0;
		base.SetFlag(BaseEntity.Flags.Reserved1, this.input1Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved2, this.input2Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved3, flag1, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved4, (this.input1Amount > 0 ? true : this.input2Amount > 0), false, false);
		base.SetFlag(BaseEntity.Flags.On, num > 0, false, true);
		base.UpdateFromInput(num, slot);
	}

	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.SetFlag(BaseEntity.Flags.Reserved8, (this.input1Amount > 0 ? true : this.input2Amount > 0), false, false);
	}
}