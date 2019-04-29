using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Rust/Player Inventory Properties")]
public class PlayerInventoryProperties : ScriptableObject
{
	public string niceName;

	public int order = 100;

	public List<ItemAmount> belt;

	public List<ItemAmount> main;

	public List<ItemAmount> wear;

	public List<PlayerInventoryProperties.ItemAmountSkinned> skinnedWear;

	public PlayerInventoryProperties()
	{
	}

	public void GiveToPlayer(BasePlayer player)
	{
		if (player == null)
		{
			return;
		}
		player.inventory.Strip();
		foreach (ItemAmount itemAmount in this.belt)
		{
			player.inventory.GiveItem(ItemManager.Create(itemAmount.itemDef, (int)itemAmount.amount, (ulong)0), player.inventory.containerBelt);
		}
		foreach (ItemAmount itemAmount1 in this.main)
		{
			player.inventory.GiveItem(ItemManager.Create(itemAmount1.itemDef, (int)itemAmount1.amount, (ulong)0), player.inventory.containerMain);
		}
		if (this.skinnedWear.Count > 0)
		{
			foreach (PlayerInventoryProperties.ItemAmountSkinned itemAmountSkinned in this.skinnedWear)
			{
				player.inventory.GiveItem(ItemManager.Create(itemAmountSkinned.itemDef, (int)itemAmountSkinned.amount, itemAmountSkinned.GetRandomSkin()), player.inventory.containerWear);
			}
		}
		foreach (ItemAmount itemAmount2 in this.wear)
		{
			player.inventory.GiveItem(ItemManager.Create(itemAmount2.itemDef, (int)itemAmount2.amount, (ulong)0), player.inventory.containerWear);
		}
	}

	[Serializable]
	public class ItemAmountSkinned : ItemAmount
	{
		public ulong skinOverride;

		public ItemAmountSkinned() : base(null, 0f)
		{
		}

		public ulong GetRandomSkin()
		{
			return this.skinOverride;
		}
	}
}