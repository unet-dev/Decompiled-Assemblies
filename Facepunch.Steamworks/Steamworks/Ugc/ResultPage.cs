using Steamworks;
using Steamworks.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Steamworks.Ugc
{
	public struct ResultPage : IDisposable
	{
		internal UGCQueryHandle_t Handle;

		public int ResultCount;

		public int TotalCount;

		public bool CachedData;

		public IEnumerable<Item> Entries
		{
			get
			{
				SteamUGCDetails_t steamUGCDetailsT = new SteamUGCDetails_t();
				for (uint i = 0; (ulong)i < (long)this.ResultCount; i++)
				{
					if (SteamUGC.Internal.GetQueryUGCResult(this.Handle, i, ref steamUGCDetailsT))
					{
						Item stat = Item.From(steamUGCDetailsT);
						stat.NumSubscriptions = this.GetStat(i, ItemStatistic.NumSubscriptions);
						stat.NumFavorites = this.GetStat(i, ItemStatistic.NumFavorites);
						stat.NumFollowers = this.GetStat(i, ItemStatistic.NumFollowers);
						stat.NumUniqueSubscriptions = this.GetStat(i, ItemStatistic.NumUniqueSubscriptions);
						stat.NumUniqueFavorites = this.GetStat(i, ItemStatistic.NumUniqueFavorites);
						stat.NumUniqueFollowers = this.GetStat(i, ItemStatistic.NumUniqueFollowers);
						stat.NumUniqueWebsiteViews = this.GetStat(i, ItemStatistic.NumUniqueWebsiteViews);
						stat.ReportScore = this.GetStat(i, ItemStatistic.ReportScore);
						stat.NumSecondsPlayed = this.GetStat(i, ItemStatistic.NumSecondsPlayed);
						stat.NumPlaytimeSessions = this.GetStat(i, ItemStatistic.NumPlaytimeSessions);
						stat.NumComments = this.GetStat(i, ItemStatistic.NumComments);
						stat.NumSecondsPlayedDuringTimePeriod = this.GetStat(i, ItemStatistic.NumSecondsPlayedDuringTimePeriod);
						stat.NumPlaytimeSessionsDuringTimePeriod = this.GetStat(i, ItemStatistic.NumPlaytimeSessionsDuringTimePeriod);
						StringBuilder stringBuilder = Helpers.TakeStringBuilder();
						if (SteamUGC.Internal.GetQueryUGCPreviewURL(this.Handle, i, stringBuilder, (uint)stringBuilder.Capacity))
						{
							stat.PreviewImageUrl = stringBuilder.ToString();
						}
						yield return stat;
						stat = new Item();
						stringBuilder = null;
					}
				}
			}
		}

		public void Dispose()
		{
			if (this.Handle != (long)0)
			{
				SteamUGC.Internal.ReleaseQueryUGCRequest(this.Handle);
				this.Handle = (long)0;
			}
		}

		private ulong GetStat(uint index, ItemStatistic stat)
		{
			ulong num;
			ulong num1 = (ulong)0;
			if (SteamUGC.Internal.GetQueryUGCStatistic(this.Handle, index, stat, ref num1))
			{
				num = num1;
			}
			else
			{
				num = (ulong)0;
			}
			return num;
		}
	}
}