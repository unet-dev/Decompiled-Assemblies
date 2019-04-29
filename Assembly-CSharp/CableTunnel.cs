using System;

public class CableTunnel : IOEntity
{
	private const int numChannels = 4;

	private int[] inputAmounts = new int[4];

	public CableTunnel()
	{
	}

	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		int num = this.inputAmounts[inputSlot];
		this.inputAmounts[inputSlot] = inputAmount;
		if (inputAmount != num)
		{
			this.ensureOutputsUpdated = true;
		}
		base.IOStateChanged(inputAmount, inputSlot);
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
			for (int i = 0; i < 4; i++)
			{
				IOEntity.IOSlot oSlot = this.outputs[i];
				if (oSlot.connectedTo.Get(true) != null)
				{
					oSlot.connectedTo.Get(true).UpdateFromInput(this.inputAmounts[i], oSlot.connectedToSlot);
				}
			}
		}
	}

	public override bool WantsPower()
	{
		return true;
	}
}