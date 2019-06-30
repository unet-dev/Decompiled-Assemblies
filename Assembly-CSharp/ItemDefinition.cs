using Rust;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ItemDefinition : MonoBehaviour
{
	[Header("Item")]
	[ReadOnly]
	public int itemid;

	[Tooltip("The shortname should be unique. A hash will be generated from it to identify the item type. If this name changes at any point it will make all saves incompatible")]
	public string shortname;

	[Header("Appearance")]
	public Translate.Phrase displayName;

	public Translate.Phrase displayDescription;

	public Sprite iconSprite;

	public ItemCategory category;

	public ItemSelectionPanel selectionPanel;

	[Header("Containment")]
	public int maxDraggable;

	public ItemContainer.ContentsType itemType = ItemContainer.ContentsType.Generic;

	public ItemDefinition.AmountType amountType;

	[InspectorFlags]
	public ItemSlot occupySlots = ItemSlot.None;

	public int stackable;

	public bool quickDespawn;

	[Header("Spawn Tables")]
	public Rarity rarity;

	public bool spawnAsBlueprint;

	[Header("Sounds")]
	public SoundDefinition inventorySelectSound;

	public SoundDefinition inventoryGrabSound;

	public SoundDefinition inventoryDropSound;

	public SoundDefinition physImpactSoundDef;

	public ItemDefinition.Condition condition;

	[Header("Misc")]
	public bool hidden;

	[InspectorFlags]
	public ItemDefinition.Flag flags;

	[Tooltip("User can craft this item on any server if they have this steam item")]
	public SteamInventoryItem steamItem;

	[Tooltip("Can only craft this item if the parent is craftable (tech tree)")]
	public ItemDefinition Parent;

	public GameObjectRef worldModelPrefab;

	[NonSerialized]
	public ItemMod[] itemMods;

	public BaseEntity.TraitFlag Traits;

	[NonSerialized]
	public ItemSkinDirectory.Skin[] skins;

	[NonSerialized]
	public InventoryDef[] _skins2;

	[Tooltip("Panel to show in the inventory menu when selected")]
	public GameObject panel;

	[NonSerialized]
	public ItemDefinition[] Children = new ItemDefinition[0];

	public ItemBlueprint Blueprint
	{
		get
		{
			return base.GetComponent<ItemBlueprint>();
		}
	}

	public bool CraftableWithSkin
	{
		get;
		private set;
	}

	public int craftingStackable
	{
		get
		{
			return Mathf.Max(10, this.stackable);
		}
	}

	public bool HasSkins
	{
		get
		{
			if (this.skins2 != null && this.skins2.Length != 0)
			{
				return true;
			}
			if (this.skins != null && this.skins.Length != 0)
			{
				return true;
			}
			return false;
		}
	}

	public bool isHoldable
	{
		get;
		private set;
	}

	public bool isUsable
	{
		get;
		private set;
	}

	public bool isWearable
	{
		get
		{
			return this.ItemModWearable != null;
		}
	}

	public ItemModWearable ItemModWearable
	{
		get;
		private set;
	}

	public InventoryDef[] skins2
	{
		get
		{
			if (this._skins2 != null)
			{
				return this._skins2;
			}
			if (SteamServer.IsValid && Steamworks.SteamInventory.Definitions != null)
			{
				string str = base.name;
				this._skins2 = Steamworks.SteamInventory.Definitions.Where<InventoryDef>((InventoryDef x) => {
					if (!(x.GetProperty("itemshortname") == this.shortname) && !(x.GetProperty("itemshortname") == str))
					{
						return false;
					}
					return !string.IsNullOrEmpty(x.GetProperty("workshopdownload"));
				}).ToArray<InventoryDef>();
			}
			return this._skins2;
		}
	}

	public ItemDefinition()
	{
	}

	public static ulong FindSkin(int itemID, int skinID)
	{
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(itemID);
		if (itemDefinition == null)
		{
			return (ulong)0;
		}
		InventoryDef inventoryDef = Steamworks.SteamInventory.FindDefinition(skinID);
		if (inventoryDef != null)
		{
			ulong property = inventoryDef.GetProperty<ulong>("workshopdownload");
			if (property != 0)
			{
				string str = inventoryDef.GetProperty<string>("itemshortname");
				if (str == itemDefinition.shortname || str == itemDefinition.name)
				{
					return property;
				}
			}
		}
		for (int i = 0; i < (int)itemDefinition.skins.Length; i++)
		{
			if (itemDefinition.skins[i].id == skinID)
			{
				return (ulong)skinID;
			}
		}
		return (ulong)0;
	}

	public bool HasFlag(ItemDefinition.Flag f)
	{
		return (this.flags & f) == f;
	}

	public void Initialize(List<ItemDefinition> itemList)
	{
		if (this.itemMods != null)
		{
			Debug.LogError(string.Concat("Item Definition Initializing twice: ", base.name));
		}
		this.skins = ItemSkinDirectory.ForItem(this);
		this.itemMods = base.GetComponentsInChildren<ItemMod>(true);
		ItemMod[] itemModArray = this.itemMods;
		for (int i = 0; i < (int)itemModArray.Length; i++)
		{
			itemModArray[i].ModInit();
		}
		this.Children = (
			from x in itemList
			where x.Parent == this
			select x).ToArray<ItemDefinition>();
		this.ItemModWearable = base.GetComponent<ItemModWearable>();
		this.isHoldable = base.GetComponent<ItemModEntity>() != null;
		this.isUsable = (base.GetComponent<ItemModEntity>() != null ? true : base.GetComponent<ItemModConsume>() != null);
	}

	public void InvalidateWorkshopSkinCache()
	{
		this._skins2 = null;
	}

	public enum AmountType
	{
		Count,
		Millilitre,
		Feet,
		Genetics,
		OxygenSeconds,
		Frequency
	}

	[Serializable]
	public struct Condition
	{
		public bool enabled;

		[Tooltip("The maximum condition this item type can have, new items will start with this value")]
		public float max;

		[Tooltip("If false then item will destroy when condition reaches 0")]
		public bool repairable;

		[Tooltip("If true, never lose max condition when repaired")]
		public bool maintainMaxCondition;

		public ItemDefinition.Condition.WorldSpawnCondition foundCondition;

		[Serializable]
		public class WorldSpawnCondition
		{
			public float fractionMin;

			public float fractionMax;

			public WorldSpawnCondition()
			{
			}
		}
	}

	[Flags]
	public enum Flag
	{
		NoDropping = 1,
		NotStraightToBelt = 2
	}
}