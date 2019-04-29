using Facepunch;
using ProtoBuf;
using System;
using UnityEngine;

public class ElectricBattery : IOEntity
{
	public int maxOutput;

	public float maxCapactiySeconds;

	public float capacitySeconds;

	public bool rechargable;

	public float chargeRatio = 0.25f;

	private const float tickRateSeconds = 1f;

	public ElectricBattery()
	{
	}

	public void AddCharge()
	{
		float single = Mathf.InverseLerp(0f, (float)this.maxOutput, (float)this.currentEnergy);
		this.capacitySeconds = this.capacitySeconds + 1f * single * this.chargeRatio;
		this.capacitySeconds = Mathf.Clamp(this.capacitySeconds, 0f, this.maxCapactiySeconds);
	}

	public void CheckDischarge()
	{
		if (this.capacitySeconds < 5f)
		{
			this.SetDischarging(false);
			return;
		}
		IOEntity oEntity = this.outputs[0].connectedTo.Get(true);
		if (!oEntity)
		{
			this.SetDischarging(false);
			return;
		}
		this.SetDischarging(oEntity.WantsPower());
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return Mathf.FloorToInt((float)this.maxOutput * (this.capacitySeconds >= 1f ? 1f : 0f));
	}

	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		if (inputSlot == 0)
		{
			if (!this.IsPowered())
			{
				if (this.rechargable)
				{
					base.CancelInvoke(new Action(this.AddCharge));
					return;
				}
			}
			else if (this.rechargable && !base.IsInvoking(new Action(this.AddCharge)))
			{
				base.InvokeRandomized(new Action(this.AddCharge), 1f, 1f, 0.1f);
			}
		}
	}

	public override bool IsRootEntity()
	{
		return true;
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.capacitySeconds = info.msg.ioEntity.genericFloat1;
		}
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.ioEntity == null)
		{
			info.msg.ioEntity = Pool.Get<ProtoBuf.IOEntity>();
		}
		info.msg.ioEntity.genericFloat1 = this.capacitySeconds;
	}

	public override void SendAdditionalData(BasePlayer player)
	{
		float single = 0f;
		base.ClientRPCPlayer<int, int, float, float>(null, player, "Client_ReceiveAdditionalData", this.currentEnergy, this.GetPassthroughAmount(0), this.capacitySeconds, single);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRandomized(new Action(this.CheckDischarge), UnityEngine.Random.Range(0f, 1f), 1f, 0.1f);
	}

	public void SetDischarging(bool wantsOn)
	{
		this.SetPassthroughOn(wantsOn);
	}

	public void SetPassthroughOn(bool wantsOn)
	{
		if (wantsOn == base.IsOn())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, wantsOn, false, true);
		if (!base.IsOn())
		{
			base.CancelInvoke(new Action(this.TickUsage));
		}
		else if (!base.IsInvoking(new Action(this.TickUsage)))
		{
			base.InvokeRandomized(new Action(this.TickUsage), 1f, 1f, 0.1f);
		}
		this.MarkDirty();
	}

	public void TickUsage()
	{
		bool flag = this.capacitySeconds > 0f;
		if (this.capacitySeconds >= 1f)
		{
			this.capacitySeconds -= 1f;
		}
		if (this.capacitySeconds <= 0f)
		{
			this.capacitySeconds = 0f;
		}
		if (flag != this.capacitySeconds > 0f)
		{
			this.MarkDirty();
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
	}

	public void Unbusy()
	{
		base.SetFlag(BaseEntity.Flags.Busy, false, false, true);
	}

	public override bool WantsPower()
	{
		return this.capacitySeconds < this.maxCapactiySeconds;
	}
}