using System;

public class ElectricalBlocker : IOEntity
{
	protected int input1Amount;

	protected int input2Amount;

	public ElectricalBlocker()
	{
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		return base.GetPassthroughAmount(outputSlot) * (base.IsOn() ? 0 : 1);
	}

	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
	}

	public virtual void UpdateBlocked()
	{
		bool flag = base.IsOn();
		base.SetFlag(BaseEntity.Flags.On, this.input1Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved8, base.IsOn(), false, false);
		this.UpdateHasPower(this.input1Amount + this.input2Amount, 1);
		if (flag != base.IsOn())
		{
			this.MarkDirty();
		}
	}

	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		if (inputSlot == 1)
		{
			this.input1Amount = inputAmount;
			this.UpdateBlocked();
			return;
		}
		if (inputSlot == 0)
		{
			this.input2Amount = inputAmount;
			base.UpdateFromInput(inputAmount, inputSlot);
		}
	}

	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.UpdateHasPower(inputAmount, inputSlot);
		base.SetFlag(BaseEntity.Flags.Reserved8, (this.input1Amount > 0 ? true : this.input2Amount > 0), false, false);
	}

	public override bool WantsPower()
	{
		return !base.IsOn();
	}
}