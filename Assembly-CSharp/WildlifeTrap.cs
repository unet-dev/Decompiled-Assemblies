using Rust;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class WildlifeTrap : StorageContainer
{
	public float tickRate = 60f;

	public GameObjectRef trappedEffect;

	public float trappedEffectRepeatRate = 30f;

	public float trapSuccessRate = 0.5f;

	public List<ItemDefinition> ignoreBait;

	public List<WildlifeTrap.WildlifeWeight> targetWildlife;

	public WildlifeTrap()
	{
	}

	public void ClearTrap()
	{
		base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
	}

	public void DestroyRandomFoodItem()
	{
		int count = this.inventory.itemList.Count;
		int num = UnityEngine.Random.Range(0, count);
		for (int i = 0; i < count; i++)
		{
			int num1 = num + i;
			if (num1 >= count)
			{
				num1 -= count;
			}
			Item item = this.inventory.itemList[num1];
			if (item != null && !(item.info.GetComponent<ItemModConsumable>() == null))
			{
				item.UseItem(1);
				return;
			}
		}
	}

	public int GetBaitCalories()
	{
		int num = 0;
		foreach (Item item in this.inventory.itemList)
		{
			ItemModConsumable component = item.info.GetComponent<ItemModConsumable>();
			if (component == null || this.ignoreBait.Contains(item.info))
			{
				continue;
			}
			foreach (ItemModConsumable.ConsumableEffect effect in component.effects)
			{
				if (effect.type != MetabolismAttribute.Type.Calories || effect.amount <= 0f)
				{
					continue;
				}
				num += Mathf.CeilToInt(effect.amount * (float)item.amount);
			}
		}
		return num;
	}

	public int GetItemCalories(Item item)
	{
		int num;
		ItemModConsumable component = item.info.GetComponent<ItemModConsumable>();
		if (component == null)
		{
			return 0;
		}
		List<ItemModConsumable.ConsumableEffect>.Enumerator enumerator = component.effects.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				ItemModConsumable.ConsumableEffect current = enumerator.Current;
				if (current.type != MetabolismAttribute.Type.Calories || current.amount <= 0f)
				{
					continue;
				}
				num = Mathf.CeilToInt(current.amount);
				return num;
			}
			return 0;
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		return num;
	}

	public TrappableWildlife GetRandomWildlife()
	{
		int item = this.targetWildlife.Sum<WildlifeTrap.WildlifeWeight>((WildlifeTrap.WildlifeWeight x) => x.weight);
		int num = UnityEngine.Random.Range(0, item);
		for (int i = 0; i < this.targetWildlife.Count; i++)
		{
			item -= this.targetWildlife[i].weight;
			if (num >= item)
			{
				return this.targetWildlife[i].wildlife;
			}
		}
		return null;
	}

	public bool HasBait()
	{
		return this.GetBaitCalories() > 0;
	}

	public bool HasCatch()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	public bool IsTrapActive()
	{
		return base.HasFlag(BaseEntity.Flags.On);
	}

	public override bool OnStartBeingLooted(BasePlayer baseEntity)
	{
		this.ClearTrap();
		return base.OnStartBeingLooted(baseEntity);
	}

	public override void PlayerStoppedLooting(BasePlayer player)
	{
		this.SetTrapActive(this.HasBait());
		this.ClearTrap();
		base.PlayerStoppedLooting(player);
	}

	public void SetTrapActive(bool trapOn)
	{
		if (trapOn == this.IsTrapActive())
		{
			return;
		}
		WildlifeTrap wildlifeTrap = this;
		base.CancelInvoke(new Action(wildlifeTrap.TrapThink));
		base.SetFlag(BaseEntity.Flags.On, trapOn, false, true);
		if (trapOn)
		{
			WildlifeTrap wildlifeTrap1 = this;
			base.InvokeRepeating(new Action(wildlifeTrap1.TrapThink), this.tickRate * 0.8f + this.tickRate * UnityEngine.Random.Range(0f, 0.4f), this.tickRate);
		}
	}

	public virtual void TrapThink()
	{
		int baitCalories = this.GetBaitCalories();
		if (baitCalories <= 0)
		{
			return;
		}
		TrappableWildlife randomWildlife = this.GetRandomWildlife();
		if (baitCalories >= randomWildlife.caloriesForInterest && UnityEngine.Random.Range(0f, 1f) <= randomWildlife.successRate)
		{
			this.UseBaitCalories(randomWildlife.caloriesForInterest);
			if (UnityEngine.Random.Range(0f, 1f) <= this.trapSuccessRate)
			{
				this.TrapWildlife(randomWildlife);
			}
		}
	}

	public void TrapWildlife(TrappableWildlife trapped)
	{
		Item item = ItemManager.Create(trapped.inventoryObject, UnityEngine.Random.Range(trapped.minToCatch, trapped.maxToCatch + 1), (ulong)0);
		if (item.MoveToContainer(this.inventory, -1, true))
		{
			base.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
		}
		else
		{
			item.Remove(0f);
		}
		this.SetTrapActive(false);
		this.Hurt(this.StartMaxHealth() * 0.1f, DamageType.Decay, null, false);
	}

	public void UseBaitCalories(int numToUse)
	{
		foreach (Item item in this.inventory.itemList)
		{
			int itemCalories = this.GetItemCalories(item);
			if (itemCalories <= 0)
			{
				continue;
			}
			numToUse -= itemCalories;
			item.UseItem(1);
			if (numToUse > 0)
			{
				continue;
			}
			return;
		}
	}

	public static class WildlifeTrapFlags
	{
		public const BaseEntity.Flags Occupied = BaseEntity.Flags.Reserved1;
	}

	[Serializable]
	public class WildlifeWeight
	{
		public TrappableWildlife wildlife;

		public int weight;

		public WildlifeWeight()
		{
		}
	}
}