using Rust.UI;
using Steamworks;
using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class ItemStoreItemInfoModal : MonoBehaviour
{
	public HttpImage Icon;

	public TextMeshProUGUI Name;

	public TextMeshProUGUI Price;

	public TextMeshProUGUI Description;

	private InventoryDef item;

	public ItemStoreItemInfoModal()
	{
	}

	public void AddToCart()
	{
		SingletonComponent<ItemStore>.Instance.AddToCart(this.item);
		this.Hide();
	}

	public void Hide()
	{
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 0f, 0.2f).setOnComplete(() => base.gameObject.SetActive(false));
	}

	public void Show(InventoryDef item)
	{
		this.item = item;
		this.Icon.Load(item.IconUrlLarge);
		this.Name.text = item.Name;
		this.Description.text = item.Description.BBCodeToUnity();
		this.Price.text = item.LocalPriceFormatted;
		base.gameObject.SetActive(true);
		base.GetComponent<CanvasGroup>().alpha = 0f;
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 1f, 0.1f);
	}
}