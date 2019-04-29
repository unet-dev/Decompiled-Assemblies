using System;
using System.Collections.Generic;

public class ItemModUseContent : ItemMod
{
	public int amountToConsume = 1;

	public ItemModUseContent()
	{
	}

	public override void DoAction(Item item, BasePlayer player)
	{
		if (item.contents == null)
		{
			return;
		}
		if (item.contents.itemList.Count == 0)
		{
			return;
		}
		item.contents.itemList[0].UseItem(this.amountToConsume);
	}
}