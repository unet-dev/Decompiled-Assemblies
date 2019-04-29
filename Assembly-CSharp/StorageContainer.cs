using ConVar;
using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class StorageContainer : DecayEntity
{
	public int inventorySlots = 6;

	public float dropChance = 0.75f;

	public bool isLootable = true;

	public bool isLockable = true;

	public string panelName = "generic";

	public ItemContainer.ContentsType allowedContents;

	public ItemDefinition allowedItem;

	public int maxStackSize;

	public bool needsBuildingPrivilegeToUse;

	public SoundDefinition openSound;

	public SoundDefinition closeSound;

	[Header("Item Dropping")]
	public Vector3 dropPosition;

	public Vector3 dropVelocity = Vector3.forward;

	public ItemCategory onlyAcceptCategory = ItemCategory.All;

	public bool onlyOneUser;

	[NonSerialized]
	public ItemContainer inventory;

	public StorageContainer()
	{
	}

	public virtual bool CanMoveFrom(BasePlayer player, Item item)
	{
		return !this.inventory.IsLocked();
	}

	public virtual bool CanOpenLootPanel(BasePlayer player, string panelName = "")
	{
		if (this.needsBuildingPrivilegeToUse && !player.CanBuild())
		{
			return false;
		}
		BaseLock slot = base.GetSlot(BaseEntity.Slot.Lock) as BaseLock;
		if (!(slot != null) || slot.OnTryToOpen(player))
		{
			return true;
		}
		player.ChatMessage("It is locked...");
		return false;
	}

	public override bool CanPickup(BasePlayer player)
	{
		bool slot = base.GetSlot(BaseEntity.Slot.Lock) != null;
		if (base.isClient)
		{
			if (!base.CanPickup(player))
			{
				return false;
			}
			return !slot;
		}
		if (this.pickup.requireEmptyInv && this.inventory != null && this.inventory.itemList.Count != 0 || !base.CanPickup(player))
		{
			return false;
		}
		return !slot;
	}

	public void CreateInventory(bool giveUID)
	{
		this.inventory = new ItemContainer()
		{
			entityOwner = this,
			allowedContents = ((int)this.allowedContents == 0 ? ItemContainer.ContentsType.Generic : this.allowedContents),
			onlyAllowedItem = this.allowedItem,
			maxStackSize = this.maxStackSize
		};
		this.inventory.ServerInitialize(null, this.inventorySlots);
		if (giveUID)
		{
			this.inventory.GiveUID();
		}
		StorageContainer storageContainer = this;
		this.inventory.onDirty += new Action(storageContainer.OnInventoryDirty);
		StorageContainer storageContainer1 = this;
		this.inventory.onItemAddedRemoved = new Action<Item, bool>(storageContainer1.OnItemAddedOrRemoved);
		if (this.onlyAcceptCategory != ItemCategory.All)
		{
			StorageContainer storageContainer2 = this;
			this.inventory.canAcceptItem = new Func<Item, int, bool>(storageContainer2.ItemFilter);
		}
	}

	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		if (this.inventory != null)
		{
			this.inventory.Kill();
			this.inventory = null;
		}
	}

	public void DropItems()
	{
		if (this.inventory == null || this.inventory.itemList == null || this.inventory.itemList.Count == 0)
		{
			return;
		}
		if (this.dropChance == 0f)
		{
			return;
		}
		if (this.ShouldDropItemsIndividually() || this.inventory.itemList.Count == 1)
		{
			DropUtil.DropItems(this.inventory, this.GetDropPosition(), 1f);
			return;
		}
	}

	public override Vector3 GetDropPosition()
	{
		return base.transform.localToWorldMatrix.MultiplyPoint(this.dropPosition);
	}

	public override Vector3 GetDropVelocity()
	{
		Vector3 inheritedDropVelocity = base.GetInheritedDropVelocity();
		Matrix4x4 matrix4x4 = base.transform.localToWorldMatrix;
		return inheritedDropVelocity + matrix4x4.MultiplyVector(this.dropPosition);
	}

	public virtual string GetPanelName()
	{
		return this.panelName;
	}

	public override bool HasSlot(BaseEntity.Slot slot)
	{
		if (this.isLockable && slot == BaseEntity.Slot.Lock)
		{
			return true;
		}
		return base.HasSlot(slot);
	}

	public virtual bool ItemFilter(Item item, int targetSlot)
	{
		if (this.onlyAcceptCategory == ItemCategory.All)
		{
			return true;
		}
		return item.info.category == this.onlyAcceptCategory;
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.storageBox != null)
		{
			if (this.inventory != null)
			{
				this.inventory.Load(info.msg.storageBox.contents);
				this.inventory.capacity = this.inventorySlots;
				return;
			}
			Debug.LogWarning(string.Concat("Storage container without inventory: ", this.ToString()));
		}
	}

	public void MoveAllInventoryItems(ItemContainer source, ItemContainer dest)
	{
		for (int i = 0; i < Mathf.Min(source.capacity, dest.capacity); i++)
		{
			Item slot = source.GetSlot(i);
			if (slot != null)
			{
				slot.MoveToContainer(dest, -1, true);
			}
		}
	}

	public bool OccupiedCheck(BasePlayer player = null)
	{
		if (player != null && player.inventory.loot.entitySource == this)
		{
			return true;
		}
		if (!this.onlyOneUser)
		{
			return true;
		}
		return !base.IsOpen();
	}

	public void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.yellow;
		Gizmos.DrawCube(this.dropPosition, Vector3.one * 0.1f);
		Gizmos.color = Color.white;
		Gizmos.DrawRay(this.dropPosition, this.dropVelocity);
	}

	protected virtual void OnInventoryDirty()
	{
		base.InvalidateNetworkCache();
	}

	public virtual void OnInventoryFirstCreated(ItemContainer container)
	{
	}

	public virtual void OnItemAddedOrRemoved(Item item, bool added)
	{
	}

	public override void OnKilled(HitInfo info)
	{
		this.DropItems();
		base.OnKilled(info);
	}

	public override void OnPickedUp(Item createdItem, BasePlayer player)
	{
		base.OnPickedUp(createdItem, player);
		if (createdItem != null && createdItem.contents != null)
		{
			this.MoveAllInventoryItems(this.inventory, createdItem.contents);
			return;
		}
		for (int i = 0; i < this.inventory.capacity; i++)
		{
			Item slot = this.inventory.GetSlot(i);
			if (slot != null)
			{
				slot.RemoveFromContainer();
				player.GiveItem(slot, BaseEntity.GiveItemReason.PickedUp);
			}
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("StorageContainer.OnRpcMessage", 0.1f))
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

	public virtual bool PlayerOpenLoot(BasePlayer player)
	{
		return this.PlayerOpenLoot(player, this.panelName);
	}

	public virtual bool PlayerOpenLoot(BasePlayer player, string panelToOpen)
	{
		object obj = Interface.CallHook("CanLootEntity", player, this);
		if (obj as bool)
		{
			return (bool)obj;
		}
		if (base.IsLocked())
		{
			player.ChatMessage("Can't loot right now");
			return false;
		}
		if (this.onlyOneUser && base.IsOpen())
		{
			player.ChatMessage("Already in use");
			return false;
		}
		if (!this.CanOpenLootPanel(player, panelToOpen))
		{
			return false;
		}
		if (!player.inventory.loot.StartLootingEntity(this, true))
		{
			return false;
		}
		base.SetFlag(BaseEntity.Flags.Open, true, false, true);
		player.inventory.loot.AddContainer(this.inventory);
		player.inventory.loot.SendImmediate();
		player.ClientRPCPlayer<string>(null, player, "RPC_OpenLootPanel", panelToOpen);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		return true;
	}

	public virtual void PlayerStoppedLooting(BasePlayer player)
	{
		Interface.CallHook("OnLootEntityEnd", player, this);
		base.SetFlag(BaseEntity.Flags.Open, false, false, true);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (this.inventory != null && this.inventory.uid == 0)
		{
			this.inventory.GiveUID();
		}
		base.SetFlag(BaseEntity.Flags.Open, false, false, true);
	}

	public override void PreServerLoad()
	{
		base.PreServerLoad();
		this.CreateInventory(false);
	}

	public virtual void ReceiveInventoryFromItem(Item item)
	{
		if (item.contents != null)
		{
			this.MoveAllInventoryItems(item.contents, this.inventory);
		}
	}

	public override void ResetState()
	{
		base.ResetState();
		if (this.inventory != null)
		{
			this.inventory.Clear();
		}
		this.inventory = null;
	}

	[IsVisible(3f)]
	[RPC_Server]
	private void RPC_OpenLoot(BaseEntity.RPCMessage rpc)
	{
		if (!this.isLootable)
		{
			return;
		}
		this.PlayerOpenLoot(rpc.player);
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

	public virtual bool ShouldDropItemsIndividually()
	{
		return false;
	}
}