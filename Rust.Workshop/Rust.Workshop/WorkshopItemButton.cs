using Rust.UI;
using Steamworks.Ugc;
using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Rust.Workshop
{
	internal class WorkshopItemButton : MonoBehaviour
	{
		public TextMeshProUGUI Name;

		public TextMeshProUGUI Description;

		public HttpImage Icon;

		public GameObject OldIndicator;

		public GameObject Incompatible;

		public GameObject ItemDownloaded;

		public GameObject ItemDownloadPending;

		public GameObject ItemDownloading;

		private Steamworks.Ugc.Item? Item;

		public WorkshopItemButton()
		{
		}

		internal void Init(Steamworks.Ugc.Item item)
		{
			string previewImageUrl;
			bool flag;
			string description;
			string title;
			this.Item = new Steamworks.Ugc.Item?(item);
			if (this.Name)
			{
				TextMeshProUGUI name = this.Name;
				ref Nullable nullablePointer = ref this.Item;
				if (nullablePointer.HasValue)
				{
					title = nullablePointer.GetValueOrDefault().Title;
				}
				else
				{
					title = null;
				}
				name.text = title;
			}
			if (this.Description)
			{
				TextMeshProUGUI textMeshProUGUI = this.Description;
				ref Nullable nullablePointer1 = ref this.Item;
				if (nullablePointer1.HasValue)
				{
					description = nullablePointer1.GetValueOrDefault().Description;
				}
				else
				{
					description = null;
				}
				textMeshProUGUI.text = description;
			}
			if (this.OldIndicator != null)
			{
				this.OldIndicator.SetActive(item.Tags.Contains<string>("version2"));
			}
			if (this.Incompatible != null)
			{
				flag = (item.Tags.Contains<string>("version3") ? false : !item.Tags.Contains<string>("version2"));
				this.Incompatible.SetActive(flag);
			}
			if (this.Icon != null)
			{
				HttpImage icon = this.Icon;
				ref Nullable nullablePointer2 = ref this.Item;
				if (nullablePointer2.HasValue)
				{
					previewImageUrl = nullablePointer2.GetValueOrDefault().PreviewImageUrl;
				}
				else
				{
					previewImageUrl = null;
				}
				icon.Load(previewImageUrl);
			}
			this.Update();
		}

		public void OpenWebpage()
		{
			if (!this.Item.HasValue)
			{
				return;
			}
			UnityEngine.Application.OpenURL(this.Item.Value.Url);
		}

		public void StartEditing()
		{
			if (!this.Item.HasValue)
			{
				return;
			}
			base.SendMessageUpwards("StartEditingItem", this.Item.Value, SendMessageOptions.RequireReceiver);
		}

		public void StartViewing()
		{
			if (!this.Item.HasValue)
			{
				return;
			}
			base.SendMessageUpwards("StartViewingItem", this.Item.Value, SendMessageOptions.RequireReceiver);
		}

		public void Update()
		{
			Steamworks.Ugc.Item value;
			if (!this.Item.HasValue)
			{
				return;
			}
			if (this.ItemDownloaded)
			{
				GameObject itemDownloaded = this.ItemDownloaded;
				value = this.Item.Value;
				itemDownloaded.SetActive(value.IsInstalled);
			}
			if (this.ItemDownloadPending)
			{
				GameObject itemDownloadPending = this.ItemDownloadPending;
				value = this.Item.Value;
				itemDownloadPending.SetActive(value.IsDownloadPending);
			}
			if (this.ItemDownloading)
			{
				GameObject itemDownloading = this.ItemDownloading;
				value = this.Item.Value;
				itemDownloading.SetActive(value.IsDownloading);
			}
		}
	}
}