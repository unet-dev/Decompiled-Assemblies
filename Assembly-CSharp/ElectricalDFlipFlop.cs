using System;

public class ElectricalDFlipFlop : IOEntity
{
	[NonSerialized]
	private int setAmount;

	[NonSerialized]
	private int resetAmount;

	[NonSerialized]
	private int toggleAmount;

	public ElectricalDFlipFlop()
	{
	}

	public bool GetDesiredState()
	{
		if (this.setAmount > 0 && this.resetAmount == 0)
		{
			return true;
		}
		if (this.setAmount > 0 && this.resetAmount > 0)
		{
			return true;
		}
		if (this.setAmount == 0 && this.resetAmount > 0)
		{
			return false;
		}
		if (this.toggleAmount > 0)
		{
			return !base.IsOn();
		}
		if (this.setAmount != 0 || this.resetAmount != 0)
		{
			return false;
		}
		return base.IsOn();
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		return base.GetPassthroughAmount(outputSlot);
	}

	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		if (inputSlot == 1)
		{
			this.setAmount = inputAmount;
			this.UpdateState();
			return;
		}
		if (inputSlot == 2)
		{
			this.resetAmount = inputAmount;
			this.UpdateState();
			return;
		}
		if (inputSlot == 3)
		{
			this.toggleAmount = inputAmount;
			this.UpdateState();
			return;
		}
		if (inputSlot == 0)
		{
			base.UpdateFromInput(inputAmount, inputSlot);
			this.UpdateState();
		}
	}

	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		if (inputSlot == 0)
		{
			base.UpdateHasPower(inputAmount, inputSlot);
		}
	}

	public override void UpdateOutputs()
	{
		if (this.outputs.Length == 0)
		{
			this.ensureOutputsUpdated = false;
			return;
		}
		if (this.ensureOutputsUpdated)
		{
			if (this.outputs[0].connectedTo.Get(true) != null)
			{
				this.outputs[0].connectedTo.Get(true).UpdateFromInput((base.IsOn() ? this.currentEnergy : 0), this.outputs[0].connectedToSlot);
			}
			if (this.outputs[1].connectedTo.Get(true) != null)
			{
				this.outputs[1].connectedTo.Get(true).UpdateFromInput((base.IsOn() ? 0 : this.currentEnergy), this.outputs[1].connectedToSlot);
			}
		}
	}

	public void UpdateState()
	{
		if (this.IsPowered())
		{
			bool flag = base.IsOn();
			base.SetFlag(BaseEntity.Flags.On, this.GetDesiredState(), false, true);
			if (flag != base.IsOn())
			{
				base.MarkDirtyForceUpdateOutputs();
			}
		}
	}
}