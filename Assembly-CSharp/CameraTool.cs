using ConVar;
using Network;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class CameraTool : HeldEntity
{
	public GameObjectRef screenshotEffect;

	public CameraTool()
	{
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("CameraTool.OnRpcMessage", 0.1f))
		{
			if (rpc != -1127088699 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - SVNoteScreenshot "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("SVNoteScreenshot", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test("SVNoteScreenshot", this, player))
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
							this.SVNoteScreenshot(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in SVNoteScreenshot");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	[FromOwner]
	[RPC_Server]
	private void SVNoteScreenshot(BaseEntity.RPCMessage msg)
	{
	}
}