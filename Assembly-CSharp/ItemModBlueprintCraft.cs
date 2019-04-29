using System;
using UnityEngine;

public class ItemModBlueprintCraft : ItemMod
{
	public GameObjectRef successEffect;

	public ItemModBlueprintCraft()
	{
	}

	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		Vector3 vector3;
		if (item.GetOwnerPlayer() != player)
		{
			return;
		}
		if (command == "craft")
		{
			if (!item.IsBlueprint())
			{
				return;
			}
			if (!player.inventory.crafting.CanCraft(item.blueprintTargetDef.Blueprint, 1))
			{
				return;
			}
			Item item1 = item;
			if (item.amount > 1)
			{
				item1 = item.SplitItem(1);
			}
			player.inventory.crafting.CraftItem(item.blueprintTargetDef.Blueprint, player, null, 1, 0, item1);
			if (this.successEffect.isValid)
			{
				string str = this.successEffect.resourcePath;
				Vector3 vector31 = player.eyes.position;
				vector3 = new Vector3();
				Effect.server.Run(str, vector31, vector3, null, false);
			}
		}
		if (command == "craft_all")
		{
			if (!item.IsBlueprint())
			{
				return;
			}
			if (!player.inventory.crafting.CanCraft(item.blueprintTargetDef.Blueprint, item.amount))
			{
				return;
			}
			player.inventory.crafting.CraftItem(item.blueprintTargetDef.Blueprint, player, null, item.amount, 0, item);
			if (this.successEffect.isValid)
			{
				string str1 = this.successEffect.resourcePath;
				Vector3 vector32 = player.eyes.position;
				vector3 = new Vector3();
				Effect.server.Run(str1, vector32, vector3, null, false);
			}
		}
	}
}