using ConVar;
using Network;
using Oxide.Core;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;

public class VendingMachine : StorageContainer
{
	[Header("VendingMachine")]
	public GameObjectRef adminMenuPrefab;

	public string customerPanel = "";

	public ProtoBuf.VendingMachine.SellOrderContainer sellOrders;

	public SoundPlayer buySound;

	public string shopName = "A Shop";

	public GameObjectRef mapMarkerPrefab;

	public ItemDefinition blueprintBaseDef;

	protected BasePlayer vend_Player;

	private int vend_sellOrderID;

	private int vend_numberOfTransactions;

	public bool transactionActive;

	private VendingMachineMapMarker myMarker;

	public VendingMachine()
	{
	}

	public void AddSellOrder(int itemToSellID, int itemToSellAmount, int currencyToUseID, int currencyAmount, byte bpState)
	{
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(itemToSellID);
		ItemDefinition itemDefinition1 = ItemManager.FindItemDefinition(currencyToUseID);
		if (itemDefinition == null || itemDefinition1 == null)
		{
			return;
		}
		currencyAmount = Mathf.Clamp(currencyAmount, 1, 10000);
		itemToSellAmount = Mathf.Clamp(itemToSellAmount, 1, itemDefinition.stackable);
		ProtoBuf.VendingMachine.SellOrder sellOrder = new ProtoBuf.VendingMachine.SellOrder()
		{
			ShouldPool = false,
			itemToSellID = itemToSellID,
			itemToSellAmount = itemToSellAmount,
			currencyID = currencyToUseID,
			currencyAmountPerItem = currencyAmount,
			currencyIsBP = (bpState == 3 ? true : bpState == 2),
			itemToSellIsBP = (bpState == 3 ? true : bpState == 1)
		};
		Interface.CallHook("OnAddVendingOffer", this, sellOrder);
		this.sellOrders.sellOrders.Add(sellOrder);
		this.RefreshSellOrderStockLevel(itemDefinition);
		this.UpdateMapMarker();
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void BuyItem(BaseEntity.RPCMessage rpc)
	{
		if (!base.OccupiedCheck(rpc.player))
		{
			return;
		}
		int num = rpc.read.Int32();
		int num1 = rpc.read.Int32();
		if (Interface.CallHook("OnBuyVendingItem", this, rpc.player, num, num1) != null)
		{
			return;
		}
		this.SetPendingOrder(rpc.player, num, num1);
		VendingMachine vendingMachine = this;
		base.Invoke(new Action(vendingMachine.CompletePendingOrder), this.GetBuyDuration());
	}

	public bool CanAcceptItem(Item item, int targetSlot)
	{
		object obj = Interface.CallHook("CanVendingAcceptItem", this, item, targetSlot);
		if (obj as bool)
		{
			return (bool)obj;
		}
		BasePlayer ownerPlayer = item.GetOwnerPlayer();
		if (this.transactionActive)
		{
			return true;
		}
		if (item.parent == null)
		{
			return true;
		}
		if (this.inventory.itemList.Contains(item))
		{
			return true;
		}
		if (ownerPlayer == null)
		{
			return false;
		}
		return this.CanPlayerAdmin(ownerPlayer);
	}

	public override bool CanMoveFrom(BasePlayer player, Item item)
	{
		return this.CanPlayerAdmin(player);
	}

	public override bool CanOpenLootPanel(BasePlayer player, string panelName = "")
	{
		object obj = Interface.CallHook("CanUseVending", player, this);
		if (obj as bool)
		{
			return (bool)obj;
		}
		if (panelName == this.customerPanel)
		{
			return true;
		}
		if (!base.CanOpenLootPanel(player, panelName))
		{
			return false;
		}
		return this.CanPlayerAdmin(player);
	}

	public virtual bool CanPlayerAdmin(BasePlayer player)
	{
		object obj = Interface.CallHook("CanAdministerVending", player, this);
		if (obj as bool)
		{
			return (bool)obj;
		}
		if (!this.PlayerBehind(player))
		{
			return false;
		}
		return base.OccupiedCheck(player);
	}

	public void ClearPendingOrder()
	{
		VendingMachine vendingMachine = this;
		base.CancelInvoke(new Action(vendingMachine.CompletePendingOrder));
		this.vend_Player = null;
		this.vend_sellOrderID = -1;
		this.vend_numberOfTransactions = -1;
		base.SetFlag(BaseEntity.Flags.Reserved2, false, false, true);
		base.ClientRPC(null, "CLIENT_CancelVendingSounds");
	}

	public virtual void CompletePendingOrder()
	{
		this.DoTransaction(this.vend_Player, this.vend_sellOrderID, this.vend_numberOfTransactions);
		this.ClearPendingOrder();
		Decay.RadialDecayTouch(base.transform.position, 40f, 2097408);
	}

	public override void DestroyShared()
	{
		if (this.myMarker)
		{
			this.myMarker.Kill(BaseNetworkable.DestroyMode.None);
			this.myMarker = null;
		}
		base.DestroyShared();
	}

	public bool DoTransaction(BasePlayer buyer, int sellOrderId, int numberOfTransactions = 1)
	{
		Item item;
		Item item1;
		if (sellOrderId < 0 || sellOrderId > this.sellOrders.sellOrders.Count)
		{
			return false;
		}
		if (Vector3.Distance(buyer.transform.position, base.transform.position) > 4f)
		{
			return false;
		}
		object obj = Interface.CallHook("OnVendingTransaction", this, buyer, sellOrderId, numberOfTransactions);
		if (obj as bool)
		{
			return (bool)obj;
		}
		ProtoBuf.VendingMachine.SellOrder sellOrder = this.sellOrders.sellOrders[sellOrderId];
		List<Item> list = this.inventory.FindItemsByItemID(sellOrder.itemToSellID);
		if (sellOrder.itemToSellIsBP)
		{
			list = (
				from x in this.inventory.FindItemsByItemID(this.blueprintBaseDef.itemid)
				where x.blueprintTarget == sellOrder.itemToSellID
				select x).ToList<Item>();
		}
		if (list == null || list.Count == 0)
		{
			return false;
		}
		numberOfTransactions = Mathf.Clamp(numberOfTransactions, 1, (list[0].hasCondition ? 1 : 1000000));
		int num = sellOrder.itemToSellAmount * numberOfTransactions;
		if (num > list.Sum<Item>((Item x) => x.amount))
		{
			return false;
		}
		List<Item> items = buyer.inventory.FindItemIDs(sellOrder.currencyID);
		if (sellOrder.currencyIsBP)
		{
			items = (
				from x in buyer.inventory.FindItemIDs(this.blueprintBaseDef.itemid)
				where x.blueprintTarget == sellOrder.currencyID
				select x).ToList<Item>();
		}
		items = items.Where<Item>((Item x) => {
			if (!x.hasCondition)
			{
				return true;
			}
			if (x.conditionNormalized < 0.5f)
			{
				return false;
			}
			return x.maxConditionNormalized > 0.5f;
		}).ToList<Item>();
		if (items.Count == 0)
		{
			return false;
		}
		List<Item> items1 = items;
		int num1 = sellOrder.currencyAmountPerItem * numberOfTransactions;
		if (items1.Sum<Item>((Item x) => x.amount) < num1)
		{
			return false;
		}
		this.transactionActive = true;
		int num2 = 0;
		foreach (Item item2 in items)
		{
			int num3 = Mathf.Min(num1 - num2, item2.amount);
			item = (item2.amount > num3 ? item2.SplitItem(num3) : item2);
			this.TakeCurrencyItem(item);
			num2 += num3;
			if (num2 < num1)
			{
				continue;
			}
			goto Label0;
		}
	Label0:
		int num4 = 0;
		foreach (Item item3 in list)
		{
			int num5 = num - num4;
			item1 = (item3.amount > num5 ? item3.SplitItem(num5) : item3);
			if (item1 != null)
			{
				num4 += item1.amount;
				this.GiveSoldItem(item1, buyer);
			}
			else
			{
				Debug.LogError("Vending machine error, contact developers!");
			}
			if (num4 < num)
			{
				continue;
			}
			this.UpdateEmptyFlag();
			this.transactionActive = false;
			return true;
		}
		this.UpdateEmptyFlag();
		this.transactionActive = false;
		return true;
	}

	public void FullUpdate()
	{
		this.RefreshSellOrderStockLevel(null);
		this.UpdateMapMarker();
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public virtual float GetBuyDuration()
	{
		return 2.5f;
	}

	public virtual void GiveSoldItem(Item soldItem, BasePlayer buyer)
	{
		buyer.GiveItem(soldItem, BaseEntity.GiveItemReason.PickedUp);
	}

	public virtual bool HasVendingSounds()
	{
		return true;
	}

	public virtual void InstallDefaultSellOrders()
	{
		this.sellOrders = new ProtoBuf.VendingMachine.SellOrderContainer()
		{
			ShouldPool = false,
			sellOrders = new List<ProtoBuf.VendingMachine.SellOrder>()
		};
	}

	public bool IsBroadcasting()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved4);
	}

