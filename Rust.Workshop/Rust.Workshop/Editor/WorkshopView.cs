using Facepunch.Extend;
using Facepunch.Steamworks;
using Rust.Workshop;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Rust.Workshop.Editor
{
	public class WorkshopView : MonoBehaviour
	{
		public Text Title;

		public Text AuthorName;

		public Text VoteInfo;

		public Button VoteUp;

		public Button VoteDown;

		private Facepunch.Steamworks.Workshop.Item item;

		protected WorkshopItemEditor Editor
		{
			get
			{
				return this.Interface.Editor;
			}
		}

		protected WorkshopInterface Interface
		{
			get
			{
				return base.GetComponentInParent<WorkshopInterface>();
			}
		}

		public WorkshopView()
		{
		}

		internal void Clear()
		{
			this.item = null;
			this.Title.text = "";
			this.AuthorName.text = "";
		}

		public void OnVoteDown()
		{
			if (this.item == null)
			{
				return;
			}
			this.item.VoteDown();
			this.UpdateFrom(this.item);
		}

		public void OnVoteUp()
		{
			if (this.item == null)
			{
				return;
			}
			this.item.VoteUp();
			this.UpdateFrom(this.item);
		}

		public void OpenWeb()
		{
			if (this.item == null)
			{
				return;
			}
			UnityEngine.Application.OpenURL(this.item.Url);
		}

		public void Update()
		{
			if (this.item == null)
			{
				return;
			}
			if (!string.IsNullOrEmpty(this.AuthorName.text))
			{
				return;
			}
			this.AuthorName.text = this.item.OwnerName.Truncate(32, null).ToUpper();
		}

		public void UpdateFrom(Facepunch.Steamworks.Workshop.Item item)
		{
			this.item = item;
			this.Title.text = item.Title.Truncate(24, null).ToUpper();
			this.AuthorName.text = item.OwnerName.Truncate(32, null).ToUpper();
		}
	}
}