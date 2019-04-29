using Facepunch.Steamworks;
using Rust;
using Rust.UI;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace Rust.Workshop
{
	internal class WorkshopItemVoteGame : MonoBehaviour
	{
		public Button VoteButton;

		private Facepunch.Steamworks.Workshop.Item[] Items;

		private Facepunch.Steamworks.Workshop.Item CurrentItem;

		public WorkshopItemVoteGame()
		{
		}

		public void Awake()
		{
			this.UpdateList();
		}

		private void OnVoted()
		{
			if ((int)this.Items.Length < 50)
			{
				this.UpdateList();
				return;
			}
			this.Refresh();
		}

		public void OnVoteNo()
		{
			if (this.CurrentItem == null)
			{
				return;
			}
			this.CurrentItem.VoteDown();
			this.OnVoted();
		}

		public void OnVoteYes()
		{
			if (this.CurrentItem == null)
			{
				return;
			}
			this.CurrentItem.VoteUp();
			this.OnVoted();
		}

		private void OnWorkshopQuery(Facepunch.Steamworks.Workshop.Query query)
		{
			if (query.Items == null)
			{
				return;
			}
			if (this.Items == null)
			{
				this.Items = new Facepunch.Steamworks.Workshop.Item[0];
			}
			this.Items = this.Items.Union<Facepunch.Steamworks.Workshop.Item>(query.Items).Distinct<Facepunch.Steamworks.Workshop.Item>().ToArray<Facepunch.Steamworks.Workshop.Item>();
			if (this.CurrentItem == null)
			{
				this.Refresh();
			}
		}

		public void OpenItem()
		{
			Global.SteamClient.Overlay.OpenUrl(this.CurrentItem.Url);
		}

		public void Refresh()
		{
			if (this.Items == null)
			{
				return;
			}
			if ((int)this.Items.Length > 2)
			{
				this.CurrentItem = this.Items[UnityEngine.Random.Range(0, (int)this.Items.Length - 1)];
				this.VoteButton.GetComponentInChildren<HttpImage>().Load(this.CurrentItem.PreviewImageUrl);
			}
			this.Items = (
				from x in this.Items
				where x.Id != this.CurrentItem.Id
				select x).ToArray<Facepunch.Steamworks.Workshop.Item>();
		}

		public void UpdateList()
		{
			if (Global.SteamClient == null)
			{
				return;
			}
			for (int i = 0; i < 5; i++)
			{
				Facepunch.Steamworks.Workshop.Query action = Global.SteamClient.Workshop.CreateQuery();
				action.Order = Facepunch.Steamworks.Workshop.Order.RankedByPublicationDate;
				action.OnResult = new Action<Facepunch.Steamworks.Workshop.Query>(this.OnWorkshopQuery);
				action.Page = i + 1;
				action.Run();
			}
			for (int j = 0; j < 5; j++)
			{
				Facepunch.Steamworks.Workshop.Query query = Global.SteamClient.Workshop.CreateQuery();
				query.Order = Facepunch.Steamworks.Workshop.Order.RankedByTrend;
				query.OnResult = new Action<Facepunch.Steamworks.Workshop.Query>(this.OnWorkshopQuery);
				query.Page = j + 1;
				query.Run();
			}
		}
	}
}