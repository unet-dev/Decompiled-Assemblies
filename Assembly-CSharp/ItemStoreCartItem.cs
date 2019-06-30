using Steamworks;
using System;
using TMPro;
using UnityEngine;

public class ItemStoreCartItem : MonoBehaviour
{
	public int Index;

	public TextMeshProUGUI Name;

	public TextMeshProUGUI Price;

	public ItemStoreCartItem()
	{
	}

	public void Init(int index, InventoryDef def)
	{
		this.Index = index;
		this.Name.text = def.Name;
		this.Price.text = def.LocalPriceFormatted;
	}

	public void RemoveFromCart()
	{
		SingletonComponent<ItemStore>.Instance.RemoveFromCart(this.Index);
	}
}