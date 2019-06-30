using Steamworks;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Steamworks.Ugc
{
	public struct Query
	{
		private UgcType matchingType;

		private UGCQuery queryType;

		private AppId consumerApp;

		private AppId creatorApp;

		private SteamId? steamid;

		private UserUGCList userType;

		private UserUGCListSortOrder userSort;

		private PublishedFileId[] Files;

		private bool? WantsReturnOnlyIDs;

		private bool? WantsReturnKeyValueTags;

		private bool? WantsReturnLongDescription;

		private bool? WantsReturnMetadata;

		private bool? WantsReturnChildren;

		private bool? WantsReturnAdditionalPreviews;

		private bool? WantsReturnTotalOnly;

		private bool? WantsReturnPlaytimeStats;

		private int? maxCacheAge;

		private string language;

		private int? trendDays;

		private List<string> requiredTags;

		private bool? matchAnyTag;

		private List<string> excludedTags;

		private Dictionary<string, string> requiredKv;

		public static Query All
		{
			get
			{
				return new Query(UgcType.All);
			}
		}

		public static Query AllGuides
		{
			get
			{
				return new Query(UgcType.AllGuides);
			}
		}

		public static Query Artwork
		{
			get
			{
				return new Query(UgcType.Artwork);
			}
		}

		public static Query Collections
		{
			get
			{
				return new Query(UgcType.Collections);
			}
		}

		public static Query ControllerBindings
		{
			get
			{
				return new Query(UgcType.ControllerBindings);
			}
		}

		public static Query GameManagedItems
		{
			get
			{
				return new Query(UgcType.GameManagedItems);
			}
		}

		public static Query IntegratedGuides
		{
			get
			{
				return new Query(UgcType.IntegratedGuides);
			}
		}

		public static Query Items
		{
			get
			{
				return new Query(UgcType.Items);
			}
		}

		public static Query ItemsMtx
		{
			get
			{
				return new Query(UgcType.Items_Mtx);
			}
		}

		public static Query ItemsReadyToUse
		{
			get
			{
				return new Query(UgcType.Items_ReadyToUse);
			}
		}

		public static Query Screenshots
		{
			get
			{
				return new Query(UgcType.Screenshots);
			}
		}

		public static Query UsableInGame
		{
			get
			{
				return new Query(UgcType.UsableInGame);
			}
		}

		public static Query Videos
		{
			get
			{
				return new Query(UgcType.Videos);
			}
		}

		public static Query WebGuides
		{
			get
			{
				return new Query(UgcType.WebGuides);
			}
		}

		public Query(UgcType type)
		{
			this = new Query()
			{
				matchingType = type
			};
		}

		public Query AllowCachedResponse(int maxSecondsAge)
		{
			this.maxCacheAge = new int?(maxSecondsAge);
			return this;
		}

		private void ApplyConstraints(UGCQueryHandle_t handle)
		{
			if (this.requiredTags != null)
			{
				foreach (string requiredTag in this.requiredTags)
				{
					SteamUGC.Internal.AddRequiredTag(handle, requiredTag);
				}
			}
			if (this.excludedTags != null)
			{
				foreach (string excludedTag in this.excludedTags)
				{
					SteamUGC.Internal.AddExcludedTag(handle, excludedTag);
				}
			}
			if (this.requiredKv != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in this.requiredKv)
				{
					SteamUGC.Internal.AddRequiredKeyValueTag(handle, keyValuePair.Key, keyValuePair.Value);
				}
			}
			if (this.matchAnyTag.HasValue)
			{
				SteamUGC.Internal.SetMatchAnyTag(handle, this.matchAnyTag.Value);
			}
			if (this.trendDays.HasValue)
			{
				SteamUGC.Internal.SetRankedByTrendDays(handle, (uint)this.trendDays.Value);
			}
		}

		public Query CreatedByFollowedUsers()
		{
			this.queryType = UGCQuery.CreatedByFollowedUsersRankedByPublicationDate;
			return this;
		}

		public Query CreatedByFriends()
		{
			this.queryType = UGCQuery.CreatedByFriendsRankedByPublicationDate;
			return this;
		}

		public Query FavoritedByFriends()
		{
			this.queryType = UGCQuery.FavoritedByFriendsRankedByPublicationDate;
			return this;
		}

		public async Task<ResultPage?> GetPageAsync(int page)
		{
			ResultPage? nullable;
			UGCQueryHandle_t uGCQueryHandleT;
			if (page <= 0)
			{
				throw new Exception("page should be > 0");
			}
			if (this.consumerApp == 0)
			{
				this.consumerApp = SteamClient.AppId;
			}
			if (this.creatorApp == 0)
			{
				this.creatorApp = this.consumerApp;
			}
			if (this.Files != null)
			{
				uGCQueryHandleT = SteamUGC.Internal.CreateQueryUGCDetailsRequest(this.Files, (uint)this.Files.Length);
			}
			else if (!this.steamid.HasValue)
			{
				uGCQueryHandleT = SteamUGC.Internal.CreateQueryAllUGCRequest1(this.queryType, this.matchingType, this.creatorApp.Value, this.consumerApp.Value, (uint)page);
			}
			else
			{
				ISteamUGC @internal = SteamUGC.Internal;
				SteamId value = this.steamid.Value;
				uGCQueryHandleT = @internal.CreateQueryUserUGCRequest(value.AccountId, this.userType, this.matchingType, this.userSort, this.creatorApp.Value, this.consumerApp.Value, (uint)page);
			}
			this.ApplyConstraints(uGCQueryHandleT);
			SteamUGCQueryCompleted_t? nullable1 = await SteamUGC.Internal.SendQueryUGCRequest(uGCQueryHandleT);
			SteamUGCQueryCompleted_t? nullable2 = nullable1;
			nullable1 = null;
			if (!nullable2.HasValue)
			{
				nullable = null;
			}
			else if (nullable2.Value.Result == Result.OK)
			{
				ResultPage resultPage = new ResultPage()
				{
					Handle = nullable2.Value.Handle,
					ResultCount = (int)nullable2.Value.NumResultsReturned,
					TotalCount = (int)nullable2.Value.TotalMatchingResults,
					CachedData = nullable2.Value.CachedData
				};
				nullable = new ResultPage?(resultPage);
			}
			else
			{
				nullable = null;
			}
			return nullable;
		}

		public Query InLanguage(string lang)
		{
			this.language = lang;
			return this;
		}

		internal Query LimitUser(SteamId steamid)
		{
			if (steamid.Value == (long)0)
			{
				steamid = SteamClient.SteamId;
			}
			this.steamid = new SteamId?(steamid);
			return this;
		}

		public Query MatchAllTags()
		{
			this.matchAnyTag = new bool?(false);
			return this;
		}

		public Query MatchAnyTag()
		{
			this.matchAnyTag = new bool?(true);
			return this;
		}

		public Query NotYetRated()
		{
			this.queryType = UGCQuery.NotYetRated;
			return this;
		}

		public Query RankedByAcceptanceDate()
		{
			this.queryType = UGCQuery.AcceptedForGameRankedByAcceptanceDate;
			return this;
		}

		public Query RankedByAveragePlaytimeTrend()
		{
			this.queryType = UGCQuery.RankedByAveragePlaytimeTrend;
			return this;
		}

		public Query RankedByLifetimeAveragePlaytime()
		{
			this.queryType = UGCQuery.RankedByLifetimeAveragePlaytime;
			return this;
		}

		public Query RankedByLifetimePlaytimeSessions()
		{
			this.queryType = UGCQuery.RankedByLifetimePlaytimeSessions;
			return this;
		}

		public Query RankedByNumTimesReported()
		{
			this.queryType = UGCQuery.RankedByNumTimesReported;
			return this;
		}

		public Query RankedByPlaytimeSessionsTrend()
		{
			this.queryType = UGCQuery.RankedByPlaytimeSessionsTrend;
			return this;
		}

		public Query RankedByPlaytimeTrend()
		{
			this.queryType = UGCQuery.RankedByPlaytimeTrend;
			return this;
		}

		public Query RankedByPublicationDate()
		{
			this.queryType = UGCQuery.RankedByPublicationDate;
			return this;
		}

		public Query RankedByTextSearch()
		{
			this.queryType = UGCQuery.RankedByTextSearch;
			return this;
		}

		public Query RankedByTotalPlaytime()
		{
			this.queryType = UGCQuery.RankedByTotalPlaytime;
			return this;
		}

		public Query RankedByTotalUniqueSubscriptions()
		{
			this.queryType = UGCQuery.RankedByTotalUniqueSubscriptions;
			return this;
		}

		public Query RankedByTotalVotesAsc()
		{
			this.queryType = UGCQuery.RankedByTotalVotesAsc;
			return this;
		}

		public Query RankedByTrend()
		{
			this.queryType = UGCQuery.RankedByTrend;
			return this;
		}

		public Query RankedByVote()
		{
			this.queryType = UGCQuery.RankedByVote;
			return this;
		}

		public Query RankedByVotesUp()
		{
			this.queryType = UGCQuery.RankedByVotesUp;
			return this;
		}

		public Query SortByCreationDate()
		{
			this.userSort = UserUGCListSortOrder.CreationOrderDesc;
			return this;
		}

		public Query SortByCreationDateAsc()
		{
			this.userSort = UserUGCListSortOrder.CreationOrderAsc;
			return this;
		}

		public Query SortByModeration()
		{
			this.userSort = UserUGCListSortOrder.ForModeration;
			return this;
		}

		public Query SortBySubscriptionDate()
		{
			this.userSort = UserUGCListSortOrder.SubscriptionDateDesc;
			return this;
		}

		public Query SortByTitleAsc()
		{
			this.userSort = UserUGCListSortOrder.TitleAsc;
			return this;
		}

		public Query SortByUpdateDate()
		{
			this.userSort = UserUGCListSortOrder.LastUpdatedDesc;
			return this;
		}

		public Query SortByVoteScore()
		{
			this.userSort = UserUGCListSortOrder.VoteScoreDesc;
			return this;
		}

		public Query WhereUserFavorited(SteamId user = null)
		{
			this.userType = UserUGCList.Favorited;
			this.LimitUser(user);
			return this;
		}

		public Query WhereUserFollowed(SteamId user = null)
		{
			this.userType = UserUGCList.Followed;
			this.LimitUser(user);
			return this;
		}

		public Query WhereUserPublished(SteamId user = null)
		{
			this.userType = UserUGCList.Published;
			this.LimitUser(user);
			return this;
		}

		public Query WhereUserSubscribed(SteamId user = null)
		{
			this.userType = UserUGCList.Subscribed;
			this.LimitUser(user);
			return this;
		}

		public Query WhereUserUsedOrPlayed(SteamId user = null)
		{
			this.userType = UserUGCList.UsedOrPlayed;
			this.LimitUser(user);
			return this;
		}

		public Query WhereUserVotedDown(SteamId user = null)
		{
			this.userType = UserUGCList.VotedDown;
			this.LimitUser(user);
			return this;
		}

		public Query WhereUserVotedOn(SteamId user = null)
		{
			this.userType = UserUGCList.VotedOn;
			this.LimitUser(user);
			return this;
		}

		public Query WhereUserVotedUp(SteamId user = null)
		{
			this.userType = UserUGCList.VotedUp;
			this.LimitUser(user);
			return this;
		}

		public Query WhereUserWillVoteLater(SteamId user = null)
		{
			this.userType = UserUGCList.WillVoteLater;
			this.LimitUser(user);
			return this;
		}

		public Query WithAdditionalPreviews(bool b)
		{
			this.WantsReturnAdditionalPreviews = new bool?(b);
			return this;
		}

		public Query WithChildren(bool b)
		{
			this.WantsReturnChildren = new bool?(b);
			return this;
		}

		public Query WithFileId(params PublishedFileId[] files)
		{
			this.Files = files;
			return this;
		}

		public Query WithKeyValueTag(bool b)
		{
			this.WantsReturnKeyValueTags = new bool?(b);
			return this;
		}

		public Query WithLongDescription(bool b)
		{
			this.WantsReturnLongDescription = new bool?(b);
			return this;
		}

		public Query WithMetadata(bool b)
		{
			this.WantsReturnMetadata = new bool?(b);
			return this;
		}

		public Query WithOnlyIDs(bool b)
		{
			this.WantsReturnOnlyIDs = new bool?(b);
			return this;
		}

		public Query WithoutTag(string tag)
		{
			if (this.excludedTags == null)
			{
				this.excludedTags = new List<string>();
			}
			this.excludedTags.Add(tag);
			return this;
		}

		public Query WithPlaytimeStats(bool b)
		{
			this.WantsReturnPlaytimeStats = new bool?(b);
			return this;
		}

		public Query WithTag(string tag)
		{
			if (this.requiredTags == null)
			{
				this.requiredTags = new List<string>();
			}
			this.requiredTags.Add(tag);
			return this;
		}

		public Query WithTotalOnly(bool b)
		{
			this.WantsReturnTotalOnly = new bool?(b);
			return this;
		}

		public Query WithTrendDays(int days)
		{
			this.trendDays = new int?(days);
			return this;
		}

		public Query WithType(UgcType type)
		{
			this.matchingType = type;
			return this;
		}
	}
}