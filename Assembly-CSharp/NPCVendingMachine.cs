using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NPCVendingMachine : VendingMachine
{
	public NPCVendingOrder vendingOrders;

	public NPCVendingMachine()
	{
	}

	public void AddItemForSale(int itemID, int amountToSell, int currencyID, int currencyPerTransaction, byte bpState)
	{
		base.AddSellOrder(itemID, amountToSell, currencyID, currencyPerTransaction, bpState);
		this.transactionActive = true;
		int num = 10;
		if (bpState == 1 || bpState == 3)
		{
			for (int i = 0; i < num; i++)
			{
				Item item = ItemManager.CreateByItemID(this.blueprintBaseDef.itemid, 1, (ulong)0);
				item.blueprintTarget = itemID;
				this.inventory.Insert(item);
			}
		}
		else
		{
			this.inventory.AddItem(ItemManager.FindItemDefinition(itemID), amountToSell * num);
		}
		this.transactionActive = false;
		base.RefreshSellOrderStockLevel(null);
	}

	public override bool CanPlayerAdmin(BasePlayer player)
	{
		return false;
	}

	public void ClearSellOrders()
	{
		this.sellOrders.sellOrders.Clear();
	}

	public byte GetBPState(bool sellItemAsBP, bool currencyItemAsBP)
	{
		byte num = 0;
		if (sellItemAsBP)
		{
			num = 1;
		}
		if (currencyItemAsBP)
		{
			num = 2;
		}
		if (sellItemAsBP & currencyItemAsBP)
		{
			num = 3;
		}
		return num;
	}

	public override void GiveSoldItem(Item soldItem, BasePlayer buyer)
	{
		Item item = ItemManager.Create(soldItem.info, soldItem.amount, (ulong)0);
		base.GiveSoldItem(soldItem, buyer);
		this.transactionActive = true;
		if (!item.MoveToContainer(this.inventory, -1, true))
		{
			Debug.LogWarning(string.Concat(new string[] { "NPCVending machine unable to refill item :", soldItem.info.shortname, " buyer :", buyer.displayName, " - Contact Developers" }));
			item.Remove(0f);
		}
		this.transactionActive = false;
	}

	public override void InstallDefaultSellOrders()
	{
		base.InstallDefaultSellOrders();
	}

	public virtual void InstallFromVendingOrders()
	{
		if (this.vendingOrders == null)
		{
			return;
		}
		this.ClearSellOrders();
		this.inventory.Clear();
		ItemManager.DoRemoves();
		NPCVendingOrder.Entry[] entryArray = this.vendingOrders.orders;
		for (int i = 0; i < (int)entryArray.Length; i++)
		{
			NPCVendingOrder.Entry entry = entryArray[i];
			this.AddItemForSale(entry.sellItem.itemid, entry.sellItemAmount, entry.currencyItem.itemid, entry.currencyAmount, this.GetBPState(entry.sellItemAsBP, entry.currencyAsBP));
		}
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		NPCVendingMachine nPCVendingMachine = this;
		base.Invoke(new Action(nPCVendingMachine.InstallFromVendingOrders), 1f);
	}

	public void RefreshStock()
	{
	}

	public override void ServerInit()
	{
		base.ServerInit();
		this.skinID = (ulong)861142659;
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		NPCVendingMachine nPCVendingMachine = this;
		base.Invoke(new Action(nPCVendingMachine.InstallFromVendingOrders), 1f);
	}

	public override void TakeCurrencyItem(Item takenCurrencyItem)
	{
		takenCurrencyItem.MoveToContainer(this.inventory, -1, true);
		takenCurrencyItem.RemoveFromContainer();
		takenCurrencyItem.Remove(0f);
	}
}