using System;
using UnityEngine;

public class ItemModSwap : ItemMod
{
	public GameObjectRef actionEffect;

	public ItemAmount[] becomeItem;

	public bool sendPlayerPickupNotification;

	public bool sendPlayerDropNotification;

	public float xpScale = 1f;

	public ItemModSwap()
	{
	}

	public override void DoAction(Item item, BasePlayer player)
	{
		if (item.amount < 1)
		{
			return;
		}
		ItemAmount[] itemAmountArray = this.becomeItem;
		for (int i = 0; i < (int)itemAmountArray.Length; i++)
		{
			ItemAmount itemAmount = itemAmountArray[i];
			Item item1 = ItemManager.Create(itemAmount.itemDef, (int)itemAmount.amount, (ulong)0);
			if (item1 != null)
			{
				if (!item1.MoveToContainer(item.parent, -1, true))
				{
					player.GiveItem(item1, BaseEntity.GiveItemReason.Generic);
				}
				if (this.sendPlayerPickupNotification)
				{
					player.Command("note.inv", new object[] { item1.info.itemid, item1.amount });
				}
			}
		}
		if (this.sendPlayerDropNotification)
		{
			player.Command("note.inv", new object[] { item.info.itemid, -1 });
		}
		if (this.actionEffect.isValid)
		{
			Effect.server.Run(this.actionEffect.resourcePath, player.transform.position, Vector3.up, null, false);
		}
		item.UseItem(1);
	}
}