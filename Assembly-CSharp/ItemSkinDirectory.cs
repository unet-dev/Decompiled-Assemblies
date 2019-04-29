using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ItemSkinDirectory : ScriptableObject
{
	private static ItemSkinDirectory _Instance;

	public ItemSkinDirectory.Skin[] skins;

	public static ItemSkinDirectory Instance
	{
		get
		{
			if (ItemSkinDirectory._Instance == null)
			{
				ItemSkinDirectory._Instance = FileSystem.Load<ItemSkinDirectory>("assets/skins.asset", true);
			}
			return ItemSkinDirectory._Instance;
		}
	}

	public ItemSkinDirectory()
	{
	}

	public static ItemSkinDirectory.Skin FindByInventoryDefinitionId(int id)
	{
		return (
			from x in ItemSkinDirectory.Instance.skins
			where x.id == id
			select x).FirstOrDefault<ItemSkinDirectory.Skin>();
	}

	public static ItemSkinDirectory.Skin[] ForItem(ItemDefinition item)
	{
		return ItemSkinDirectory.Instance.skins.Where<ItemSkinDirectory.Skin>((ItemSkinDirectory.Skin x) => {
			if (!x.isSkin)
			{
				return false;
			}
			return x.itemid == item.itemid;
		}).ToArray<ItemSkinDirectory.Skin>();
	}

	[Serializable]
	public struct Skin
	{
		public int id;

		public int itemid;

		public string name;

		public bool isSkin;

		private SteamInventoryItem _invItem;

		public SteamInventoryItem invItem
		{
			get
			{
				if (this._invItem == null)
				{
					this._invItem = FileSystem.Load<SteamInventoryItem>(this.name, true);
				}
				return this._invItem;
			}
		}
	}
}