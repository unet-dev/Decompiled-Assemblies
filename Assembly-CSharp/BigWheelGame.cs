using Oxide.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BigWheelGame : SpinnerWheel
{
	public HitNumber[] hitNumbers;

	public GameObject indicator;

	public GameObjectRef winEffect;

	[ServerVar]
	public static float spinFrequencySeconds;

	protected int spinNumber;

	protected int lastPaidSpinNumber = -1;

	protected List<BigWheelBettingTerminal> terminals = new List<BigWheelBettingTerminal>();

	static BigWheelGame()
	{
		BigWheelGame.spinFrequencySeconds = 45f;
	}

	public BigWheelGame()
	{
	}

	public override bool AllowPlayerSpins()
	{
		return false;
	}

	public override bool CanUpdateSign(BasePlayer player)
	{
		return false;
	}

	public void DoSpin()
	{
		if (this.velocity > 0f)
		{
			return;
		}
		this.velocity += UnityEngine.Random.Range(7f, 10f);
		this.spinNumber++;
		this.SetTerminalsLocked(true);
	}

	public HitNumber GetCurrentHitType()
	{
		HitNumber hitNumber = null;
		float single = Single.PositiveInfinity;
		HitNumber[] hitNumberArray = this.hitNumbers;
		for (int i = 0; i < (int)hitNumberArray.Length; i++)
		{
			HitNumber hitNumber1 = hitNumberArray[i];
			float single1 = Vector3.Distance(this.indicator.transform.position, hitNumber1.transform.position);
			if (single1 < single)
			{
				hitNumber = hitNumber1;
				single = single1;
			}
		}
		return hitNumber;
	}

	public override float GetMaxSpinSpeed()
	{
		return 180f;
	}

	protected void InitBettingTerminals()
	{
		this.terminals.Clear();
		Vis.Entities<BigWheelBettingTerminal>(base.transform.position, 30f, this.terminals, 256, QueryTriggerInteraction.Collide);
		foreach (BigWheelBettingTerminal terminal in this.terminals)
		{
		}
	}

	[ContextMenu("LoadHitNumbers")]
	private void LoadHitNumbers()
	{
		this.hitNumbers = base.GetComponentsInChildren<HitNumber>();
	}

	public void Payout()
	{
		HitNumber currentHitType = this.GetCurrentHitType();
		foreach (BigWheelBettingTerminal terminal in this.terminals)
		{
			if (terminal.isClient)
			{
				continue;
			}
			bool flag = false;
			bool flag1 = false;
			Item slot = terminal.inventory.GetSlot((int)currentHitType.hitType);
			if (slot != null)
			{
				int multiplier = currentHitType.ColorToMultiplier(currentHitType.hitType);
				Item item = slot;
				item.amount = item.amount + slot.amount * multiplier;
				slot.RemoveFromContainer();
				slot.MoveToContainer(terminal.inventory, 5, true);
				flag = true;
			}
			for (int i = 0; i < 5; i++)
			{
				Item slot1 = terminal.inventory.GetSlot(i);
				if (slot1 != null)
				{
					Interface.CallHook("OnBigWheelLoss", this, slot1);
					slot1.Remove(0f);
					flag1 = true;
				}
			}
			if (!(flag | flag1))
			{
				continue;
			}
			terminal.ClientRPC<bool>(null, "WinOrLoseSound", flag);
		}
		ItemManager.DoRemoves();
		this.SetTerminalsLocked(false);
	}

	public void QueueSpin()
	{
		foreach (BigWheelBettingTerminal terminal in this.terminals)
		{
			terminal.ClientRPC<float>(null, "SetTimeUntilNextSpin", this.SpinSpacing());
		}
		base.Invoke(new Action(this.DoSpin), this.SpinSpacing());
	}

	public override void ServerInit()
	{
		base.ServerInit();
		base.Invoke(new Action(this.InitBettingTerminals), 3f);
		base.Invoke(new Action(this.DoSpin), 10f);
	}

	public void SetTerminalsLocked(bool isLocked)
	{
		foreach (BigWheelBettingTerminal terminal in this.terminals)
		{
			terminal.inventory.SetLocked(isLocked);
		}
	}

	public float SpinSpacing()
	{
		return BigWheelGame.spinFrequencySeconds;
	}

	public override void Update_Server()
	{
		float single = this.velocity;
		base.Update_Server();
		float single1 = this.velocity;
		if (single > 0f && single1 == 0f && this.spinNumber > this.lastPaidSpinNumber)
		{
			this.Payout();
			this.lastPaidSpinNumber = this.spinNumber;
			this.QueueSpin();
		}
	}
}