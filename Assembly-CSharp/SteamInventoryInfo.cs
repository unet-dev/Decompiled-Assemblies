using Rust.Workshop.Game;
using System;
using UnityEngine;

public class SteamInventoryInfo : SingletonComponent<SteamInventoryInfo>
{
	public GameObject inventoryItemPrefab;

	public GameObject inventoryCanvas;

	public GameObject missingItems;

	public WorkshopInventoryCraftingControls CraftControl;

	public SteamInventoryInfo()
	{
	}
}