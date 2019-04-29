using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class PowerCounter : IOEntity
{
	private int counterNumber;

	private int targetCounterNumber = 10;

	public CanvasGroup screenAlpha;

	public Text screenText;

	public const BaseEntity.Flags Flag_ShowPassthrough = BaseEntity.Flags.Reserved2;

	public GameObjectRef counterConfigPanel;

	public Color passthroughColor;

	public Color counterColor;

	public PowerCounter()
	{
	}

	public bool CanPlayerAdmin(BasePlayer player)
	{
		if (player == null)
		{
			return false;
		}
		return player.CanBuild();
	}

	public bool DisplayCounter()
	{
		return !this.DisplayPassthrough();
	}

	public bool DisplayPassthrough()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved2);
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!this.DisplayPassthrough() && this.counterNumber < this.targetCounterNumber)
		{
			return 0;
		}
		return base.GetPassthroughAmount(outputSlot);
	}

	public int GetTarget()
	{
		return this.targetCounterNumber;
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			if (base.isServer)
			{
				this.counterNumber = info.msg.ioEntity.genericInt1;
			}
			this.targetCounterNumber = info.msg.ioEntity.genericInt3;
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		BaseEntity.RPCMessage rPCMessage;
		using (TimeWarning timeWarning = TimeWarning.New("PowerCounter.OnRpcMessage", 0.1f))
		{
			if (rpc == -740740535 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - SERVER_SetTarget "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("SERVER_SetTarget", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("SERVER_SetTarget", this, player, 3f))
						{
							flag = true;
							return flag;
						}
					}
					try
					{
						using (timeWarning2 = TimeWarning.New("Call", 0.1f))
						{
							rPCMessage = new BaseEntity.RPCMessage()
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SERVER_SetTarget(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in SERVER_SetTarget");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc != -1072492137 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - ToggleDisplayMode "));
				}
				using (timeWarning1 = TimeWarning.New("ToggleDisplayMode", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("ToggleDisplayMode", this, player, 3f))
						{
							flag = true;
							return flag;
						}
					}
					try
					{
						using (timeWarning2 = TimeWarning.New("Call", 0.1f))
						{
							rPCMessage = new BaseEntity.RPCMessage()
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ToggleDisplayMode(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in ToggleDisplayMode");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.ioEntity == null)
		{
			info.msg.ioEntity = Facepunch.Pool.Get<ProtoBuf.IOEntity>();
		}
		info.msg.ioEntity.genericInt1 = this.counterNumber;
		info.msg.ioEntity.genericInt2 = this.GetPassthroughAmount(0);
		info.msg.ioEntity.genericInt3 = this.GetTarget();
	}

	public override void SendIONetworkUpdate()
	{
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void SERVER_SetTarget(BaseEntity.RPCMessage msg)
	{
		if (!this.CanPlayerAdmin(msg.player))
		{
			return;
		}
		this.targetCounterNumber = msg.read.Int32();
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public void SetCounterNumber(int newNumber)
	{
		this.counterNumber = newNumber;
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void ToggleDisplayMode(BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanBuild())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.Reserved2, msg.read.Bit(), false, false);
		this.MarkDirty();
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		if (this.DisplayCounter() && inputAmount > 0 && inputSlot != 0)
		{
			int num = this.counterNumber;
			if (inputSlot == 1)
			{
				this.counterNumber++;
			}
			else if (inputSlot == 2)
			{
				this.counterNumber--;
				if (this.counterNumber < 0)
				{
					this.counterNumber = 0;
				}
			}
			else if (inputSlot == 3)
			{
				this.counterNumber = 0;
			}
			this.counterNumber = Mathf.Clamp(this.counterNumber, 0, 100);
			if (num != this.counterNumber)
			{
				this.MarkDirty();
				base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			}
		}
		if (inputSlot == 0)
		{
			base.UpdateFromInput(inputAmount, inputSlot);
		}
	}

	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		if (inputSlot == 0)
		{
			base.UpdateHasPower(inputAmount, inputSlot);
		}
	}
}