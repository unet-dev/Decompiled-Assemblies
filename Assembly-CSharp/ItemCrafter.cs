using ConVar;
using Facepunch;
using Facepunch.Rust;
using Oxide.Core;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ItemCrafter : EntityComponent<BasePlayer>
{
	public List<ItemContainer> containers = new List<ItemContainer>();

	public Queue<ItemCraftTask> queue = new Queue<ItemCraftTask>();

	public int taskUID;

	public ItemCrafter()
	{
	}

	public void AddContainer(ItemContainer container)
	{
		this.containers.Add(container);
	}

	public void CancelAll(bool returnItems)
	{
		foreach (ItemCraftTask itemCraftTask in this.queue)
		{
			this.CancelTask(itemCraftTask.taskUID, returnItems);
		}
	}

	public bool CancelBlueprint(int itemid)
	{
		if (this.queue.Count == 0)
		{
			return false;
		}
		ItemCraftTask itemCraftTask = this.queue.FirstOrDefault<ItemCraftTask>((ItemCraftTask x) => {
			if (x.blueprint.targetItem.itemid != itemid)
			{
				return false;
			}
			return !x.cancelled;
		});
		if (itemCraftTask == null)
		{
			return false;
		}
		return this.CancelTask(itemCraftTask.taskUID, true);
	}

	public bool CancelTask(int iID, bool ReturnItems)
	{
		if (this.queue.Count == 0)
		{
			return false;
		}
		ItemCraftTask itemCraftTask = this.queue.FirstOrDefault<ItemCraftTask>((ItemCraftTask x) => {
			if (x.taskUID != iID)
			{
				return false;
			}
			return !x.cancelled;
		});
		if (itemCraftTask == null)
		{
			return false;
		}
		itemCraftTask.cancelled = true;
		if (itemCraftTask.owner == null)
		{
			return true;
		}
		Interface.CallHook("OnItemCraftCancelled", itemCraftTask);
		itemCraftTask.owner.Command("note.craft_done", new object[] { itemCraftTask.taskUID, 0 });
		if ((itemCraftTask.takenItems == null ? false : itemCraftTask.takenItems.Count > 0) & ReturnItems)
		{
			foreach (Item takenItem in itemCraftTask.takenItems)
			{
				if (takenItem == null || takenItem.amount <= 0)
				{
					continue;
				}
				if (takenItem.IsBlueprint() && takenItem.blueprintTargetDef == itemCraftTask.blueprint.targetItem)
				{
					takenItem.UseItem(itemCraftTask.numCrafted);
				}
				if (takenItem.amount <= 0 || takenItem.MoveToContainer(itemCraftTask.owner.inventory.containerMain, -1, true))
				{
					continue;
				}
				takenItem.Drop((itemCraftTask.owner.inventory.containerMain.dropPosition + (UnityEngine.Random.@value * Vector3.down)) + UnityEngine.Random.insideUnitSphere, itemCraftTask.owner.inventory.containerMain.dropVelocity, new Quaternion());
				itemCraftTask.owner.Command("note.inv", new object[] { takenItem.info.itemid, -takenItem.amount });
			}
		}
		return true;
	}

	public bool CanCraft(ItemBlueprint bp, int amount = 1)
	{
		bool flag;
		float single = (float)amount / (float)bp.targetItem.craftingStackable;
		foreach (ItemCraftTask itemCraftTask in this.queue)
		{
			if (itemCraftTask.cancelled)
			{
				continue;
			}
			single = single + (float)itemCraftTask.amount / (float)itemCraftTask.blueprint.targetItem.craftingStackable;
		}
		if (single > 8f)
		{
			return false;
		}
		if (amount < 1 || amount > bp.targetItem.craftingStackable)
		{
			return false;
		}
		object obj = Interface.CallHook("CanCraft", this, bp, amount);
		if (obj as bool)
		{
			return (bool)obj;
		}
		List<ItemAmount>.Enumerator enumerator = bp.ingredients.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				ItemAmount current = enumerator.Current;
				if (this.DoesHaveUsableItem(current.itemid, (int)current.amount * amount))
				{
					continue;
				}
				flag = false;
				return flag;
			}
			return true;
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		return flag;
	}

	public bool CanCraft(ItemDefinition def, int amount = 1)
	{
		if (this.CanCraft(def.GetComponent<ItemBlueprint>(), amount))
		{
			return true;
		}
		return false;
	}

	private void CollectIngredient(int item, int amount, List<Item> collect)
	{
		foreach (ItemContainer container in this.containers)
		{
			amount -= container.Take(collect, item, amount);
			if (amount > 0)
			{
				continue;
			}
			return;
		}
	}

	private void CollectIngredients(ItemBlueprint bp, ItemCraftTask task, int amount = 1, BasePlayer player = null)
	{
		List<Item> items = new List<Item>();
		foreach (ItemAmount ingredient in bp.ingredients)
		{
			this.CollectIngredient(ingredient.itemid, (int)ingredient.amount * amount, items);
		}
		task.potentialOwners = new List<ulong>();
		foreach (Item item in items)
		{
			item.CollectedForCrafting(player);
			if (task.potentialOwners.Contains(player.userID))
			{
				continue;
			}
			task.potentialOwners.Add(player.userID);
		}
		task.takenItems = items;
	}

	public bool CraftItem(ItemBlueprint bp, BasePlayer owner, ProtoBuf.Item.InstanceData instanceData = null, int amount = 1, int skinID = 0, Item fromTempBlueprint = null)
	{
		if (!this.CanCraft(bp, amount))
		{
			return false;
		}
		this.taskUID++;
		ItemCraftTask itemCraftTask = Facepunch.Pool.Get<ItemCraftTask>();
		itemCraftTask.blueprint = bp;
		this.CollectIngredients(bp, itemCraftTask, amount, owner);
		itemCraftTask.endTime = 0f;
		itemCraftTask.taskUID = this.taskUID;
		itemCraftTask.owner = owner;
		itemCraftTask.instanceData = instanceData;
		if (itemCraftTask.instanceData != null)
		{
			itemCraftTask.instanceData.ShouldPool = false;
		}
		itemCraftTask.amount = amount;
		itemCraftTask.skinID = skinID;
		if (fromTempBlueprint != null && itemCraftTask.takenItems != null)
		{
			fromTempBlueprint.RemoveFromContainer();
			itemCraftTask.takenItems.Add(fromTempBlueprint);
			itemCraftTask.conditionScale = 0.5f;
		}
		object obj = Interface.CallHook("OnItemCraft", itemCraftTask, owner, fromTempBlueprint);
		if (obj as bool)
		{
			return (bool)obj;
		}
		this.queue.Enqueue(itemCraftTask);
		if (itemCraftTask.owner != null)
		{
			itemCraftTask.owner.Command("note.craft_add", new object[] { itemCraftTask.taskUID, itemCraftTask.blueprint.targetItem.itemid, amount, itemCraftTask.skinID });
		}
		return true;
	}

	private bool DoesHaveUsableItem(int item, int iAmount)
	{
		int amount = 0;
		foreach (ItemContainer container in this.containers)
		{
			amount += container.GetAmount(item, true);
		}
		return amount >= iAmount;
	}

	public void FinishCrafting(ItemCraftTask task)
	{
		task.amount--;
		task.numCrafted++;
		ulong num = ItemDefinition.FindSkin(task.blueprint.targetItem.itemid, task.skinID);
		Item item = ItemManager.CreateByItemID(task.blueprint.targetItem.itemid, 1, num);
		item.amount = task.blueprint.amountToCreate;
		if (item.hasCondition && task.conditionScale != 1f)
		{
			Item item1 = item;
			item1.maxCondition = item1.maxCondition * task.conditionScale;
			item.condition = item.maxCondition;
		}
		item.OnVirginSpawn();
		foreach (ItemAmount ingredient in task.blueprint.ingredients)
		{
			int num1 = (int)ingredient.amount;
			if (task.takenItems == null)
			{
				continue;
			}
			foreach (Item takenItem in task.takenItems)
			{
				if (takenItem.info == ingredient.itemDef)
				{
					int num2 = Mathf.Min(takenItem.amount, num1);
					takenItem.UseItem(num1);
					num1 -= num2;
				}
			}
		}
		Facepunch.Rust.Analytics.Crafting(task.blueprint.targetItem.shortname, task.skinID);
		task.owner.Command("note.craft_done", new object[] { task.taskUID, 1, task.amount });
		Interface.CallHook("OnItemCraftFinished", task, item);
		if (task.instanceData != null)
		{
			item.instanceData = task.instanceData;
		}
		if (!string.IsNullOrEmpty(task.blueprint.UnlockAchievment))
		{
			task.owner.GiveAchievement(task.blueprint.UnlockAchievment);
		}
		if (task.owner.inventory.GiveItem(item, null))
		{
			task.owner.Command("note.inv", new object[] { item.info.itemid, item.amount });
			return;
		}
		ItemContainer itemContainer = this.containers.First<ItemContainer>();
		task.owner.Command("note.inv", new object[] { item.info.itemid, item.amount });
		task.owner.Command("note.inv", new object[] { item.info.itemid, -item.amount });
		item.Drop(itemContainer.dropPosition, itemContainer.dropVelocity, new Quaternion());
	}

	public static float GetScaledDuration(ItemBlueprint bp, float workbenchLevel)
	{
		float single = workbenchLevel - (float)bp.workbenchLevelRequired;
		if (single == 1f)
		{
			return bp.time * 0.5f;
		}
		if (single < 2f)
		{
			return bp.time;
		}
		return bp.time * 0.25f;
	}

	public void ServerUpdate(float delta)
	{
		if (this.queue.Count == 0)
		{
			return;
		}
		ItemCraftTask itemCraftTask = this.queue.Peek();
		if (itemCraftTask.cancelled)
		{
			itemCraftTask.owner.Command("note.craft_done", new object[] { itemCraftTask.taskUID, 0 });
			this.queue.Dequeue();
			return;
		}
		float single = itemCraftTask.owner.currentCraftLevel;
		if (itemCraftTask.endTime > UnityEngine.Time.realtimeSinceStartup)
		{
			return;
		}
		if (itemCraftTask.endTime != 0f)
		{
			this.FinishCrafting(itemCraftTask);
			if (itemCraftTask.amount > 0)
			{
				itemCraftTask.endTime = 0f;
				return;
			}
			this.queue.Dequeue();
			return;
		}
		float scaledDuration = ItemCrafter.GetScaledDuration(itemCraftTask.blueprint, single);
		itemCraftTask.endTime = UnityEngine.Time.realtimeSinceStartup + scaledDuration;
		if (itemCraftTask.owner != null)
		{
			itemCraftTask.owner.Command("note.craft_start", new object[] { itemCraftTask.taskUID, scaledDuration, itemCraftTask.amount });
			if (itemCraftTask.owner.IsAdmin && Craft.instant)
			{
				itemCraftTask.endTime = UnityEngine.Time.realtimeSinceStartup + 1f;
			}
		}
	}
}