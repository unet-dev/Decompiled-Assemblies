using Rust.UI;
using Steamworks;
using System;
using TMPro;
using UnityEngine;

public class ItemStoreItem : MonoBehaviour
{
	public HttpImage Icon;

	public TextMeshProUGUI Name;

	public TextMeshProUGUI Price;

	private InventoryDef item;

	public ItemStoreItem()
	{
	}

	public void AddToCart()
	{
		SingletonComponent<ItemStore>.Instance.AddToCart(this.item);
	}

	internal void Init(InventoryDef item)
	{
		this.item = item;
		this.Icon.Load(item.IconUrlLarge);
		this.Name.text = item.Name;
		this.Price.text = item.LocalPriceFormatted;
	}

	public void ShowModal()
	{
		SingletonComponent<ItemStore>.Instance.ShowModal(this.item);
	}
}