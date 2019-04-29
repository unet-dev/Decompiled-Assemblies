using ConVar;
using Network;
using Oxide.Core;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class ResourceContainer : EntityComponent<BaseEntity>
{
	public bool lootable = true;

	[NonSerialized]
	public ItemContainer container;

	[NonSerialized]
	public float lastAccessTime;

	public int accessedSecondsAgo
	{
		get
		{
			return (int)(UnityEngine.Time.realtimeSinceStartup - this.lastAccessTime);
		}
	}

	public ResourceContainer()
	{
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("ResourceContainer.OnRpcMessage", 0.1f))
		{
			if (rpc != 548378753 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - StartLootingContainer "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("StartLootingContainer", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("StartLootingContainer", this.GetBaseEntity(), player, 3f))
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
							this.StartLootingContainer(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in StartLootingContainer");
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
	private void StartLootingContainer(BaseEntity.RPCMessage msg)
	{
		BasePlayer basePlayer = msg.player;
		if (!basePlayer || !basePlayer.CanInteract())
		{
			return;
		}
		if (!this.lootable)
		{
			return;
		}
		if (Interface.CallHook("CanLootEntity", basePlayer, this) != null)
		{
			return;
		}
		if (basePlayer.inventory.loot.StartLootingEntity(base.baseEntity, true))
		{
			this.lastAccessTime = UnityEngine.Time.realtimeSinceStartup;
			basePlayer.inventory.loot.AddContainer(this.container);
		}
	}
}