using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemModConsumeContents : ItemMod
{
	public GameObjectRef consumeEffect;

	public ItemModConsumeContents()
	{
	}

	public override bool CanDoAction(Item item, BasePlayer player)
	{
		bool flag;
		if (!player.metabolism.CanConsume())
		{
			return false;
		}
		if (item.contents == null)
		{
			return false;
		}
		List<Item>.Enumerator enumerator = item.contents.itemList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Item current = enumerator.Current;
				ItemModConsume component = current.info.GetComponent<ItemModConsume>();
				if (component == null || !component.CanDoAction(current, player))
				{
					continue;
				}
				flag = true;
				return flag;
			}
			return false;
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		return flag;
	}

	public override void DoAction(Item item, BasePlayer player)
	{
		foreach (Item content in item.contents.itemList)
		{
			ItemModConsume component = content.info.GetComponent<ItemModConsume>();
			if (component == null || !component.CanDoAction(content, player))
			{
				continue;
			}
			component.DoAction(content, player);
			return;
		}
	}
}