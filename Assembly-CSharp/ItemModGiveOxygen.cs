using System;
using UnityEngine;

public class ItemModGiveOxygen : ItemMod
{
	public int amountToConsume = 1;

	public GameObjectRef inhaleEffect;

	public GameObjectRef exhaleEffect;

	public GameObjectRef bubblesEffect;

	private bool inhaled;

	public ItemModGiveOxygen()
	{
	}

	public override void DoAction(Item item, BasePlayer player)
	{
		if (!item.hasCondition)
		{
			return;
		}
		if (item.conditionNormalized == 0f)
		{
			return;
		}
		if (player == null)
		{
			return;
		}
		if (player.WaterFactor() < 1f)
		{
			return;
		}
		if (item.parent == null)
		{
			return;
		}
		if (item.parent != player.inventory.containerWear)
		{
			return;
		}
		Effect.server.Run((!this.inhaled ? this.inhaleEffect.resourcePath : this.exhaleEffect.resourcePath), player, StringPool.Get("jaw"), Vector3.zero, Vector3.forward, null, false);
		this.inhaled = !this.inhaled;
		if (!this.inhaled && WaterLevel.GetWaterDepth(player.eyes.position) > 3f)
		{
			Effect.server.Run(this.bubblesEffect.resourcePath, player, StringPool.Get("jaw"), Vector3.zero, Vector3.forward, null, false);
		}
		item.LoseCondition((float)this.amountToConsume);
		player.metabolism.oxygen.Add(1f);
	}
}