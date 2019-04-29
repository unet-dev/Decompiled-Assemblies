using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ItemBasedFlowRestrictor : IOEntity
{
	public ItemDefinition passthroughItem;

	public ItemContainer.ContentsType allowedContents = ItemContainer.ContentsType.Generic;

	public int maxStackSize = 1;

	public int numSlots;

	public string lootPanelName = "generic";

	public const BaseEntity.Flags HasPassthrough = BaseEntity.Flags.Reserved1;

	public const BaseEntity.Flags Sparks = BaseEntity.Flags.Reserved2;

	public float passthroughItemConditionLossPerSec = 1f;

	private ItemContainer inventory;

	public ItemBasedFlowRestrictor()
	{
	}

	public void CreateInventory(bool giveUID)
	{
		this.inventory = new ItemContainer()
		{
			entityOwner = this,
			allowedContents = ((int)this.allowedContents == 0 ? ItemContainer.ContentsType.Generic : this.allowedContents),
			onlyAllowedItem = this.passthroughItem,
			maxStackSize = this.maxStackSize
		};
		this.inventory.ServerInitialize(null, this.numSlots);
		if (giveUID)
		{
			this.inventory.GiveUID();
		}
		ItemBasedFlowRestrictor itemBasedFlowRestrictor = this;
		this.inventory.onItemAddedRemoved = new Action<Item, bool>(itemBasedFlowRestrictor.OnItemAddedOrRemoved);
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.HasFlag(BaseEntity.Flags.Reserved1))
		{
			return 0;
		}
		return base.GetPassthroughAmount(outputSlot);
	}

	public virtual bool HasPassthroughItem()
	{
		if (this.inventory.itemList.Count <= 0)
		{
			return false;
		}
		Item slot = this.inventory.GetSlot(0);
		if (slot == null)
		{
			return false;
		}
		if (this.passthroughItemConditionLossPerSec > 0f && slot.hasCondition && slot.conditionNormalized <= 0f)
		{
			return false;
		}
		if (slot.info == this.passthroughItem)
		{
			return true;
		}
		return false;
	}

	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		base.SetFlag(BaseEntity.Flags.On, this.IsPowered(), false, true);
		base.SetFlag(BaseEntity.Flags.Reserved1, this.HasPassthroughItem(), false, true);
		base.SetFlag(BaseEntity.Flags.Reserved2, (!base.IsOn() ? false : !base.HasFlag(BaseEntity.Flags.Reserved1)), false, true);
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.storageBox != null)
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
		base.SetFlag(BaseEntity.Flags.Reserved1, this.HasPassthroughItem(), false, true);
		this.MarkDirty();
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("ItemBasedFlowRestrictor.OnRpcMessage", 0.1f))
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

	public void PlayerStoppedLooting(BasePlayer player)
	{
	}

	public override void PreServerLoad()
	{
		base.PreServerLoad();
		this.CreateInventory(false);
	}

	public override void ResetIOState()
	{
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		if (this.inventory != null)
		{
			Item slot = this.inventory.GetSlot(0);
			if (slot != null)
			{
				slot.Drop(this.debugOrigin.transform.position + (base.transform.forward * 0.5f), base.GetInheritedDropVelocity() + (base.transform.forward * 2f), new Quaternion());
			}
		}
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
		ItemBasedFlowRestrictor itemBasedFlowRestrictor = this;
		base.InvokeRandomized(new Action(itemBasedFlowRestrictor.TickPassthroughItem), 1f, 1f, 0.015f);
		base.ServerInit();
	}

	public virtual void TickPassthroughItem()
	{
		if (this.inventory.itemList.Count <= 0)
		{
			return;
		}
		if (!base.HasFlag(BaseEntity.Flags.On))
		{
			return;
		}
		Item slot = this.inventory.GetSlot(0);
		if (slot != null && slot.hasCondition)
		{
			slot.LoseCondition(1f);
		}
	}
}