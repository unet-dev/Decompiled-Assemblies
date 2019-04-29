using ConVar;
using Network;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class BowWeapon : BaseProjectile
{
	public BowWeapon()
	{
	}

	[IsActiveItem]
	[RPC_Server]
	private void BowReload(BaseEntity.RPCMessage msg)
	{
		base.ReloadMagazine(-1);
	}

	public override bool ForceSendMagazine()
	{
		return true;
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("BowWeapon.OnRpcMessage", 0.1f))
		{
			if (rpc != -66919106 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - BowReload "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("BowReload", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test("BowReload", this, player))
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
							this.BowReload(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in BowReload");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}
}