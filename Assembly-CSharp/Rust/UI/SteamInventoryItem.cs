using Facepunch.Extend;
using Steamworks;
using System;
using TMPro;
using UnityEngine;

namespace Rust.UI
{
	public class SteamInventoryItem : MonoBehaviour
	{
		public InventoryItem Item;

		public HttpImage Image;

		public SteamInventoryItem()
		{
		}

		public bool Setup(InventoryItem item)
		{
			this.Item = item;
			if (item.Def == null)
			{
				return false;
			}
			base.transform.FindChildRecursive("ItemName").GetComponent<TextMeshProUGUI>().text = item.Def.Name;
			return this.Image.Load(item.Def.IconUrl);
		}
	}
}