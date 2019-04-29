using ConVar;
using Network;
using Oxide.Core;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class CollectibleEntity : BaseEntity, IPrefabPreProcess
{
	public Translate.Phrase itemName;

	public ItemAmount[] itemList;

	public GameObjectRef pickupEffect;

	public float xpScale = 1f;

	public CollectibleEntity()
	{
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("CollectibleEntity.OnRpcMessage", 0.1f))
		{
			if (rpc != -1516891826 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - Pickup "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("Pickup", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("Pickup", this, player, 3f))
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
							this.Pickup(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in Pickup");
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
	public void Pickup(BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		if (this.itemList == null)
		{
			return;
		}
		ItemAmount[] itemAmountArray = this.itemList;
		for (int i = 0; i < (int)itemAmountArray.Length; i++)
		{
			ItemAmount itemAmount = itemAmountArray[i];
			Item item = ItemManager.Create(itemAmount.itemDef, (int)itemAmount.amount, (ulong)0);
			if (Interface.CallHook("OnCollectiblePickup", item, msg.player, this) != null)
			{
				return;
			}
			msg.player.GiveItem(item, BaseEntity.GiveItemReason.ResourceHarvested);
		}
		this.itemList = null;
		if (this.pickupEffect.isValid)
		{
			Effect.server.Run(this.pickupEffect.resourcePath, base.transform.position, base.transform.up, null, false);
		}
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	public override void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(preProcess, rootObj, name, serverside, clientside, bundling);
		if (serverside)
		{
			preProcess.RemoveComponent(base.GetComponent<Collider>());
		}
	}
}