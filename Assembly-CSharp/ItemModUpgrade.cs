using Oxide.Core;
using System;
using UnityEngine;

public class ItemModUpgrade : ItemMod
{
	public int numForUpgrade = 10;

	public float upgradeSuccessChance = 1f;

	public int numToLoseOnFail = 2;

	public ItemDefinition upgradedItem;

	public int numUpgradedItem = 1;

	public GameObjectRef successEffect;

	public GameObjectRef failEffect;

	public ItemModUpgrade()
	{
	}

	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		Vector3 vector3;
		if (command == "upgrade_item")
		{
			if (item.amount < this.numForUpgrade)
			{
				return;
			}
			if (UnityEngine.Random.Range(0f, 1f) > this.upgradeSuccessChance)
			{
				item.UseItem(this.numToLoseOnFail);
				if (this.failEffect.isValid)
				{
					string str = this.failEffect.resourcePath;
					Vector3 vector31 = player.eyes.position;
					vector3 = new Vector3();
					Effect.server.Run(str, vector31, vector3, null, false);
				}
			}
			else
			{
				item.UseItem(this.numForUpgrade);
				Item item1 = ItemManager.Create(this.upgradedItem, this.numUpgradedItem, (ulong)0);
				Interface.CallHook("OnItemUpgrade", item, item1, player);
				if (!item1.MoveToContainer(player.inventory.containerMain, -1, true))
				{
					item1.Drop(player.GetDropPosition(), player.GetDropVelocity(), new Quaternion());
				}
				if (this.successEffect.isValid)
				{
					string str1 = this.successEffect.resourcePath;
					Vector3 vector32 = player.eyes.position;
					vector3 = new Vector3();
					Effect.server.Run(str1, vector32, vector3, null, false);
					return;
				}
			}
		}
	}
}