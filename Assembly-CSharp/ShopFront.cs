using ConVar;
using Network;
using Oxide.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ShopFront : StorageContainer
{
	public BasePlayer vendorPlayer;

	public BasePlayer customerPlayer;

	public GameObjectRef transactionCompleteEffect;

	public ItemContainer customerInventory;

	public ItemContainer vendorInventory
	{
		get
		{
			return this.inventory;
		}
	}

	public ShopFront()
	{
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void AcceptClicked(BaseEntity.RPCMessage msg)
	{
		if (!this.IsTradingPlayer(msg.player))
		{
			return;
		}
		if (this.vendorPlayer == null || this.customerPlayer == null)
		{
			return;
		}
		if (this.IsPlayerVendor(msg.player))
		{
			base.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
			this.vendorInventory.SetLocked(true);
		}
		else if (this.IsPlayerCustomer(msg.player))
		{
			base.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
			this.customerInventory.SetLocked(true);
		}
		if (base.HasFlag(BaseEntity.Flags.Reserved1) && base.HasFlag(BaseEntity.Flags.Reserved2))
		{
			base.SetFlag(BaseEntity.Flags.Reserved3, true, false, true);
			base.Invoke(new Action(this.CompleteTrade), 2f);
		}
	}

	private bool CanAcceptCustomerItem(Item item, int targetSlot)
	{
		if (this.customerPlayer != null && item.GetOwnerPlayer() == this.customerPlayer || this.customerInventory.itemList.Contains(item) || item.parent == null)
		{
			return true;
		}
		return false;
	}

	private bool CanAcceptVendorItem(Item item, int targetSlot)
	{
		if (this.vendorPlayer != null && item.GetOwnerPlayer() == this.vendorPlayer || this.vendorInventory.itemList.Contains(item) || item.parent == null)
		{
			return true;
		}
		return false;
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void CancelClicked(BaseEntity.RPCMessage msg)
	{
		if (!this.IsTradingPlayer(msg.player))
		{
			return;
		}
		bool flag = this.vendorPlayer;
		bool flag1 = this.customerPlayer;
		this.ResetTrade();
	}

	public override bool CanMoveFrom(BasePlayer player, Item item)
	{
		if (this.TradeLocked())
		{
			return false;
		}
		if (this.IsTradingPlayer(player))
		{
			if (this.IsPlayerCustomer(player) && this.customerInventory.itemList.Contains(item) && !this.customerInventory.IsLocked())
			{
				return true;
			}
			if (this.IsPlayerVendor(player) && this.vendorInventory.itemList.Contains(item) && !this.vendorInventory.IsLocked())
			{
				return true;
			}
		}
		return false;
	}

	public override bool CanOpenLootPanel(BasePlayer player, string panelName = "")
	{
		if (!base.CanOpenLootPanel(player, panelName))
		{
			return false;
		}
		return this.LootEligable(player);
	}

	public void CompleteTrade()
	{
		if (this.vendorPlayer != null && this.customerPlayer != null && base.HasFlag(BaseEntity.Flags.Reserved1) && base.HasFlag(BaseEntity.Flags.Reserved2))
		{
			if (Interface.CallHook("OnShopCompleteTrade", this) != null)
			{
				return;
			}
			for (int i = this.vendorInventory.capacity - 1; i >= 0; i--)
			{
				Item slot = this.vendorInventory.GetSlot(i);
				Item item = this.customerInventory.GetSlot(i);
				if (this.customerPlayer && slot != null)
				{
					this.customerPlayer.GiveItem(slot, BaseEntity.GiveItemReason.Generic);
				}
				if (this.vendorPlayer && item != null)
				{
					this.vendorPlayer.GiveItem(item, BaseEntity.GiveItemReason.Generic);
				}
			}
			Effect.server.Run(this.transactionCompleteEffect.resourcePath, this, 0, new Vector3(0f, 1f, 0f), Vector3.zero, null, false);
		}
		this.ResetTrade();
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public bool IsPlayerCustomer(BasePlayer player)
	{
		return player == this.customerPlayer;
	}

	public bool IsPlayerVendor(BasePlayer player)
	{
		return player == this.vendorPlayer;
	}

	public bool IsTradingPlayer(BasePlayer player)
	{
		if (player == null)
		{
			return false;
		}
		if (this.IsPlayerCustomer(player))
		{
			return true;
		}
		return this.IsPlayerVendor(player);
	}

	public bool LootEligable(BasePlayer player)
	{
		if (player == null)
		{
			return false;
		}
		if (this.PlayerInVendorPos(player) && this.vendorPlayer == null)
		{
			return true;
		}
		if (this.PlayerInCustomerPos(player) && this.customerPlayer == null)
		{
			return true;
		}
		return false;
	}

	public override void OnItemAddedOrRemoved(Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
		this.ResetTrade();
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		BaseEntity.RPCMessage rPCMessage;
		using (TimeWarning timeWarning = TimeWarning.New("ShopFront.OnRpcMessage", 0.1f))
		{
			if (rpc == 1159607245 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - AcceptClicked "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("AcceptClicked", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("AcceptClicked", this, player, 3f))
						{
							flag = true;
							return flag;
						}
					}
					try
					{
						using (timeWarning2 = TimeWarning.New("Call", 0.1f))
						{
							rPCMessage = new BaseEntity.RPCMessage()
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.AcceptClicked(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in AcceptClicked");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc != -1126859756 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - CancelClicked "));
				}
				using (timeWarning1 = TimeWarning.New("CancelClicked", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("CancelClicked", this, player, 3f))
						{
							flag = true;
							return flag;
						}
					}
					try
					{
						using (timeWarning2 = TimeWarning.New("Call", 0.1f))
						{
							rPCMessage = new BaseEntity.RPCMessage()
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.CancelClicked(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in CancelClicked");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public bool PlayerInCustomerPos(BasePlayer player)
	{
		Vector3 vector3 = base.transform.right;
		Vector3 vector31 = player.transform.position - base.transform.position;
		return Vector3.Dot(vector3, vector31.normalized) >= 0.7f;
	}

	public bool PlayerInVendorPos(BasePlayer player)
	{
		Vector3 vector3 = base.transform.right;
		Vector3 vector31 = player.transform.position - base.transform.position;
		return Vector3.Dot(vector3, vector31.normalized) <= -0.7f;
	}

	public override bool PlayerOpenLoot(BasePlayer player, string panelToOpen)
	{
		bool flag = base.PlayerOpenLoot(player, panelToOpen);
		if (flag)
		{
			player.inventory.loot.AddContainer(this.customerInventory);
			player.inventory.loot.SendImmediate();
		}
		if (!this.PlayerInVendorPos(player) || !(this.vendorPlayer == null))
		{
			if (!this.PlayerInCustomerPos(player) || !(this.customerPlayer == null))
			{
				return false;
			}
			this.customerPlayer = player;
		}
		else
		{
			this.vendorPlayer = player;
		}
		this.ResetTrade();
		this.UpdatePlayers();
		return flag;
	}

	public override void PlayerStoppedLooting(BasePlayer player)
	{
		if (!this.IsTradingPlayer(player))
		{
			return;
		}
		this.ReturnPlayerItems(player);
		if (player == this.vendorPlayer)
		{
			this.vendorPlayer = null;
		}
		if (player == this.customerPlayer)
		{
			this.customerPlayer = null;
		}
		this.UpdatePlayers();
		this.ResetTrade();
		base.PlayerStoppedLooting(player);
	}

	public override void PreServerLoad()
	{
		base.PreServerLoad();
	}

	public void ResetTrade()
	{
		base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved2, false, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
		this.vendorInventory.SetLocked(false);
		this.customerInventory.SetLocked(false);
		base.CancelInvoke(new Action(this.CompleteTrade));
	}

	public void ReturnPlayerItems(BasePlayer player)
	{
		if (this.IsTradingPlayer(player))
		{
			ItemContainer itemContainer = null;
			if (this.IsPlayerVendor(player))
			{
				itemContainer = this.vendorInventory;
			}
			else if (this.IsPlayerCustomer(player))
			{
				itemContainer = this.customerInventory;
			}
			if (itemContainer != null)
			{
				for (int i = itemContainer.itemList.Count - 1; i >= 0; i--)
				{
					player.GiveItem(itemContainer.itemList[i], BaseEntity.GiveItemReason.Generic);
				}
			}
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		this.vendorInventory.canAcceptItem += new Func<Item, int, bool>(this.CanAcceptVendorItem);
		if (this.customerInventory == null)
		{
			this.customerInventory = new ItemContainer()
			{
				allowedContents = ((int)this.allowedContents == 0 ? ItemContainer.ContentsType.Generic : this.allowedContents),
				onlyAllowedItem = this.allowedItem,
				entityOwner = this,
				maxStackSize = this.maxStackSize
			};
			this.customerInventory.ServerInitialize(null, this.inventorySlots);
			this.customerInventory.GiveUID();
			ShopFront shopFront = this;
			this.customerInventory.onDirty += new Action(shopFront.OnInventoryDirty);
			ShopFront shopFront1 = this;
			this.customerInventory.onItemAddedRemoved = new Action<Item, bool>(shopFront1.OnItemAddedOrRemoved);
			this.customerInventory.canAcceptItem += new Func<Item, int, bool>(this.CanAcceptCustomerItem);
			this.OnInventoryFirstCreated(this.customerInventory);
		}
	}

	public bool TradeLocked()
	{
		return false;
	}

	public void UpdatePlayers()
	{
		uint d;
		uint num;
		if (this.vendorPlayer == null)
		{
			d = 0;
		}
		else
		{
			d = this.vendorPlayer.net.ID;
		}
		if (this.customerPlayer == null)
		{
			num = 0;
		}
		else
		{
			num = this.customerPlayer.net.ID;
		}
		base.ClientRPC<uint, uint>(null, "CLIENT_ReceivePlayers", d, num);
	}

	public static class ShopFrontFlags
	{
		public const BaseEntity.Flags VendorAccepted = BaseEntity.Flags.Reserved1;

		public const BaseEntity.Flags CustomerAccepted = BaseEntity.Flags.Reserved2;

		public const BaseEntity.Flags Exchanging = BaseEntity.Flags.Reserved3;
	}
}