using ConVar;
using Network;
using Oxide.Core;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class BaseLock : BaseEntity
{
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition itemType;

	public BaseLock()
	{
	}

	public override float BoundsPadding()
	{
		return 2f;
	}

	public virtual bool GetPlayerLockPermission(BasePlayer player)
	{
		return this.OnTryToOpen(player);
	}

	public virtual bool HasLockPermission(BasePlayer player)
	{
		return true;
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("BaseLock.OnRpcMessage", 0.1f))
		{
			if (rpc != -722410641 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_TakeLock "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("RPC_TakeLock", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_TakeLock", this, player, 3f))
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
							this.RPC_TakeLock(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in RPC_TakeLock");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public virtual bool OnTryToClose(BasePlayer player)
	{
		return true;
	}

	public virtual bool OnTryToOpen(BasePlayer player)
	{
		return !base.IsLocked();
	}

	[MaxDistance(3f)]
	[RPC_Server]
	public void RPC_TakeLock(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (base.IsLocked())
		{
			return;
		}
		if (Interface.CallHook("CanPickupLock", rpc.player, this) != null)
		{
			return;
		}
		Item item = ItemManager.Create(this.itemType, 1, this.skinID);
		if (item != null)
		{
			rpc.player.GiveItem(item, BaseEntity.GiveItemReason.Generic);
		}
		base.Kill(BaseNetworkable.DestroyMode.None);
	}
}