using System;
using UnityEngine;

public class ItemModReveal : ItemMod
{
	public int numForReveal = 10;

	public ItemDefinition revealedItemOverride;

	public int revealedItemAmount = 1;

	public LootSpawn revealList;

	public GameObjectRef successEffect;

	public ItemModReveal()
	{
	}

	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (command == "reveal")
		{
			if (item.amount < this.numForReveal)
			{
				return;
			}
			int num = item.position;
			item.UseItem(this.numForReveal);
			Item item1 = null;
			if (this.revealedItemOverride)
			{
				item1 = ItemManager.Create(this.revealedItemOverride, this.revealedItemAmount, (ulong)0);
			}
			if (item1 != null)
			{
				if (!item1.MoveToContainer(player.inventory.containerMain, (item.amount == 0 ? num : -1), true))
				{
					item1.Drop(player.GetDropPosition(), player.GetDropVelocity(), new Quaternion());
				}
			}
			if (this.successEffect.isValid)
			{
				Effect.server.Run(this.successEffect.resourcePath, player.eyes.position, new Vector3(), null, false);
			}
		}
	}
}