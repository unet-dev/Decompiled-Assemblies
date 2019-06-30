using Steamworks;
using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rust.UI
{
	public class SteamInventoryCrafting : MonoBehaviour
	{
		public GameObject Container;

		public Button ConvertToItem;

		public TextMeshProUGUI WoodAmount;

		public TextMeshProUGUI ClothAmount;

		public TextMeshProUGUI MetalAmount;

		public int SelectedCount;

		public TextMeshProUGUI InfoText;

		public SteamInventoryCrateOpen CraftModal;

		public GameObject CraftingContainer;

		public GameObject CraftingButton;

		public SteamInventoryNewItem NewItemModal;

		public Coroutine MarketCoroutine
		{
			get;
			private set;
		}

		public InventoryDef ResultItem
		{
			get;
			private set;
		}

		public SteamInventoryCrafting()
		{
		}
	}
}