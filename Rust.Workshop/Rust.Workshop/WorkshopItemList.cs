using Facepunch.Steamworks;
using Rust;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Rust.Workshop
{
	internal class WorkshopItemList : MonoBehaviour
	{
		private static int StaticRefresh;

		public WorkshopItemButton ButtonPrefab;

		public GameObject Container;

		public Rust.Workshop.ListType ListType;

		public int PerPage = 40;

		public int Page = 1;

		public Button PreviousPage;

		public Button NextPage;

		public Text PageInfo;

		public string ItemFilter;

		public Dropdown ItemTypeSelector;

		private int ForcedRefresh;

		private int TotalResults;

		private bool Refreshing;

		private int NumPages
		{
			get
			{
				return Mathf.FloorToInt((float)(this.TotalResults / this.PerPage)) + 1;
			}
		}

		static WorkshopItemList()
		{
			WorkshopItemList.StaticRefresh = 1;
		}

		public WorkshopItemList()
		{
		}

		public void OnChangedItemType(int type)
		{
			Dropdown.OptionData item = this.ItemTypeSelector.options[type];
			if (item.text != "All")
			{
				this.ItemFilter = item.text;
			}
			else
			{
				this.ItemFilter = null;
			}
			this.ForcedRefresh++;
		}

		private void OnDisable()
		{
			if (Rust.Application.isQuitting)
			{
				return;
			}
			if (this.PreviousPage)
			{
				this.PreviousPage.onClick.RemoveListener(new UnityAction(this.PagePrev));
			}
			if (this.NextPage)
			{
				this.NextPage.onClick.RemoveListener(new UnityAction(this.PageNext));
			}
		}

		private void OnEnable()
		{
			if (this.PreviousPage)
			{
				this.PreviousPage.onClick.AddListener(new UnityAction(this.PagePrev));
			}
			if (this.NextPage)
			{
				this.NextPage.onClick.AddListener(new UnityAction(this.PageNext));
			}
			if (this.ItemTypeSelector)
			{
				this.ItemTypeSelector.ClearOptions();
				this.ItemTypeSelector.AddOptions(new List<string>()
				{
					"All"
				});
				this.ItemTypeSelector.AddOptions((
					from x in (IEnumerable<Skinnable>)Skinnable.All
					select x.Name into x
					orderby x
					select x).ToList<string>());
			}
		}

		private void PageNext()
		{
			if (this.Refreshing)
			{
				return;
			}
			this.Page++;
			base.StartCoroutine(this.Refresh());
		}

		private void PagePrev()
		{
			if (this.Refreshing)
			{
				return;
			}
			this.Page--;
			base.StartCoroutine(this.Refresh());
		}

		public IEnumerator Refresh()
		{
			WorkshopItemList totalResults = null;
			if (totalResults.Refreshing)
			{
				yield break;
			}
			totalResults.Refreshing = true;
			totalResults.Page = Mathf.Clamp(totalResults.Page, 1, totalResults.NumPages);
			while (totalResults.Container.transform.childCount > 0)
			{
				UnityEngine.Object.DestroyImmediate(totalResults.Container.transform.GetChild(0).gameObject);
			}
			Facepunch.Steamworks.Workshop.Query perPage = Global.SteamClient.Workshop.CreateQuery();
			perPage.PerPage = totalResults.PerPage;
			perPage.Page = totalResults.Page;
			perPage.RequireAllTags = true;
			if (!string.IsNullOrEmpty(totalResults.ItemFilter))
			{
				perPage.RequireTags.Add(totalResults.ItemFilter);
			}
			switch (totalResults.ListType)
			{
				case Rust.Workshop.ListType.MyItems:
				{
					perPage.UserId = new ulong?(Global.SteamClient.SteamId);
					break;
				}
				case Rust.Workshop.ListType.MostRecent:
				{
					perPage.Order = Facepunch.Steamworks.Workshop.Order.RankedByPublicationDate;
					perPage.RequireTags.Add("Version3");
					break;
				}
				case Rust.Workshop.ListType.MostPopular:
				{
					perPage.Order = Facepunch.Steamworks.Workshop.Order.RankedByTrend;
					perPage.RankedByTrendDays = 30;
					perPage.RequireTags.Add("Version3");
					break;
				}
				case Rust.Workshop.ListType.Trending:
				{
					perPage.Order = Facepunch.Steamworks.Workshop.Order.RankedByTrend;
					perPage.RankedByTrendDays = 1;
					perPage.RequireTags.Add("Version3");
					break;
				}
				case Rust.Workshop.ListType.Accepted:
				{
					perPage.Order = Facepunch.Steamworks.Workshop.Order.AcceptedForGameRankedByAcceptanceDate;
					perPage.RequireTags.Add("Version3");
					break;
				}
			}
			if (totalResults.PageInfo != null)
			{
				totalResults.PageInfo.text = "UPDATING";
			}
			perPage.Run();
			yield return new WaitWhile(() => perPage.IsRunning);
			totalResults.TotalResults = perPage.TotalResults;
			if (totalResults.ListType == Rust.Workshop.ListType.MyItems)
			{
				Facepunch.Steamworks.Workshop.Query array = perPage;
				Facepunch.Steamworks.Workshop.Item[] items = perPage.Items;
				array.Items = (
					from x in (IEnumerable<Facepunch.Steamworks.Workshop.Item>)items
					orderby x.Modified descending
					select x).ToArray<Facepunch.Steamworks.Workshop.Item>();
			}
			Facepunch.Steamworks.Workshop.Item[] itemArray = perPage.Items;
			for (int i = 0; i < (int)itemArray.Length; i++)
			{
				Facepunch.Steamworks.Workshop.Item item = itemArray[i];
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(totalResults.ButtonPrefab.gameObject);
				gameObject.transform.SetParent(totalResults.Container.transform, false);
				gameObject.GetComponent<WorkshopItemButton>().Init(item);
			}
			perPage.Dispose();
			if (totalResults.PageInfo != null)
			{
				totalResults.PageInfo.text = string.Format("Page {0} of {1}", totalResults.Page, totalResults.NumPages);
			}
			totalResults.Refreshing = false;
		}

		public static void RefreshAll()
		{
			WorkshopItemList.StaticRefresh++;
		}

		public void SwitchToAccepted(bool b)
		{
			if (!b)
			{
				return;
			}
			this.ListType = Rust.Workshop.ListType.Accepted;
			this.Page = 1;
			this.ForcedRefresh++;
		}

		public void SwitchToLatest(bool b)
		{
			if (!b)
			{
				return;
			}
			this.ListType = Rust.Workshop.ListType.MostRecent;
			this.Page = 1;
			this.ForcedRefresh++;
		}

		public void SwitchToPopular(bool b)
		{
			if (!b)
			{
				return;
			}
			this.ListType = Rust.Workshop.ListType.MostPopular;
			this.Page = 1;
			this.ForcedRefresh++;
		}

		public void SwitchToTrending(bool b)
		{
			if (!b)
			{
				return;
			}
			this.ListType = Rust.Workshop.ListType.Trending;
			this.Page = 1;
			this.ForcedRefresh++;
		}

		private void Update()
		{
			if (this.Refreshing)
			{
				return;
			}
			if (this.ForcedRefresh != WorkshopItemList.StaticRefresh)
			{
				this.ForcedRefresh = WorkshopItemList.StaticRefresh;
				base.StartCoroutine(this.Refresh());
			}
		}
	}
}