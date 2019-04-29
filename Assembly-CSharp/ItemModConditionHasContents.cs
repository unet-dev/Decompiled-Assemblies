using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ItemModConditionHasContents : ItemMod
{
	[Tooltip("Can be null to mean any item")]
	public ItemDefinition itemDef;

	public bool requiredState;

	public ItemModConditionHasContents()
	{
	}

	public override bool Passes(Item item)
	{
		if (item.contents == null)
		{
			return !this.requiredState;
		}
		if (item.contents.itemList.Count == 0)
		{
			return !this.requiredState;
		}
		if (!this.itemDef || item.contents.itemList.Any<Item>((Item x) => x.info == this.itemDef))
		{
			return this.requiredState;
		}
		return !this.requiredState;
	}
}