using ConVar;
using Network;
using Oxide.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class DoorCloser : BaseEntity
{
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition itemType;

	public float delay = 3f;

	public DoorCloser()
	{
	}

	public override float BoundsPadding()
	{
		return 1f;
	}

	public Door GetDoor()
	{
		return base.GetParentEntity() as Door;
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("DoorCloser.OnRpcMessage", 0.1f))
		{
			if (rpc != 342802563 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_Take "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("RPC_Take", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_Take", this, player, 3f))
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
							this.RPC_Take(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in RPC_Take");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	[MaxDistance(3f)]
	[RPC_Server]
	public void RPC_Take(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!rpc.player.CanBuild())
		{
			return;
		}
		if (Interface.CallHook("ICanPickupEntity", rpc.player, this) != null)
		{
			return;
		}
		Door door = this.GetDoor();
		if (door == null)
		{
			return;
		}
		if (!door.GetPlayerLockPermission(rpc.player))
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

	public void SendClose()
	{
		BaseEntity parentEntity = base.GetParentEntity();
		if (this.children != null)
		{
			foreach (BaseEntity child in this.children)
			{
				if (child == null)
				{
					continue;
				}
				base.Invoke(new Action(this.SendClose), this.delay);
				return;
			}
		}
		if (parentEntity)
		{
			parentEntity.SendMessage("CloseRequest");
		}
	}

	public void Think()
	{
		base.Invoke(new Action(this.SendClose), this.delay);
	}
}