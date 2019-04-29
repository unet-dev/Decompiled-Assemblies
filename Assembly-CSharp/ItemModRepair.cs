using System;
using UnityEngine;

public class ItemModRepair : ItemMod
{
	public float conditionLost = 0.05f;

	public GameObjectRef successEffect;

	public int workbenchLvlRequired;

	public ItemModRepair()
	{
	}

	public bool HasCraftLevel(BasePlayer player = null)
	{
		if (!(player != null) || !player.isServer)
		{
			return false;
		}
		return player.currentCraftLevel >= (float)this.workbenchLvlRequired;
	}

	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (command == "refill")
		{
			if (player.IsSwimming())
			{
				return;
			}
			if (!this.HasCraftLevel(player))
			{
				return;
			}
			if (item.conditionNormalized >= 1f)
			{
				return;
			}
			item.DoRepair(this.conditionLost);
			if (this.successEffect.isValid)
			{
				Effect.server.Run(this.successEffect.resourcePath, player.eyes.position, new Vector3(), null, false);
			}
		}
	}
}