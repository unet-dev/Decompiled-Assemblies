using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemModStudyBlueprint : ItemMod
{
	public GameObjectRef studyEffect;

	public ItemModStudyBlueprint()
	{
	}

	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (item.GetOwnerPlayer() != player)
		{
			bool flag = false;
			foreach (ItemContainer container in player.inventory.loot.containers)
			{
				if (item.GetRootContainer() != container)
				{
					continue;
				}
				flag = true;
				goto Label0;
			}
		Label0:
			if (!flag)
			{
				return;
			}
		}
		if (command == "study")
		{
			if (!item.IsBlueprint())
			{
				return;
			}
			ItemDefinition itemDefinition = item.blueprintTargetDef;
			if (player.blueprints.IsUnlocked(itemDefinition))
			{
				return;
			}
			Item item1 = item;
			if (item.amount > 1)
			{
				item1 = item.SplitItem(1);
			}
			item1.UseItem(1);
			player.blueprints.Unlock(itemDefinition);
			if (this.studyEffect.isValid)
			{
				Effect.server.Run(this.studyEffect.resourcePath, player, StringPool.Get("head"), Vector3.zero, Vector3.zero, null, false);
			}
		}
	}
}