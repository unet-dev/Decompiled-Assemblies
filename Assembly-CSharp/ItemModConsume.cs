using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ItemModConsumable))]
public class ItemModConsume : ItemMod
{
	public GameObjectRef consumeEffect;

	public string eatGesture = "eat_2hand";

	[Tooltip("Items that are given on consumption of this item")]
	public ItemAmountRandom[] product;

	public ItemModConsumable primaryConsumable;

	public ItemModConsume()
	{
	}

	public override bool CanDoAction(Item item, BasePlayer player)
	{
		return player.metabolism.CanConsume();
	}

	public override void DoAction(Item item, BasePlayer player)
	{
		if (item.amount < 1)
		{
			return;
		}
		GameObjectRef consumeEffect = this.GetConsumeEffect();
		if (consumeEffect.isValid)
		{
			Vector3 vector3 = (player.IsDucked() ? new Vector3(0f, 1f, 0f) : new Vector3(0f, 2f, 0f));
			Effect.server.Run(consumeEffect.resourcePath, player, 0, vector3, Vector3.zero, null, false);
		}
		player.metabolism.MarkConsumption();
		ItemModConsumable consumable = this.GetConsumable();
		float single = (float)Mathf.Max(consumable.amountToConsume, 1);
		float single1 = (float)Mathf.Min((float)item.amount, single);
		float single2 = single1 / single;
		float single3 = item.conditionNormalized;
		if (consumable.conditionFractionToLose > 0f)
		{
			single3 = consumable.conditionFractionToLose;
		}
		foreach (ItemModConsumable.ConsumableEffect effect in consumable.effects)
		{
			if (player.healthFraction > effect.onlyIfHealthLessThan)
			{
				continue;
			}
			if (effect.type != MetabolismAttribute.Type.Health)
			{
				player.metabolism.ApplyChange(effect.type, effect.amount * single2 * single3, effect.time * single2 * single3);
			}
			else if (effect.amount >= 0f)
			{
				BasePlayer basePlayer = player;
				basePlayer.health = basePlayer.health + effect.amount * single2 * single3;
			}
			else
			{
				player.OnAttacked(new HitInfo(player, player, DamageType.Generic, -effect.amount * single2 * single3, player.transform.position + (player.transform.forward * 1f)));
			}
		}
		if (this.product != null)
		{
			ItemAmountRandom[] itemAmountRandomArray = this.product;
			for (int i = 0; i < (int)itemAmountRandomArray.Length; i++)
			{
				ItemAmountRandom itemAmountRandom = itemAmountRandomArray[i];
				int num = Mathf.RoundToInt((float)itemAmountRandom.RandomAmount() * single3);
				if (num > 0)
				{
					Item item1 = ItemManager.Create(itemAmountRandom.itemDef, num, (ulong)0);
					player.GiveItem(item1, BaseEntity.GiveItemReason.Generic);
				}
			}
		}
		if (string.IsNullOrEmpty(this.eatGesture))
		{
			player.SignalBroadcast(BaseEntity.Signal.Gesture, this.eatGesture, null);
		}
		if (consumable.conditionFractionToLose <= 0f)
		{
			item.UseItem((int)single1);
			return;
		}
		item.LoseCondition(consumable.conditionFractionToLose * item.maxCondition);
	}

	public virtual ItemModConsumable GetConsumable()
	{
		if (this.primaryConsumable)
		{
			return this.primaryConsumable;
		}
		return base.GetComponent<ItemModConsumable>();
	}

	public virtual GameObjectRef GetConsumeEffect()
	{
		return this.consumeEffect;
	}
}