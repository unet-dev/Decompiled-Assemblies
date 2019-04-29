using System;
using UnityEngine;

public class ItemModUnwrap : ItemMod
{
	public LootSpawn revealList;

	public GameObjectRef successEffect;

	public int minTries = 1;

	public int maxTries = 1;

	public ItemModUnwrap()
	{
	}

	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (command == "unwrap")
		{
			if (item.amount <= 0)
			{
				return;
			}
			item.UseItem(1);
			int num = UnityEngine.Random.Range(this.minTries, this.maxTries + 1);
			for (int i = 0; i < num; i++)
			{
				this.revealList.SpawnIntoContainer(player.inventory.containerMain);
			}
			if (this.successEffect.isValid)
			{
				Effect.server.Run(this.successEffect.resourcePath, player.eyes.position, new Vector3(), null, false);
			}
		}
	}
}