using System;
using UnityEngine;

public class ItemModRecycleInto : ItemMod
{
	public ItemDefinition recycleIntoItem;

	public int numRecycledItemMin = 1;

	public int numRecycledItemMax = 1;

	public GameObjectRef successEffect;

	public ItemModRecycleInto()
	{
	}

	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (command == "recycle_item")
		{
			int num = UnityEngine.Random.Range(this.numRecycledItemMin, this.numRecycledItemMax + 1);
			item.UseItem(1);
			if (num > 0)
			{
				Item item1 = ItemManager.Create(this.recycleIntoItem, num, (ulong)0);
				if (!item1.MoveToContainer(player.inventory.containerMain, -1, true))
				{
					item1.Drop(player.GetDropPosition(), player.GetDropVelocity(), new Quaternion());
				}
				if (this.successEffect.isValid)
				{
					Effect.server.Run(this.successEffect.resourcePath, player.eyes.position, new Vector3(), null, false);
				}
			}
		}
	}
}