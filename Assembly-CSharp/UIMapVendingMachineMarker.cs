using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMapVendingMachineMarker : MonoBehaviour
{
	public Color inStock;

	public Color outOfStock;

	public Image colorBackground;

	public string displayName;

	public Tooltip toolTip;

	private bool isInStock;

	public UIMapVendingMachineMarker()
	{
	}

	public void SetOutOfStock(bool stock)
	{
		this.colorBackground.color = (stock ? this.inStock : this.outOfStock);
		this.isInStock = stock;
	}

	public void UpdateDisplayName(string newName, ProtoBuf.VendingMachine.SellOrderContainer sellOrderContainer)
	{
		this.displayName = newName;
		this.toolTip.Text = this.displayName;
		if (this.isInStock && sellOrderContainer != null && sellOrderContainer.sellOrders != null && sellOrderContainer.sellOrders.Count > 0)
		{
			Tooltip tooltip = this.toolTip;
			tooltip.Text = string.Concat(tooltip.Text, "\n");
			foreach (ProtoBuf.VendingMachine.SellOrder sellOrder in sellOrderContainer.sellOrders)
			{
				if (sellOrder.inStock <= 0)
				{
					continue;
				}
				string str = string.Concat(ItemManager.FindItemDefinition(sellOrder.itemToSellID).displayName.translated, (sellOrder.itemToSellIsBP ? " (BP)" : ""));
				string str1 = string.Concat(ItemManager.FindItemDefinition(sellOrder.currencyID).displayName.translated, (sellOrder.currencyIsBP ? " (BP)" : ""));
				Tooltip tooltip1 = this.toolTip;
				tooltip1.Text = string.Concat(new object[] { tooltip1.Text, "\n", sellOrder.itemToSellAmount, " ", str, " | ", sellOrder.currencyAmountPerItem, " ", str1 });
				tooltip1 = this.toolTip;
				tooltip1.Text = string.Concat(new object[] { tooltip1.Text, " (", sellOrder.inStock, " Left)" });
			}
		}
		this.toolTip.enabled = this.toolTip.Text != "";
	}
}