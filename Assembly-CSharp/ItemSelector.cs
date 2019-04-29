using System;
using UnityEngine;

public class ItemSelector : PropertyAttribute
{
	public ItemCategory category = ItemCategory.All;

	public ItemSelector(ItemCategory category = 11)
	{
		this.category = category;
	}
}