using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DevDressPlayer : MonoBehaviour
{
	public bool DressRandomly;

	public List<ItemAmount> clothesToWear;

	public DevDressPlayer()
	{
	}

	private void DoRandomClothes(BasePlayer player)
	{
		string str = "";
		foreach (ItemDefinition itemDefinition in (
			from x in ItemManager.GetItemDefinitions()
			where x.GetComponent<ItemModWearable>()
			orderby Guid.NewGuid()
			select x).Take<ItemDefinition>(UnityEngine.Random.Range(0, 4)))
		{
			ItemManager.Create(itemDefinition, 1, (ulong)0).MoveToContainer(player.inventory.containerWear, -1, true);
			str = string.Concat(str, itemDefinition.shortname, " ");
		}
		str = str.Trim();
		if (str == "")
		{
			str = "naked";
		}
		player.displayName = str;
	}

	private void ServerInitComponent()
	{
		BasePlayer component = base.GetComponent<BasePlayer>();
		if (this.DressRandomly)
		{
			this.DoRandomClothes(component);
		}
		foreach (ItemAmount itemAmount in this.clothesToWear)
		{
			if (itemAmount.itemDef == null)
			{
				continue;
			}
			ItemManager.Create(itemAmount.itemDef, 1, (ulong)0).MoveToContainer(component.inventory.containerWear, -1, true);
		}
	}
}