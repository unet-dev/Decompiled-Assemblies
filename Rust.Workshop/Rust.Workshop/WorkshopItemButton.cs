using Facepunch.Steamworks;
using Rust.UI;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Rust.Workshop
{
	internal class WorkshopItemButton : MonoBehaviour
	{
		public Text Name;

		public Text Description;

		public HttpImage Icon;

		public GameObject OldIndicator;

		public GameObject Incompatible;

		public GameObject ItemDownloaded;

		public GameObject ItemDownloadPending;

		public GameObject ItemDownloading;

		private Facepunch.Steamworks.Workshop.Item Item;

		protected WorkshopInterface Interface
		{
			get
			{
				return base.GetComponentInParent<WorkshopInterface>();
			}
		}

		public WorkshopItemButton()
		{
		}

		internal void Init(Facepunch.Steamworks.Workshop.Item item)
		{
			bool flag;
			this.Item = item;
			if (this.Name)
			{
				this.Name.text = this.Item.Title;
			}
			if (this.Description)
			{
				this.Description.text = this.Item.Description;
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
				this.Icon.Load(item.PreviewImageUrl);
			}
			this.Update();
		}

		public void OpenWebpage()
		{
			UnityEngine.Application.OpenURL(this.Item.Url);
		}

		public void StartEditing()
		{
			this.Interface.StartEditing(this.Item);
		}

		public void StartViewing()
		{
			this.Interface.StartViewing(this.Item);
		}

		public void Update()
		{
			if (this.Item == null)
			{
				return;
			}
			if (this.ItemDownloaded)
			{
				this.ItemDownloaded.SetActive(this.Item.Installed);
			}
			if (this.ItemDownloadPending)
			{
				this.ItemDownloadPending.SetActive(this.Item.Downloading);
			}
			if (this.ItemDownloading)
			{
				this.ItemDownloading.SetActive(this.Item.Downloading);
			}
		}
	}
}