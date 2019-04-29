using System;
using UnityEngine;

public class ItemModActionChange : ItemMod
{
	public ItemMod[] actions;

	public ItemModActionChange()
	{
	}

	public override void OnChanged(Item item)
	{
		if (!item.isServer)
		{
			return;
		}
		BasePlayer ownerPlayer = item.GetOwnerPlayer();
		ItemMod[] itemModArray = this.actions;
		for (int i = 0; i < (int)itemModArray.Length; i++)
		{
			ItemMod itemMod = itemModArray[i];
			if (itemMod.CanDoAction(item, ownerPlayer))
			{
				itemMod.DoAction(item, ownerPlayer);
			}
		}
	}

	private void OnValidate()
	{
		if (this.actions == null)
		{
			Debug.LogWarning("ItemModMenuOption: actions is null!", base.gameObject);
		}
	}
}