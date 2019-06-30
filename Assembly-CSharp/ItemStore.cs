using Facepunch;
using GameAnalyticsSDK;
using Steamworks;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ItemStore : SingletonComponent<ItemStore>, VirtualScroll.IDataSource
{
	public GameObject ItemPrefab;

	public RectTransform ItemParent;

	public List<InventoryDef> Cart = new List<InventoryDef>();

	public ItemStoreItemInfoModal ItemStoreInfoModal;

	public GameObject BuyingModal;

	public ItemStoreBuyFailedModal ItemStoreBuyFailedModal;

	public ItemStoreBuySuccessModal ItemStoreBuySuccessModal;

	public SoundDefinition AddToCartSound;

	public TextMeshProUGUI TotalValue;

	private Task refreshing;

	public ItemStore()
	{
	}

	private void AddItem(InventoryDef item)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.ItemPrefab);
		gameObject.transform.SetParent(this.ItemParent, false);
		gameObject.GetComponent<ItemStoreItem>().Init(item);
	}

	internal void AddToCart(InventoryDef item)
	{
		this.Cart.Add(item);
		this.AddToCartSound.Play();
		this.UpdateShoppingList();
		GameAnalytics.NewDesignEvent("itemstore:addtocart");
	}

	protected override void Awake()
	{
		base.Awake();
		GameAnalytics.NewDesignEvent("itemstore:open");
		this.refreshing = this.RefreshList();
		SteamUser.OnMicroTxnAuthorizationResponse += new Action<AppId, ulong, bool>(this.OnPurchaseFinished);
	}

	public void Checkout()
	{
		this.CheckoutAsync();
	}

	public async Task CheckoutAsync()
	{
		if (this.Cart.Count != 0)
		{
			GameAnalytics.NewDesignEvent("itemstore:startcheckout");
			this.BuyingModal.SetActive(true);
			InventoryPurchaseResult? nullable = await Steamworks.SteamInventory.StartPurchaseAsync(this.Cart.ToArray());
			if (!nullable.HasValue)
			{
				UnityEngine.Debug.Log("RESULT IS NULL");
			}
			UnityEngine.Debug.Log(string.Format("Result: {0}", nullable.Value.Result));
			UnityEngine.Debug.Log(string.Format("TransID: {0}", nullable.Value.TransID));
			UnityEngine.Debug.Log(string.Format("OrderID: {0}", nullable.Value.OrderID));
			Result result = nullable.Value.Result;
		}
	}

	public void EmptyCart()
	{
		this.Cart.Clear();
		this.UpdateShoppingList();
	}

	public int GetItemCount()
	{
		return this.Cart.Count;
	}

	private void OnPurchaseFinished(AppId appid, ulong orderid, bool success)
	{
		UnityEngine.Debug.Log(string.Format("OnPurchaseFinished: appid {0}", appid));
		UnityEngine.Debug.Log(string.Format("OnPurchaseFinished: orderid {0}", orderid));
		UnityEngine.Debug.Log(string.Format("OnPurchaseFinished: success {0}", success));
		this.BuyingModal.SetActive(false);
		if (!success)
		{
			GameAnalytics.NewDesignEvent("itemstore:buyfailed");
			this.ItemStoreBuyFailedModal.Show(orderid);
			return;
		}
		GameAnalytics.NewDesignEvent("itemstore:buysuccess");
		GameAnalytics.NewBusinessEvent(Steamworks.SteamInventory.Currency, this.Cart.Sum<InventoryDef>((InventoryDef x) => x.LocalPrice), "skins", "skins", "skins");
		this.ItemStoreBuySuccessModal.Show(orderid);
		this.EmptyCart();
	}

	private async Task RefreshList()
	{
		InventoryDef[] definitionsWithPricesAsync = await Steamworks.SteamInventory.GetDefinitionsWithPricesAsync();
		if (definitionsWithPricesAsync != null)
		{
			this.UpdateShoppingList();
			InventoryDef[] inventoryDefArray = definitionsWithPricesAsync;
			for (int i = 0; i < (int)inventoryDefArray.Length; i++)
			{
				this.AddItem(inventoryDefArray[i]);
			}
			InventoryDef[] inventoryDefArray1 = definitionsWithPricesAsync;
			PlayerPrefs.SetInt("sawSaleItems", ((IEnumerable<InventoryDef>)inventoryDefArray1).Sum<InventoryDef>((InventoryDef x) => x.Id));
		}
	}

	public void RemoveFromCart(int index)
	{
		this.Cart.RemoveAt(index);
		this.UpdateShoppingList();
	}

	public void SetItemData(int i, GameObject obj)
	{
		obj.GetComponent<ItemStoreCartItem>().Init(i, this.Cart[i]);
	}

	internal void ShowModal(InventoryDef item)
	{
		this.ItemStoreInfoModal.Show(item);
	}

	private void UpdateShoppingList()
	{
		int num = this.Cart.Sum<InventoryDef>((InventoryDef x) => x.LocalPrice);
		this.TotalValue.text = Utility.FormatPrice(Steamworks.SteamInventory.Currency, (double)num / 100);
		base.GetComponentInChildren<VirtualScroll>().FullRebuild();
	}
}