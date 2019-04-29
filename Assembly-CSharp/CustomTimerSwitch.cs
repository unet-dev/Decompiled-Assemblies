using ConVar;
using Network;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class CustomTimerSwitch : TimerSwitch
{
	public GameObjectRef timerPanelPrefab;

	public CustomTimerSwitch()
	{
	}

	public bool CanPlayerAdmin(BasePlayer player)
	{
		if (!(player != null) || !player.CanBuild())
		{
			return false;
		}
		return !base.IsOn();
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("CustomTimerSwitch.OnRpcMessage", 0.1f))
		{
			if (rpc != 1019813162 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - SERVER_SetTime "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("SERVER_SetTime", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("SERVER_SetTime", this, player, 3f))
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
							this.SERVER_SetTime(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in SERVER_SetTime");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void SERVER_SetTime(BaseEntity.RPCMessage msg)
	{
		if (!this.CanPlayerAdmin(msg.player))
		{
			return;
		}
		this.timerLength = (float)msg.read.Int32();
		Debug.Log(string.Concat("Server updating time to : ", this.timerLength));
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		base.UpdateFromInput(inputAmount, inputSlot);
		if (inputAmount > 0 && inputSlot == 1)
		{
			base.SwitchPressed();
		}
	}
}