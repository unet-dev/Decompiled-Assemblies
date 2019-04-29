using ConVar;
using Network;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class Jackhammer : BaseMelee
{
	public Jackhammer()
	{
	}

	public bool HasAmmo()
	{
		return true;
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("Jackhammer.OnRpcMessage", 0.1f))
		{
			if (rpc != 1699910227 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - Server_SetEngineStatus "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("Server_SetEngineStatus", 0.1f))
				{
					try
					{
						using (TimeWarning timeWarning2 = TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage rPCMessage = new BaseEntity.RPCMessage()
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_SetEngineStatus(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in Server_SetEngineStatus");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	[RPC_Server]
	public void Server_SetEngineStatus(BaseEntity.RPCMessage msg)
	{
		this.SetEngineStatus(msg.read.Bit());
	}

	public void SetEngineStatus(bool on)
	{
		base.SetFlag(BaseEntity.Flags.Reserved8, on, false, true);
	}

	public override void SetHeld(bool bHeld)
	{
		if (!bHeld)
		{
			this.SetEngineStatus(false);
		}
		base.SetHeld(bHeld);
	}
}