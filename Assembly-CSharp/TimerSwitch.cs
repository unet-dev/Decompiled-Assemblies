using ConVar;
using Network;
using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class TimerSwitch : IOEntity
{
	public float timerLength = 10f;

	public Transform timerDrum;

	private float timePassed = -1f;

	public TimerSwitch()
	{
	}

	public void AdvanceTime()
	{
		if (this.timePassed < 0f)
		{
			this.timePassed = 0f;
		}
		this.timePassed += 0.1f;
		if (this.timePassed >= this.timerLength)
		{
			this.EndTimer();
			return;
		}
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public void EndTimer()
	{
		base.CancelInvoke(new Action(this.AdvanceTime));
		this.timePassed = -1f;
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		base.SendNetworkUpdateImmediate(false);
		this.MarkDirty();
	}

	public float GetPassedTime()
	{
		return this.timePassed;
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (outputSlot != 0)
		{
			return base.GetPassthroughAmount(0);
		}
		if ((!this.IsPowered() ? true : !base.IsOn()))
		{
			return 0;
		}
		return base.GetPassthroughAmount(0);
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.timerLength = info.msg.ioEntity.genericFloat2;
			this.timePassed = info.msg.ioEntity.genericFloat1;
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("TimerSwitch.OnRpcMessage", 0.1f))
		{
			if (rpc != -127127424 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - SVSwitch "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("SVSwitch", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("SVSwitch", this, player, 3f))
						{
							flag = true;
							return flag;
						}
					}
					try
					{
						using (timeWarning2 = TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage rPCMessage = new BaseEntity.RPCMessage()
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SVSwitch(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in SVSwitch");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (this.timePassed != -1f)
		{
			this.SwitchPressed();
		}
		else if (base.IsOn())
		{
			base.SetFlag(BaseEntity.Flags.On, false, false, true);
			return;
		}
	}

	public override void ResetIOState()
	{
		base.ResetIOState();
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		if (base.IsInvoking(new Action(this.AdvanceTime)))
		{
			this.EndTimer();
		}
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity.genericFloat1 = this.GetPassedTime();
		info.msg.ioEntity.genericFloat2 = this.timerLength;
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void SVSwitch(BaseEntity.RPCMessage msg)
	{
		this.SwitchPressed();
	}

	public void SwitchPressed()
	{
		if (base.IsOn())
		{
			return;
		}
		if (!this.IsPowered())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		this.MarkDirty();
		base.InvokeRepeating(new Action(this.AdvanceTime), 0f, 0.1f);
		base.SendNetworkUpdateImmediate(false);
	}

	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		if (inputSlot == 0)
		{
			base.UpdateFromInput(inputAmount, inputSlot);
			if (!this.IsPowered() && base.IsInvoking(new Action(this.AdvanceTime)))
			{
				this.EndTimer();
				return;
			}
			if (this.timePassed != -1f)
			{
				base.SetFlag(BaseEntity.Flags.On, false, false, false);
				this.SwitchPressed();
				return;
			}
		}
		else if (inputSlot == 1 && inputAmount > 0)
		{
			this.SwitchPressed();
		}
	}

	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		if (inputSlot == 0)
		{
			base.SetFlag(BaseEntity.Flags.Reserved8, inputAmount > 0, false, false);
		}
	}
}