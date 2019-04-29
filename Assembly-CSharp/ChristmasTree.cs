using System;
using System.Collections.Generic;
using UnityEngine;

public class ChristmasTree : StorageContainer
{
	public GameObject[] decorations;

	public ChristmasTree()
	{
	}

	public override bool ItemFilter(Item item, int targetSlot)
	{
		bool flag;
		if (item.info.GetComponent<ItemModXMasTreeDecoration>() == null)
		{
			return false;
		}
		List<Item>.Enumerator enumerator = this.inventory.itemList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.info != item.info)
				{
					continue;
				}
				flag = false;
				return flag;
			}
			return base.ItemFilter(item, targetSlot);
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		return flag;
	}

	public override void OnItemAddedOrRemoved(Item item, bool added)
	{
		ItemModXMasTreeDecoration component = item.info.GetComponent<ItemModXMasTreeDecoration>();
		if (component != null)
		{
			base.SetFlag((BaseEntity.Flags)component.flagsToChange, added, false, true);
		}
		base.OnItemAddedOrRemoved(item, added);
	}
}