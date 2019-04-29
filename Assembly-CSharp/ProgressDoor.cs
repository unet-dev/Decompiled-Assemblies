using System;
using UnityEngine;

public class ProgressDoor : IOEntity
{
	public float storedEnergy;

	public float energyForOpen = 1f;

	public float secondsToClose = 1f;

	public float openProgress;

	public ProgressDoor()
	{
	}

	public virtual void AddEnergy(float amount)
	{
		if (amount <= 0f)
		{
			return;
		}
		this.storedEnergy += amount;
		this.storedEnergy = Mathf.Clamp(this.storedEnergy, 0f, this.energyForOpen);
	}

	public override float IOInput(IOEntity from, IOEntity.IOType inputType, float inputAmount, int slot = 0)
	{
		if (inputAmount <= 0f)
		{
			this.NoEnergy();
			return inputAmount;
		}
		this.AddEnergy(inputAmount);
		if (this.storedEnergy == this.energyForOpen)
		{
			return inputAmount;
		}
		return 0f;
	}

	public virtual void NoEnergy()
	{
	}

	public override void ResetIOState()
	{
		this.storedEnergy = 0f;
		this.UpdateProgress();
	}

	public virtual void UpdateProgress()
	{
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}
}