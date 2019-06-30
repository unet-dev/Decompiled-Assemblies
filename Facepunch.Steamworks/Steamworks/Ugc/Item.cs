using Steamworks;
using Steamworks.Data;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Steamworks.Ugc
{
	public struct Item
	{
		internal SteamUGCDetails_t details;

		internal PublishedFileId _id;

		public string ChangelogUrl
		{
			get
			{
				return String.Format("http://steamcommunity.com/sharedfiles/filedetails/changelog/{0}", this.Id);
			}
		}

		public string CommentsUrl
		{
			get
			{
				return String.Format("http://steamcommunity.com/sharedfiles/filedetails/comments/{0}", this.Id);
			}
		}

		public AppId ConsumerApp
		{
			get
			{
				return this.details.ConsumerAppID;
			}
		}

		public DateTime Created
		{
			get
			{
				return Epoch.ToDateTime(this.details.TimeCreated);
			}
		}

		public AppId CreatorApp
		{
			get
			{
				return this.details.CreatorAppID;
			}
		}

		public string Description
		{
			get
			{
				return this.details.Description;
			}
		}

		public string Directory
		{
			get
			{
				string str;
				ulong num = (ulong)0;
				uint num1 = 0;
				StringBuilder stringBuilder = Helpers.TakeStringBuilder();
				if (SteamUGC.Internal.GetItemInstallInfo(this.Id, ref num, stringBuilder, (uint)stringBuilder.Capacity, ref num1))
				{
					str = stringBuilder.ToString();
				}
				else
				{
					str = null;
				}
				return str;
			}
		}

		public string DiscussUrl
		{
			get
			{
				return String.Format("http://steamcommunity.com/sharedfiles/filedetails/discussions/{0}", this.Id);
			}
		}

		public PublishedFileId Id
		{
			get
			{
				return this._id;
			}
		}

		public bool IsAcceptedForUse
		{
			get
			{
				return this.details.AcceptedForUse;
			}
		}

		public bool IsBanned
		{
			get
			{
				return this.details.Banned;
			}
		}

		public bool IsDownloading
		{
			get
			{
				return (this.State & ItemState.Downloading) == ItemState.Downloading;
			}
		}

		public bool IsDownloadPending
		{
			get
			{
				return (this.State & ItemState.DownloadPending) == ItemState.DownloadPending;
			}
		}

		public bool IsFriendsOnly
		{
			get
			{
				return this.details.Visibility == RemoteStoragePublishedFileVisibility.FriendsOnly;
			}
		}

		public bool IsInstalled
		{
			get
			{
				return (this.State & ItemState.Installed) == ItemState.Installed;
			}
		}

		public bool IsPrivate
		{
			get
			{
				return this.details.Visibility == RemoteStoragePublishedFileVisibility.Private;
			}
		}

		public bool IsPublic
		{
			get
			{
				return this.details.Visibility == RemoteStoragePublishedFileVisibility.Public;
			}
		}

		public bool IsSubscribed
		{
			get
			{
				return (this.State & ItemState.Subscribed) == ItemState.Subscribed;
			}
		}

		public bool NeedsUpdate
		{
			get
			{
				return (this.State & ItemState.NeedsUpdate) == ItemState.NeedsUpdate;
			}
		}

		public ulong NumComments
		{
			get;
			internal set;
		}

		public ulong NumFavorites
		{
			get;
			internal set;
		}

		public ulong NumFollowers
		{
			get;
			internal set;
		}

		public ulong NumPlaytimeSessions
		{
			get;
			internal set;
		}

		public ulong NumPlaytimeSessionsDuringTimePeriod
		{
			get;
			internal set;
		}

		public ulong NumSecondsPlayed
		{
			get;
			internal set;
		}

		public ulong NumSecondsPlayedDuringTimePeriod
		{
			get;
			internal set;
		}

		public ulong NumSubscriptions
		{
			get;
			internal set;
		}

		public ulong NumUniqueFavorites
		{
			get;
			internal set;
		}

		public ulong NumUniqueFollowers
		{
			get;
			internal set;
		}

		public ulong NumUniqueSubscriptions
		{
			get;
			internal set;
		}

		public ulong NumUniqueWebsiteViews
		{
			get;
			internal set;
		}

		public Friend Owner
		{
			get
			{
				return new Friend(this.details.SteamIDOwner);
			}
		}

		public string PreviewImageUrl
		{
			get;
			internal set;
		}

		public ulong ReportScore
		{
			get;
			internal set;
		}

		public float Score
		{
			get
			{
				return this.details.Score;
			}
		}

		private ItemState State
		{
			get
			{
				return (ItemState)SteamUGC.Internal.GetItemState(this.Id);
			}
		}

		public string StatsUrl
		{
			get
			{
				return String.Format("http://steamcommunity.com/sharedfiles/filedetails/stats/{0}", this.Id);
			}
		}

		public string[] Tags
		{
			get;
			internal set;
		}

		public string Title
		{
			get
			{
				return this.details.Title;
			}
		}

		public DateTime Updated
		{
			get
			{
				return Epoch.ToDateTime(this.details.TimeUpdated);
			}
		}

		public string Url
		{
			get
			{
				return String.Format("http://steamcommunity.com/sharedfiles/filedetails/?source=Facepunch.Steamworks&id={0}", this.Id);
			}
		}

		public Item(PublishedFileId id)
		{
			this = new Item()
			{
				_id = id
			};
		}

		public bool Download(bool highPriority = false)
		{
			return SteamUGC.Internal.DownloadItem(this.Id, highPriority);
		}

		public Editor Edit()
		{
			return new Editor(this.Id);
		}

		internal static Item From(SteamUGCDetails_t details)
		{
			Item item = new Item()
			{
				_id = details.PublishedFileId,
				details = details,
				Tags = details.Tags.ToLower().Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
			};
			return item;
		}

		public static async Task<Item?> GetAsync(PublishedFileId id, int maxageseconds = 1800)
		{
			Item? nullable;
			SteamUGCRequestUGCDetailsResult_t? nullable1 = await SteamUGC.Internal.RequestUGCDetails(id, (uint)maxageseconds);
			SteamUGCRequestUGCDetailsResult_t? nullable2 = nullable1;
			nullable1 = null;
			if (nullable2.HasValue)
			{
				nullable = new Item?(Item.From(nullable2.Value.Details));
			}
			else
			{
				nullable = null;
			}
			return nullable;
		}

		public bool HasTag(string find)
		{
			bool flag;
			flag = (this.Tags.Length != 0 ? this.Tags.Contains<string>(find, StringComparer.OrdinalIgnoreCase) : false);
			return flag;
		}

		public async Task<bool> Vote(bool up)
		{
			bool flag;
			SetUserItemVoteResult_t? nullable = await SteamUGC.Internal.SetUserItemVote(this.Id, up);
			SetUserItemVoteResult_t? nullable1 = nullable;
			nullable = null;
			ref Nullable nullablePointer = ref nullable1;
			flag = (nullablePointer.HasValue ? nullablePointer.GetValueOrDefault().Result == Result.OK : false);
			return flag;
		}
	}
}