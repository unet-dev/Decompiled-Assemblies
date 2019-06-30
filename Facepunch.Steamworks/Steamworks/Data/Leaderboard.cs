using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	public struct Leaderboard
	{
		internal SteamLeaderboard_t Id;

		private static int[] detailsBuffer;

		private static int[] noDetails;

		public LeaderboardDisplay Display
		{
			get
			{
				return SteamUserStats.Internal.GetLeaderboardDisplayType(this.Id);
			}
		}

		public string Name
		{
			get
			{
				return SteamUserStats.Internal.GetLeaderboardName(this.Id);
			}
		}

		public LeaderboardSort Sort
		{
			get
			{
				return SteamUserStats.Internal.GetLeaderboardSortMethod(this.Id);
			}
		}

		static Leaderboard()
		{
			Leaderboard.detailsBuffer = new Int32[64];
			Leaderboard.noDetails = new Int32[0];
		}

		public async Task<Result> AttachUgc(Steamworks.Data.Ugc file)
		{
			LeaderboardUGCSet_t? nullable = await SteamUserStats.Internal.AttachLeaderboardUGC(this.Id, file.Handle);
			LeaderboardUGCSet_t? nullable1 = nullable;
			nullable = null;
			return (nullable1.HasValue ? nullable1.Value.Result : Result.Fail);
		}

		public async Task<LeaderboardEntry[]> GetScoresAroundUserAsync(int start = -10, int end = 10)
		{
			LeaderboardEntry[] entries;
			LeaderboardScoresDownloaded_t? nullable = await SteamUserStats.Internal.DownloadLeaderboardEntries(this.Id, LeaderboardDataRequest.GlobalAroundUser, start, end);
			LeaderboardScoresDownloaded_t? nullable1 = nullable;
			nullable = null;
			if (nullable1.HasValue)
			{
				entries = await this.LeaderboardResultToEntries(nullable1.Value);
			}
			else
			{
				entries = null;
			}
			return entries;
		}

		public async Task<LeaderboardEntry[]> GetScoresAsync(int count, int offset = 1)
		{
			LeaderboardEntry[] entries;
			if (offset <= 0)
			{
				throw new ArgumentException("Should be 1+", "offset");
			}
			LeaderboardScoresDownloaded_t? nullable = await SteamUserStats.Internal.DownloadLeaderboardEntries(this.Id, LeaderboardDataRequest.Global, offset, offset + count);
			LeaderboardScoresDownloaded_t? nullable1 = nullable;
			nullable = null;
			if (nullable1.HasValue)
			{
				entries = await this.LeaderboardResultToEntries(nullable1.Value);
			}
			else
			{
				entries = null;
			}
			return entries;
		}

		public async Task<LeaderboardEntry[]> GetScoresFromFriendsAsync()
		{
			LeaderboardEntry[] entries;
			LeaderboardScoresDownloaded_t? nullable = await SteamUserStats.Internal.DownloadLeaderboardEntries(this.Id, LeaderboardDataRequest.Friends, 0, 0);
			LeaderboardScoresDownloaded_t? nullable1 = nullable;
			nullable = null;
			if (nullable1.HasValue)
			{
				entries = await this.LeaderboardResultToEntries(nullable1.Value);
			}
			else
			{
				entries = null;
			}
			return entries;
		}

		internal async Task<LeaderboardEntry[]> LeaderboardResultToEntries(LeaderboardScoresDownloaded_t r)
		{
			LeaderboardEntry[] leaderboardEntryArray;
			if (r.CEntryCount > 0)
			{
				LeaderboardEntry[] leaderboardEntryArray1 = new LeaderboardEntry[r.CEntryCount];
				LeaderboardEntry_t leaderboardEntryT = new LeaderboardEntry_t();
				for (int i = 0; i < (int)leaderboardEntryArray1.Length; i++)
				{
					if (SteamUserStats.Internal.GetDownloadedLeaderboardEntry(r.SteamLeaderboardEntries, i, ref leaderboardEntryT, Leaderboard.detailsBuffer, (int)Leaderboard.detailsBuffer.Length))
					{
						leaderboardEntryArray1[i] = LeaderboardEntry.From(leaderboardEntryT, Leaderboard.detailsBuffer);
					}
				}
				await this.WaitForUserNames(leaderboardEntryArray1);
				leaderboardEntryArray = leaderboardEntryArray1;
			}
			else
			{
				leaderboardEntryArray = null;
			}
			return leaderboardEntryArray;
		}

		public async Task<LeaderboardUpdate?> ReplaceScore(int score, int[] details = null)
		{
			LeaderboardUpdate? nullable;
			if (details == null)
			{
				details = Leaderboard.noDetails;
			}
			LeaderboardScoreUploaded_t? nullable1 = await SteamUserStats.Internal.UploadLeaderboardScore(this.Id, LeaderboardUploadScoreMethod.ForceUpdate, score, details, (int)details.Length);
			LeaderboardScoreUploaded_t? nullable2 = nullable1;
			nullable1 = null;
			if (nullable2.HasValue)
			{
				nullable = new LeaderboardUpdate?(LeaderboardUpdate.From(nullable2.Value));
			}
			else
			{
				nullable = null;
			}
			return nullable;
		}

		public async Task<LeaderboardUpdate?> SubmitScoreAsync(int score, int[] details = null)
		{
			LeaderboardUpdate? nullable;
			if (details == null)
			{
				details = Leaderboard.noDetails;
			}
			LeaderboardScoreUploaded_t? nullable1 = await SteamUserStats.Internal.UploadLeaderboardScore(this.Id, LeaderboardUploadScoreMethod.KeepBest, score, details, (int)details.Length);
			LeaderboardScoreUploaded_t? nullable2 = nullable1;
			nullable1 = null;
			if (nullable2.HasValue)
			{
				nullable = new LeaderboardUpdate?(LeaderboardUpdate.From(nullable2.Value));
			}
			else
			{
				nullable = null;
			}
			return nullable;
		}

		internal async Task WaitForUserNames(LeaderboardEntry[] entries)
		{
			bool flag = false;
			while (!flag)
			{
				flag = true;
				LeaderboardEntry[] leaderboardEntryArray = entries;
				for (int i = 0; i < (int)leaderboardEntryArray.Length; i++)
				{
					LeaderboardEntry leaderboardEntry = leaderboardEntryArray[i];
					if (leaderboardEntry.User.Id != (long)0)
					{
						if (SteamFriends.Internal.RequestUserInformation(leaderboardEntry.User.Id, true))
						{
							flag = false;
							leaderboardEntry = new LeaderboardEntry();
						}
					}
				}
				leaderboardEntryArray = null;
				await Task.Delay(1);
			}
		}
	}
}