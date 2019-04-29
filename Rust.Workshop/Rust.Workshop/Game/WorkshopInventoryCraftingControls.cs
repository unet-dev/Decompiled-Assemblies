using Facepunch.Steamworks;
using Newtonsoft.Json;
using Rust;
using Rust.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Rust.Workshop.Game
{
	public class WorkshopInventoryCraftingControls : MonoBehaviour
	{
		public GameObject Container;

		public Button ConvertToItem;

		internal List<Inventory.Result> CraftingResult = new List<Inventory.Result>();

		public Text WoodAmount;

		public Text ClothAmount;

		public Text MetalAmount;

		public int SelectedCount;

		public Text InfoText;

		public WorkshopCraftInfoModal CraftModal;

		public GameObject CraftingContainer;

		public GameObject CraftingButton;

		private Dictionary<int, WorkshopInventoryCraftingControls.MarketPrice> priceCache = new Dictionary<int, WorkshopInventoryCraftingControls.MarketPrice>();

		public Coroutine MarketCoroutine
		{
			get;
			private set;
		}

		public Inventory.Definition ResultItem
		{
			get;
			private set;
		}

		public WorkshopInventoryCraftingControls()
		{
		}

		public void DoExchange()
		{
			WorkshopInventoryItem[] array = (
				from x in (IEnumerable<Toggle>)this.Container.GetComponentsInChildren<Toggle>()
				where x.isOn
				select x.GetComponent<WorkshopInventoryItem>()).ToArray<WorkshopInventoryItem>();
			if (array.Count<WorkshopInventoryItem>() == 0)
			{
				return;
			}
			base.StartCoroutine(this.ExchangeItems(array));
		}

		private IEnumerator ExchangeItems(WorkshopInventoryItem[] selected)
		{
			WorkshopInventoryCraftingControls workshopInventoryCraftingControl = null;
			int i;
			Inventory.Definition resultItem = workshopInventoryCraftingControl.ResultItem;
			WorkshopInventoryItem[] workshopInventoryItemArray = selected;
			for (i = 0; i < (int)workshopInventoryItemArray.Length; i++)
			{
				WorkshopInventoryItem workshopInventoryItem = workshopInventoryItemArray[i];
				workshopInventoryItem.GetComponent<Toggle>().isOn = false;
				workshopInventoryItem.GetComponent<Toggle>().interactable = false;
			}
			for (int j = 0; j < (int)selected.Length; j = i + 1)
			{
				List<Inventory.Result> craftingResult = workshopInventoryCraftingControl.CraftingResult;
				Inventory inventory = Global.SteamClient.Inventory;
				Inventory.Item[] item = new Inventory.Item[] { selected[j].Item };
				craftingResult.Add(inventory.CraftItem(item, resultItem));
				yield return new WaitForSeconds(0.1f);
				i = j;
			}
		}

		private void FillCraftingContainer()
		{
			if (Global.SteamClient.Inventory.Definitions == null)
			{
				return;
			}
			Transform[] array = this.CraftingContainer.transform.Cast<Transform>().ToArray<Transform>();
			for (int i = 0; i < (int)array.Length; i++)
			{
				UnityEngine.Object.Destroy(array[i].gameObject);
			}
			foreach (Inventory.Definition definition in 
				from x in (IEnumerable<Inventory.Definition>)Global.SteamClient.Inventory.Definitions
				where x.Type == "Crate"
				select x)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.CraftingButton);
				gameObject.GetComponent<Button>().onClick.AddListener(() => this.CraftModal.Open(definition.Id));
				gameObject.GetComponentInChildren<HttpImage>().Load(definition.IconUrl);
				gameObject.transform.SetParent(this.CraftingContainer.transform, false);
			}
		}

		private IEnumerator GetMarketPrice(Inventory.Definition definition)
		{
			WorkshopInventoryCraftingControls workshopInventoryCraftingControl = null;
			if (workshopInventoryCraftingControl.priceCache.ContainsKey(definition.Id))
			{
				if (workshopInventoryCraftingControl.priceCache[definition.Id] != null && workshopInventoryCraftingControl.priceCache[definition.Id].success)
				{
					workshopInventoryCraftingControl.InfoText.text = string.Concat(workshopInventoryCraftingControl.InfoText.text, string.Format("\n<b>{0}</b> on market, lowest price is <b>{1} USD</b>", workshopInventoryCraftingControl.priceCache[definition.Id].volume, workshopInventoryCraftingControl.priceCache[definition.Id].lowest_price));
				}
				yield break;
			}
			WWW wWW = new WWW(string.Concat("http://steamcommunity.com/market/priceoverview/?appid=252490&currency=1&market_hash_name=", definition.Name.Replace(" ", "%20").Replace("&", "%26")));
			yield return wWW;
			if (wWW.error != null)
			{
				UnityEngine.Debug.LogWarning(wWW.error);
				workshopInventoryCraftingControl.priceCache.Add(definition.Id, null);
				wWW.Dispose();
				yield break;
			}
			WorkshopInventoryCraftingControls.MarketPrice marketPrice = JsonConvert.DeserializeObject<WorkshopInventoryCraftingControls.MarketPrice>(wWW.text);
			workshopInventoryCraftingControl.priceCache.Add(definition.Id, marketPrice);
			if (workshopInventoryCraftingControl.priceCache[definition.Id] != null && workshopInventoryCraftingControl.priceCache[definition.Id].success)
			{
				workshopInventoryCraftingControl.InfoText.text = string.Concat(workshopInventoryCraftingControl.InfoText.text, string.Format("\n<b>{0}</b> on market, lowest price is <b>{1} USD</b>", workshopInventoryCraftingControl.priceCache[definition.Id].volume, workshopInventoryCraftingControl.priceCache[definition.Id].lowest_price));
			}
			wWW.Dispose();
		}

		private void OnChanged(WorkshopInventoryItem[] items)
		{
			this.ResultItem = null;
			this.ConvertToItem.gameObject.SetActive(false);
			this.InfoText.text = "";
			if (items == null)
			{
				return;
			}
			IEnumerable<IGrouping<Inventory.Definition, WorkshopInventoryItem>> groupings = (
				from x in (IEnumerable<WorkshopInventoryItem>)items
				where x.Item.Definition != null
				select x).GroupBy<WorkshopInventoryItem, Inventory.Definition>((WorkshopInventoryItem x) => {
				if (x.Item.Definition.IngredientFor == null)
				{
					return null;
				}
				return x.Item.Definition.IngredientFor.DefaultIfEmpty<Inventory.Recipe>().FirstOrDefault<Inventory.Recipe>().Result;
			});
			if (groupings.Count<IGrouping<Inventory.Definition, WorkshopInventoryItem>>() != 1)
			{
				this.InfoText.text = "Multiple item types selected";
				return;
			}
			this.ResultItem = groupings.First<IGrouping<Inventory.Definition, WorkshopInventoryItem>>().Key;
			if (this.ResultItem != null)
			{
				if (this.ResultItem.IsGenerator)
				{
					if ((int)items.Length != 1)
					{
						this.InfoText.text = "Can only open one at a time";
						return;
					}
					this.UpdateInfoText((int)items.Length, items.First<WorkshopInventoryItem>().Item.Definition);
					this.ConvertToItem.GetComponentInChildren<Text>().text = string.Concat("Open ", items[0].Item.Definition.Name);
					this.ConvertToItem.gameObject.SetActive(true);
					return;
				}
				Inventory.Definition definition = items.First<WorkshopInventoryItem>().Item.Definition;
				if (!items.All<WorkshopInventoryItem>((WorkshopInventoryItem x) => x.Item.Definition == definition))
				{
					definition = null;
				}
				this.UpdateInfoText((int)items.Length, definition);
				this.ConvertToItem.GetComponentInChildren<Text>().text = string.Concat("Break Into ", this.ResultItem.Name);
				this.ConvertToItem.gameObject.SetActive(true);
			}
		}

		public void OnEnable()
		{
			this.FillCraftingContainer();
		}

		public void OnManualRefresh()
		{
			this.CraftingResult.Clear();
			this.ClothAmount.text = "-";
			this.MetalAmount.text = "-";
			this.WoodAmount.text = "-";
		}

		public void OnRefreshed(Inventory.Item[] items)
		{
			Text clothAmount = this.ClothAmount;
			int num = (
				from x in (IEnumerable<Inventory.Item>)items
				where x.DefinitionId == 10031
				select x).Sum<Inventory.Item>((Inventory.Item x) => x.Quantity);
			clothAmount.text = num.ToString();
			Text metalAmount = this.MetalAmount;
			num = (
				from x in (IEnumerable<Inventory.Item>)items
				where x.DefinitionId == 14182
				select x).Sum<Inventory.Item>((Inventory.Item x) => x.Quantity);
			metalAmount.text = num.ToString();
			Text woodAmount = this.WoodAmount;
			num = (
				from x in (IEnumerable<Inventory.Item>)items
				where x.DefinitionId == 14183
				select x).Sum<Inventory.Item>((Inventory.Item x) => x.Quantity);
			woodAmount.text = num.ToString();
		}

		public void Update()
		{
			this.UpdateCraft();
			WorkshopInventoryItem[] array = (
				from x in (IEnumerable<Toggle>)this.Container.GetComponentsInChildren<Toggle>()
				where x.isOn
				select x.GetComponent<WorkshopInventoryItem>()).ToArray<WorkshopInventoryItem>();
			if (this.SelectedCount == (int)array.Length)
			{
				return;
			}
			this.SelectedCount = (int)array.Length;
			if (this.SelectedCount != 0 && this.CraftingResult.Count <= 0)
			{
				this.OnChanged(array);
				return;
			}
			this.OnChanged(null);
		}

		private void UpdateCraft()
		{
			this.CraftingResult.RemoveAll((Inventory.Result x) => !x.IsPending);
		}

		private void UpdateInfoText(int length, Inventory.Definition definition)
		{
			string str = string.Format("<b>{0}</b> items selected", length);
			if (definition != null && definition.PriceDollars > 0)
			{
				double priceDollars = definition.PriceDollars;
				str = string.Concat(str, "\nIn Item Store for <b>$", priceDollars.ToString("0.00"), " USD</b>");
			}
			this.InfoText.text = str;
			if (definition != null && definition.Marketable)
			{
				if (this.MarketCoroutine != null)
				{
					base.StopCoroutine(this.MarketCoroutine);
					this.MarketCoroutine = null;
				}
				this.MarketCoroutine = base.StartCoroutine(this.GetMarketPrice(definition));
			}
		}

		internal class MarketPrice
		{
			public string lowest_price
			{
				get;
				set;
			}

			public string median_price
			{
				get;
				set;
			}

			public bool success
			{
				get;
				set;
			}

			public ulong volume
			{
				get;
				set;
			}

			public MarketPrice()
			{
			}
		}
	}
}