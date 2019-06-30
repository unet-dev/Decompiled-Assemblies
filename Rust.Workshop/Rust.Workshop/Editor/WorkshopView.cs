using Facepunch.Extend;
using Rust.Workshop;
using Steamworks;
using Steamworks.Ugc;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rust.Workshop.Editor
{
	public class WorkshopView : MonoBehaviour
	{
		public TextMeshProUGUI Title;

		public TextMeshProUGUI AuthorName;

		public TextMeshProUGUI VoteInfo;

		public Button VoteUp;

		public Button VoteDown;

		private Item? item;

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
			if (!this.item.HasValue)
			{
				return;
			}
			ref Nullable nullablePointer = ref this.item;
			if (nullablePointer.HasValue)
			{
				nullablePointer.GetValueOrDefault().Vote(false);
			}
			this.UpdateFrom(this.item.Value);
		}

		public void OnVoteUp()
		{
			if (!this.item.HasValue)
			{
				return;
			}
			ref Nullable nullablePointer = ref this.item;
			if (nullablePointer.HasValue)
			{
				nullablePointer.GetValueOrDefault().Vote(true);
			}
			this.UpdateFrom(this.item.Value);
		}

		public void OpenWeb()
		{
			string url;
			if (!this.item.HasValue)
			{
				return;
			}
			ref Nullable nullablePointer = ref this.item;
			if (nullablePointer.HasValue)
			{
				url = nullablePointer.GetValueOrDefault().Url;
			}
			else
			{
				url = null;
			}
			UnityEngine.Application.OpenURL(url);
		}

		public void Update()
		{
			if (!this.item.HasValue)
			{
				return;
			}
			if (!string.IsNullOrEmpty(this.AuthorName.text))
			{
				return;
			}
			TextMeshProUGUI authorName = this.AuthorName;
			Friend owner = this.item.Value.Owner;
			authorName.text = owner.Name.Truncate(32, null).ToUpper();
		}

		public void UpdateFrom(Item item)
		{
			this.item = new Item?(item);
			this.Title.text = item.Title.Truncate(24, null).ToUpper();
			TextMeshProUGUI authorName = this.AuthorName;
			Friend owner = item.Owner;
			authorName.text = owner.Name.Truncate(32, null).ToUpper();
		}
	}
}