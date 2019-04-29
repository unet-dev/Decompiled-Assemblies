using System;
using UnityEngine;

public class ItemMod : MonoBehaviour
{
	private ItemMod[] siblingMods;

	public ItemMod()
	{
	}

	public virtual bool CanDoAction(Item item, BasePlayer player)
	{
		ItemMod[] itemModArray = this.siblingMods;
		for (int i = 0; i < (int)itemModArray.Length; i++)
		{
			if (!itemModArray[i].Passes(item))
			{
				return false;
			}
		}
		return true;
	}

	public virtual void CollectedForCrafting(Item item, BasePlayer crafter)
	{
	}

	public virtual void DoAction(Item item, BasePlayer player)
	{
	}

	public virtual void ModInit()
	{
		this.siblingMods = base.GetComponents<ItemMod>();
	}

	public virtual void OnAttacked(Item item, HitInfo info)
	{
	}

	public virtual void OnChanged(Item item)
	{
	}

	public virtual void OnItemCreated(Item item)
	{
	}

	public virtual void OnMovedToWorld(Item item)
	{
	}

	public virtual void OnParentChanged(Item item)
	{
	}

	public virtual void OnRemove(Item item)
	{
	}

	public virtual void OnRemovedFromWorld(Item item)
	{
	}

	public virtual void OnVirginItem(Item item)
	{
	}

	public virtual bool Passes(Item item)
	{
		return true;
	}

	public virtual void ReturnedFromCancelledCraft(Item item, BasePlayer crafter)
	{
	}

	public virtual void ServerCommand(Item item, string command, BasePlayer player)
	{
	}
}