	public bool IsInventoryEmpty()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	public bool IsVending()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved2);
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.vendingMachine != null)
		{
			this.shopName = info.msg.vendingMachine.shopName;
			if (info.msg.vendingMachine.sellOrderContainer != null)
			{
				this.sellOrders = info.msg.vendingMachine.sellOrderContainer;
				this.sellOrders.ShouldPool = false;
			}
			if (info.fromDisk && base.isServer)
			{
				this.RefreshSellOrderStockLevel(null);
			}
		}
	}

	protected override void OnInventoryDirty()
	{
		base.OnInventoryDirty();
		base.CancelInvoke(new Action(this.FullUpdate));
		base.Invoke(new Action(this.FullUpdate), 0.2f);
	}

	public override void OnItemAddedOrRemoved(Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		BaseEntity.RPCMessage rPCMessage;
		using (TimeWarning timeWarning = TimeWarning.New("VendingMachine.OnRpcMessage", 0.1f))
		{
			if (rpc == -1283913593 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - BuyItem "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("BuyItem", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("BuyItem", this, player, 3f))
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
							this.BuyItem(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in BuyItem");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc == 1626480840 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_AddSellOrder "));
				}
				using (timeWarning1 = TimeWarning.New("RPC_AddSellOrder", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_AddSellOrder", this, player, 3f))
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
							this.RPC_AddSellOrder(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in RPC_AddSellOrder");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
			else if (rpc == 169239598 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_Broadcast "));
				}
				using (timeWarning1 = TimeWarning.New("RPC_Broadcast", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_Broadcast", this, player, 3f))
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
							this.RPC_Broadcast(rPCMessage);
						}
					}
					catch (Exception exception2)
					{
						player.Kick("RPC Error in RPC_Broadcast");
						Debug.LogException(exception2);
					}
				}
				flag = true;
			}
			else if (rpc == -614066159 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_DeleteSellOrder "));
				}
				using (timeWarning1 = TimeWarning.New("RPC_DeleteSellOrder", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_DeleteSellOrder", this, player, 3f))
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
							this.RPC_DeleteSellOrder(rPCMessage);
						}
					}
					catch (Exception exception3)
					{
						player.Kick("RPC Error in RPC_DeleteSellOrder");
						Debug.LogException(exception3);
					}
				}
				flag = true;
			}
			else if (rpc == -1738973937 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_OpenAdmin "));
				}
				using (timeWarning1 = TimeWarning.New("RPC_OpenAdmin", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_OpenAdmin", this, player, 3f))
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
							this.RPC_OpenAdmin(rPCMessage);
						}
					}
					catch (Exception exception4)
					{
						player.Kick("RPC Error in RPC_OpenAdmin");
						Debug.LogException(exception4);
					}
				}
				flag = true;
			}
			else if (rpc == 36164441 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_OpenShop "));
				}
				using (timeWarning1 = TimeWarning.New("RPC_OpenShop", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_OpenShop", this, player, 3f))
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
							this.RPC_OpenShop(rPCMessage);
						}
					}
					catch (Exception exception5)
					{
						player.Kick("RPC Error in RPC_OpenShop");
						Debug.LogException(exception5);
					}
				}
				flag = true;
			}
			else if (rpc == -948454197 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_RotateVM "));
				}
				using (timeWarning1 = TimeWarning.New("RPC_RotateVM", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_RotateVM", this, player, 3f))
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
							this.RPC_RotateVM(rPCMessage);
						}
					}
					catch (Exception exception6)
					{
						player.Kick("RPC Error in RPC_RotateVM");
						Debug.LogException(exception6);
					}
				}
				flag = true;
			}
			else if (rpc == 1012779214 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_UpdateShopName "));
				}
				using (timeWarning1 = TimeWarning.New("RPC_UpdateShopName", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_UpdateShopName", this, player, 3f))
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
							this.RPC_UpdateShopName(rPCMessage);
						}
					}
					catch (Exception exception7)
					{
						player.Kick("RPC Error in RPC_UpdateShopName");
						Debug.LogException(exception7);
					}
				}
				flag = true;
			}
			else if (rpc != -735952465 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - TransactionStart "));
				}
				using (timeWarning1 = TimeWarning.New("TransactionStart", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("TransactionStart", this, player, 3f))
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
							this.TransactionStart(rPCMessage);
						}
					}
					catch (Exception exception8)
					{
						player.Kick("RPC Error in TransactionStart");
						Debug.LogException(exception8);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public bool OutOfStock()
	{
		bool flag;
		List<ProtoBuf.VendingMachine.SellOrder>.Enumerator enumerator = this.sellOrders.sellOrders.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.inStock <= 0)
				{
					continue;
				}
				flag = true;
				return flag;
			}
			return false;
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		return flag;
	}

	public bool PlayerBehind(BasePlayer player)
	{
		Vector3 vector3 = base.transform.forward;
		Vector3 vector31 = player.transform.position - base.transform.position;
		return Vector3.Dot(vector3, vector31.normalized) <= -0.7f;
	}

	public bool PlayerInfront(BasePlayer player)
	{
		Vector3 vector3 = base.transform.forward;
		Vector3 vector31 = player.transform.position - base.transform.position;
		return Vector3.Dot(vector3, vector31.normalized) >= 0.7f;
	}

	public override void PlayerStoppedLooting(BasePlayer player)
	{
		base.PlayerStoppedLooting(player);
		this.UpdateEmptyFlag();
		if (this.vend_Player != null && this.vend_Player == player)
		{
			this.ClearPendingOrder();
		}
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.RefreshSellOrderStockLevel(null);
		this.UpdateMapMarker();
	}

	public void RefreshAndSendNetworkUpdate()
	{
		this.RefreshSellOrderStockLevel(null);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public void RefreshSellOrderStockLevel(ItemDefinition itemDef = null)
	{
		foreach (ProtoBuf.VendingMachine.SellOrder sellOrder in this.sellOrders.sellOrders)
		{
			if (!(itemDef == null) && itemDef.itemid != sellOrder.itemToSellID)
			{
				continue;
			}
			if (!sellOrder.itemToSellIsBP)
			{
				List<Item> items = this.inventory.FindItemsByItemID(sellOrder.itemToSellID);
				sellOrder.inStock = (items == null || items.Count < 0 ? 0 : items.Sum<Item>((Item x) => x.amount) / sellOrder.itemToSellAmount);
			}
			else
			{
				List<Item> list = (
					from x in this.inventory.FindItemsByItemID(this.blueprintBaseDef.itemid)
					where x.blueprintTarget == sellOrder.itemToSellID
					select x).ToList<Item>();
				sellOrder.inStock = (list == null || list.Count<Item>() < 0 ? 0 : list.Sum<Item>((Item x) => x.amount) / sellOrder.itemToSellAmount);
			}
		}
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void RPC_AddSellOrder(BaseEntity.RPCMessage msg)
	{
		BasePlayer basePlayer = msg.player;
		if (!this.CanPlayerAdmin(basePlayer))
		{
			return;
		}
		if (this.sellOrders.sellOrders.Count >= 7)
		{
			basePlayer.ChatMessage("Too many sell orders - remove some");
			return;
		}
		int num = msg.read.Int32();
		int num1 = msg.read.Int32();
		int num2 = msg.read.Int32();
		int num3 = msg.read.Int32();
		byte num4 = msg.read.UInt8();
		this.AddSellOrder(num, num1, num2, num3, num4);
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void RPC_Broadcast(BaseEntity.RPCMessage msg)
	{
		BasePlayer basePlayer = msg.player;
		bool flag = msg.read.Bit();
		if (this.CanPlayerAdmin(basePlayer))
		{
			base.SetFlag(BaseEntity.Flags.Reserved4, flag, false, true);
			Interface.CallHook("OnToggleVendingBroadcast", this, basePlayer);
			this.UpdateMapMarker();
		}
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void RPC_DeleteSellOrder(BaseEntity.RPCMessage msg)
	{
		BasePlayer basePlayer = msg.player;
		if (!this.CanPlayerAdmin(basePlayer))
		{
			return;
		}
		int num = msg.read.Int32();
		Interface.CallHook("OnDeleteVendingOffer", this, num);
		if (num >= 0 && num < this.sellOrders.sellOrders.Count)
		{
			this.sellOrders.sellOrders.RemoveAt(num);
		}
		this.RefreshSellOrderStockLevel(null);
		this.UpdateMapMarker();
		this.SendSellOrders(basePlayer);
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void RPC_OpenAdmin(BaseEntity.RPCMessage msg)
	{
		BasePlayer basePlayer = msg.player;
		if (!this.CanPlayerAdmin(basePlayer))
		{
			return;
		}
		this.SendSellOrders(basePlayer);
		this.PlayerOpenLoot(basePlayer);
		base.ClientRPCPlayer(null, basePlayer, "CLIENT_OpenAdminMenu");
		Interface.CallHook("OnOpenVendingAdmin", this, basePlayer);
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void RPC_OpenShop(BaseEntity.RPCMessage msg)
	{
		if (!base.OccupiedCheck(msg.player))
		{
			return;
		}
		this.SendSellOrders(msg.player);
		this.PlayerOpenLoot(msg.player, this.customerPanel);
		Interface.CallHook("OnOpenVendingShop", this, msg.player);
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void RPC_RotateVM(BaseEntity.RPCMessage msg)
	{
		if (Interface.CallHook("OnRotateVendingMachine", this, msg.player) != null)
		{
			return;
		}
		if (msg.player.CanBuild() && this.IsInventoryEmpty())
		{
			base.transform.rotation = Quaternion.LookRotation(-base.transform.forward, base.transform.up);
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void RPC_UpdateShopName(BaseEntity.RPCMessage msg)
	{
		BasePlayer basePlayer = msg.player;
		string printable = msg.read.String().ToPrintable(32);
		if (this.CanPlayerAdmin(basePlayer))
		{
			this.shopName = printable;
			this.UpdateMapMarker();
		}
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.vendingMachine = new ProtoBuf.VendingMachine()
		{
			ShouldPool = false,
			shopName = this.shopName
		};
		if (this.sellOrders != null)
		{
			info.msg.vendingMachine.sellOrderContainer = new ProtoBuf.VendingMachine.SellOrderContainer()
			{
				ShouldPool = false,
				sellOrders = new List<ProtoBuf.VendingMachine.SellOrder>()
			};
			foreach (ProtoBuf.VendingMachine.SellOrder sellOrder in this.sellOrders.sellOrders)
			{
				ProtoBuf.VendingMachine.SellOrder sellOrder1 = new ProtoBuf.VendingMachine.SellOrder()
				{
					ShouldPool = false
				};
				sellOrder.CopyTo(sellOrder1);
				info.msg.vendingMachine.sellOrderContainer.sellOrders.Add(sellOrder1);
			}
		}
	}

	public void SendSellOrders(BasePlayer player = null)
	{
		if (!player)
		{
			base.ClientRPC<ProtoBuf.VendingMachine.SellOrderContainer>(null, "CLIENT_ReceiveSellOrders", this.sellOrders);
			return;
		}
		base.ClientRPCPlayer<ProtoBuf.VendingMachine.SellOrderContainer>(null, player, "CLIENT_ReceiveSellOrders", this.sellOrders);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (base.isServer)
		{
			this.InstallDefaultSellOrders();
			base.SetFlag(BaseEntity.Flags.Reserved2, false, false, true);
			VendingMachine vendingMachine = this;
			this.inventory.onItemAddedRemoved = new Action<Item, bool>(vendingMachine.OnItemAddedOrRemoved);
			this.RefreshSellOrderStockLevel(null);
			this.inventory.canAcceptItem += new Func<Item, int, bool>(this.CanAcceptItem);
			this.UpdateMapMarker();
		}
	}

	public void SetPendingOrder(BasePlayer buyer, int sellOrderId, int numberOfTransactions)
	{
		this.ClearPendingOrder();
		this.vend_Player = buyer;
		this.vend_sellOrderID = sellOrderId;
		this.vend_numberOfTransactions = numberOfTransactions;
		base.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
		if (this.HasVendingSounds())
		{
			base.ClientRPC<int>(null, "CLIENT_StartVendingSounds", sellOrderId);
		}
	}

	public virtual void TakeCurrencyItem(Item takenCurrencyItem)
	{
		if (!takenCurrencyItem.MoveToContainer(this.inventory, -1, true))
		{
			takenCurrencyItem.Drop(this.inventory.dropPosition, Vector3.zero, new Quaternion());
		}
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void TransactionStart(BaseEntity.RPCMessage rpc)
	{
	}

	public void UpdateEmptyFlag()
	{
		base.SetFlag(BaseEntity.Flags.Reserved1, this.inventory.itemList.Count == 0, false, true);
	}

	public void UpdateMapMarker()
	{
		if (!this.IsBroadcasting())
		{
			if (this.myMarker)
			{
				this.myMarker.Kill(BaseNetworkable.DestroyMode.None);
				this.myMarker = null;
			}
			return;
		}
		bool flag = false;
		if (this.myMarker == null)
		{
			this.myMarker = GameManager.server.CreateEntity(this.mapMarkerPrefab.resourcePath, base.transform.position, Quaternion.identity, true) as VendingMachineMapMarker;
			flag = true;
		}
		this.myMarker.SetFlag(BaseEntity.Flags.Busy, this.OutOfStock(), false, true);
		this.myMarker.markerShopName = this.shopName;
		this.myMarker.server_vendingMachine = this;
		if (flag)
		{
			this.myMarker.Spawn();
			return;
		}
		this.myMarker.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public void UpdateOrCreateSalesSheet()
	{
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition("note");
		Item item = null;
		foreach (Item item1 in this.inventory.FindItemsByItemID(itemDefinition.itemid))
		{
			if (item1.text.Length != 0)
			{
				continue;
			}
			item = item1;
			goto Label0;
		}
	Label0:
		if (item == null)
		{
			ItemDefinition itemDefinition1 = ItemManager.FindItemDefinition("paper");
			Item item2 = this.inventory.FindItemByItemID(itemDefinition1.itemid);
			if (item2 != null)
			{
				item = ItemManager.CreateByItemID(itemDefinition.itemid, 1, (ulong)0);
				if (!item.MoveToContainer(this.inventory, -1, true))
				{
					item.Drop(this.GetDropPosition(), this.GetDropVelocity(), new Quaternion());
				}
				item2.UseItem(1);
			}
		}
		if (item != null)
		{
			foreach (ProtoBuf.VendingMachine.SellOrder sellOrder in this.sellOrders.sellOrders)
			{
				ItemDefinition itemDefinition2 = ItemManager.FindItemDefinition(sellOrder.itemToSellID);
				Item item3 = item;
				item3.text = string.Concat(item3.text, itemDefinition2.displayName.translated, "\n");
			}
			item.MarkDirty();
		}
	}

	public static class VendingMachineFlags
	{
		public const BaseEntity.Flags EmptyInv = BaseEntity.Flags.Reserved1;

		public const BaseEntity.Flags IsVending = BaseEntity.Flags.Reserved2;

		public const BaseEntity.Flags Broadcasting = BaseEntity.Flags.Reserved4;

		public const BaseEntity.Flags OutOfStock = BaseEntity.Flags.Reserved5;
	}
}