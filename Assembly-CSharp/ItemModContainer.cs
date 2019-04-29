using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemModContainer : ItemMod
{
	public int capacity = 6;

	public int maxStackSize;

	[InspectorFlags]
	public ItemContainer.Flag containerFlags;

	public ItemContainer.ContentsType onlyAllowedContents = ItemContainer.ContentsType.Generic;

	public ItemDefinition onlyAllowedItemType;

	public List<ItemSlot> availableSlots = new List<ItemSlot>();

	public bool openInDeployed = true;

	public bool openInInventory = true;

	public List<ItemAmount> defaultContents = new List<ItemAmount>();

	public ItemModContainer()
	{
	}

	public override void CollectedForCrafting(Item item, BasePlayer crafter)
	{
		if (item.contents == null)
		{
			return;
		}
		for (int i = item.contents.itemList.Count - 1; i >= 0; i--)
		{
			Item item1 = item.contents.itemList[i];
			if (!item1.MoveToContainer(crafter.inventory.containerMain, -1, true))
			{
				item1.Drop(crafter.GetDropPosition(), crafter.GetDropVelocity(), new Quaternion());
			}
		}
	}

	public override void OnItemCreated(Item item)
	{
		if (!item.isServer)
		{
			return;
		}
		if (this.capacity <= 0)
		{
			return;
		}
		if (item.contents != null)
		{
			return;
		}
		item.contents = new ItemContainer()
		{
			flags = this.containerFlags,
			allowedContents = ((int)this.onlyAllowedContents == 0 ? ItemContainer.ContentsType.Generic : this.onlyAllowedContents),
			onlyAllowedItem = this.onlyAllowedItemType,
			availableSlots = this.availableSlots
		};
		item.contents.ServerInitialize(item, this.capacity);
		item.contents.maxStackSize = this.maxStackSize;
		item.contents.GiveUID();
	}

	public override void OnVirginItem(Item item)
	{
		base.OnVirginItem(item);
		foreach (ItemAmount defaultContent in this.defaultContents)
		{
			Item item1 = ItemManager.Create(defaultContent.itemDef, (int)defaultContent.amount, (ulong)0);
			if (item1 == null)
			{
				continue;
			}
			item1.MoveToContainer(item.contents, -1, true);
		}
	}
}