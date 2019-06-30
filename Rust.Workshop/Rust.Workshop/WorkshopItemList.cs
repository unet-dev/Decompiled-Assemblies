using Steamworks;
using Steamworks.Ugc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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

		private const int PerPage = 50;

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
				return Mathf.FloorToInt((float)(this.TotalResults / 50)) + 1;
			}
		}

		static WorkshopItemList()
		{
			WorkshopItemList.StaticRefresh = 10;
		}

		public WorkshopItemList()
		{
		}

		private void Awake()
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
				if (Skinnable.All != null)
				{
					this.ItemTypeSelector.AddOptions((
						from x in (IEnumerable<Skinnable>)Skinnable.All
						select x.Name into x
						orderby x
						select x).ToList<string>());
				}
			}
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

		private void PageNext()
		{
			if (this.Refreshing)
			{
				return;
			}
			this.Page++;
			this.Refresh();
		}

		private void PagePrev()
		{
			if (this.Refreshing)
			{
				return;
			}
			this.Page--;
			this.Refresh();
		}

		public async Task Refresh()
		{
			if (!this.Refreshing)
			{
				this.Refreshing = true;
				this.Page = Mathf.Clamp(this.Page, 1, this.NumPages);
				while (this.Container.transform.childCount > 0)
				{
					UnityEngine.Object.DestroyImmediate(this.Container.transform.GetChild(0).gameObject);
				}
				Query query = Query.All.MatchAllTags();
				if (!string.IsNullOrEmpty(this.ItemFilter))
				{
					query = query.WithTag(this.ItemFilter);
				}
				switch (this.ListType)
				{
					case Rust.Workshop.ListType.MyItems:
					{
						query = query.WhereUserPublished(new SteamId());
						break;
					}
					case Rust.Workshop.ListType.MostRecent:
					{
						query = query.RankedByPublicationDate();
						query = query.WithTag("Version3");
						break;
					}
					case Rust.Workshop.ListType.MostPopular:
					{
						query = query.RankedByTrend();
						query = query.WithTrendDays(30);
						query = query.WithTag("Version3");
						break;
					}
					case Rust.Workshop.ListType.Trending:
					{
						query = query.RankedByTrend();
						query = query.WithTrendDays(7);
						query = query.WithTag("Version3");
						break;
					}
					case Rust.Workshop.ListType.Accepted:
					{
						query = query.RankedByAcceptanceDate();
						query = query.WithTag("Version3");
						break;
					}
				}
				if (this.PageInfo != null)
				{
					this.PageInfo.text = "UPDATING";
				}
				ResultPage? pageAsync = await query.GetPageAsync(this.Page);
				if (pageAsync.HasValue)
				{
					this.TotalResults = pageAsync.Value.TotalCount;
					Item[] array = pageAsync.Value.Entries.ToArray<Item>();
					if (this.ListType == Rust.Workshop.ListType.MyItems)
					{
						Item[] itemArray = array;
						array = (
							from x in (IEnumerable<Item>)itemArray
							orderby x.Updated descending
							select x).ToArray<Item>();
					}
					Item[] itemArray1 = array;
					for (int i = 0; i < (int)itemArray1.Length; i++)
					{
						Item item = itemArray1[i];
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.ButtonPrefab.gameObject);
						gameObject.transform.SetParent(this.Container.transform, false);
						gameObject.GetComponent<WorkshopItemButton>().Init(item);
					}
					if (this.PageInfo != null)
					{
						this.PageInfo.text = string.Format("Page {0} of {1}", this.Page, this.NumPages);
					}
				}
				this.Refreshing = false;
			}
		}

		public static void RefreshAll()
		{
			WorkshopItemList.StaticRefresh++;
		}

		public void SwitchToAccepted()
		{
			if (this.ListType == Rust.Workshop.ListType.Accepted)
			{
				return;
			}
			this.ListType = Rust.Workshop.ListType.Accepted;
			this.Page = 1;
			this.ForcedRefresh++;
		}

		public void SwitchToLatest()
		{
			if (this.ListType == Rust.Workshop.ListType.MostRecent)
			{
				return;
			}
			this.ListType = Rust.Workshop.ListType.MostRecent;
			this.Page = 1;
			this.ForcedRefresh++;
		}

		public void SwitchToPopular()
		{
			if (this.ListType == Rust.Workshop.ListType.MostPopular)
			{
				return;
			}
			this.ListType = Rust.Workshop.ListType.MostPopular;
			this.Page = 1;
			this.ForcedRefresh++;
		}

		public void SwitchToTrending()
		{
			if (this.ListType == Rust.Workshop.ListType.Trending)
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
				this.Refresh();
			}
		}
	}
}