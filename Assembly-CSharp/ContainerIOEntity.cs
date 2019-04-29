using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class ContainerIOEntity : IOEntity
{
	public ItemDefinition onlyAllowedItem;

	public ItemContainer.ContentsType allowedContents = ItemContainer.ContentsType.Generic;

	public int maxStackSize = 1;

	public int numSlots;

	public string lootPanelName = "generic";

	public bool needsBuildingPrivilegeToUse;

	public bool isLootable = true;

	protected ItemContainer inventory;

	public ContainerIOEntity()
	{
	}

	public void CreateInventory(bool giveUID)
	{
		this.inventory = new ItemContainer()
		{
			entityOwner = this,
			allowedContents = ((int)this.allowedContents == 0 ? ItemContainer.ContentsType.Generic : this.allowedContents),
			onlyAllowedItem = this.onlyAllowedItem,
			maxStackSize = this.maxStackSize
		};
		this.inventory.ServerInitialize(null, this.numSlots);
		if (giveUID)
		{
			this.inventory.GiveUID();
		}
		ContainerIOEntity containerIOEntity = this;
		this.inventory.onItemAddedRemoved = new Action<Item, bool>(containerIOEntity.OnItemAddedOrRemoved);
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk && info.msg.storageBox != null)
		{
			if (this.inventory != null)
			{
				this.inventory.Load(info.msg.storageBox.contents);
				this.inventory.capacity = this.numSlots;
				return;
			}
			Debug.LogWarning(string.Concat("Storage container without inventory: ", this.ToString()));
		}
	}

	public virtual void OnInventoryFirstCreated(ItemContainer container)
	{
	}

	public virtual void OnItemAddedOrRemoved(Item item, bool added)
	{
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("ContainerIOEntity.OnRpcMessage", 0.1f))
		{
			if (rpc != 331989034 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_OpenLoot "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("RPC_OpenLoot", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_OpenLoot", this, player, 3f))
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
							this.RPC_OpenLoot(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in RPC_OpenLoot");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public virtual void PlayerStoppedLooting(BasePlayer player)
	{
	}

	public override void PreServerLoad()
	{
		base.PreServerLoad();
		this.CreateInventory(false);
	}

	[IsVisible(3f)]
	[RPC_Server]
	private void RPC_OpenLoot(BaseEntity.RPCMessage rpc)
	{
		if (this.inventory == null)
		{
			return;
		}
		BasePlayer basePlayer = rpc.player;
		if (!basePlayer || !basePlayer.CanInteract())
		{
			return;
		}
		if (this.needsBuildingPrivilegeToUse && !basePlayer.CanBuild())
		{
			return;
		}
		if (basePlayer.inventory.loot.StartLootingEntity(this, true))
		{
			base.SetFlag(BaseEntity.Flags.Open, true, false, true);
			basePlayer.inventory.loot.AddContainer(this.inventory);
			basePlayer.inventory.loot.SendImmediate();
			basePlayer.ClientRPCPlayer<string>(null, basePlayer, "RPC_OpenLootPanel", this.lootPanelName);
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			if (this.inventory != null)
			{
				info.msg.storageBox = Facepunch.Pool.Get<StorageBox>();
				info.msg.storageBox.contents = this.inventory.Save();
				return;
			}
			Debug.LogWarning(string.Concat("Storage container without inventory: ", this.ToString()));
		}
	}

	public override void ServerInit()
	{
		if (this.inventory == null)
		{
			this.CreateInventory(true);
			this.OnInventoryFirstCreated(this.inventory);
		}
		base.ServerInit();
	}
}