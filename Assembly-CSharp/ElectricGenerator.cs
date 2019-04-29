using Facepunch;
using ProtoBuf;
using System;
using UnityEngine;

public class ElectricGenerator : IOEntity
{
	public float electricAmount = 8f;

	public ElectricGenerator()
	{
	}

	private void ForcePuzzleReset()
	{
		PuzzleReset component = base.GetComponent<PuzzleReset>();
		if (component != null)
		{
			component.DoReset();
			component.ResetTimer();
		}
	}

	public override int GetCurrentEnergy()
	{
		return (int)this.electricAmount;
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		return this.GetCurrentEnergy();
	}

	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
	}

	public override bool IsRootEntity()
	{
		return true;
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk && info.msg.puzzleReset != null)
		{
			PuzzleReset component = base.GetComponent<PuzzleReset>();
			if (component != null)
			{
				component.playersBlockReset = info.msg.puzzleReset.playerBlocksReset;
				if (component.playerDetectionOrigin != null)
				{
					component.playerDetectionOrigin.position = info.msg.puzzleReset.playerDetectionOrigin;
				}
				component.playerDetectionRadius = info.msg.puzzleReset.playerDetectionRadius;
				component.scaleWithServerPopulation = info.msg.puzzleReset.scaleWithServerPopulation;
				component.timeBetweenResets = info.msg.puzzleReset.timeBetweenResets;
				component.ResetTimer();
			}
		}
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.Invoke(new Action(this.ForcePuzzleReset), 1f);
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			PuzzleReset component = base.GetComponent<PuzzleReset>();
			if (component)
			{
				info.msg.puzzleReset = Pool.Get<ProtoBuf.PuzzleReset>();
				info.msg.puzzleReset.playerBlocksReset = component.playersBlockReset;
				if (component.playerDetectionOrigin != null)
				{
					info.msg.puzzleReset.playerDetectionOrigin = component.playerDetectionOrigin.position;
				}
				info.msg.puzzleReset.playerDetectionRadius = component.playerDetectionRadius;
				info.msg.puzzleReset.scaleWithServerPopulation = component.scaleWithServerPopulation;
				info.msg.puzzleReset.timeBetweenResets = component.timeBetweenResets;
			}
		}
	}

	public override void UpdateOutputs()
	{
		this.currentEnergy = this.GetCurrentEnergy();
		IOEntity.IOSlot[] oSlotArray = this.outputs;
		for (int i = 0; i < (int)oSlotArray.Length; i++)
		{
			IOEntity.IOSlot oSlot = oSlotArray[i];
			if (oSlot.connectedTo.Get(true) != null)
			{
				oSlot.connectedTo.Get(true).UpdateFromInput(this.currentEnergy, oSlot.connectedToSlot);
			}
		}
	}
}