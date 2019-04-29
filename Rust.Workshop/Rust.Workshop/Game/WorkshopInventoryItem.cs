using Facepunch.Extend;
using Facepunch.Steamworks;
using Rust.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Rust.Workshop.Game
{
	public class WorkshopInventoryItem : MonoBehaviour
	{
		public Inventory.Item Item;

		public HttpImage Image;

		public WorkshopInventoryItem()
		{
		}

		public void Setup(Inventory.Item item, bool PlayNewAnimation)
		{
			this.Item = item;
			if (item.Definition == null)
			{
				return;
			}
			base.transform.FindChildRecursive("ItemName").GetComponent<Text>().text = item.Definition.Name;
			this.Image.Load(item.Definition.IconUrl);
			if (PlayNewAnimation)
			{
				LeanTween.scale(base.gameObject, Vector3.one * 1.5f, 0.5f).setEase(TweenMode.Punch);
			}
		}
	}
}