using Steamworks.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Steamworks
{
	public static class SteamUserStats
	{
		private static ISteamUserStats _internal;

		public static IEnumerable<Achievement> Achievements
		{
			get
			{
				for (int i = 0; (long)i < (ulong)SteamUserStats.Internal.GetNumAchievements(); i++)
				{
					yield return new Achievement(SteamUserStats.Internal.GetAchievementName((uint)i));
				}
			}
		}

		internal static ISteamUserStats Internal
		{
			get
			{
				if (SteamUserStats._internal == null)
				{
					SteamUserStats._internal = new ISteamUserStats();
					SteamUserStats._internal.Init();
					SteamUserStats.RequestCurrentStats();
				}
				return SteamUserStats._internal;
			}
		}

		public static bool StatsRecieved
		{
			get;
			internal set;
		}

		public static bool AddStat(string name, int amount = 1)
		{
			int statInt = SteamUserStats.GetStatInt(name);
			return SteamUserStats.SetStat(name, statInt + amount);
		}

		public static bool AddStat(string name, float amount = 1f)
		{
			float statFloat = SteamUserStats.GetStatFloat(name);
			return SteamUserStats.SetStat(name, statFloat + amount);
		}

		public static async Task<Leaderboard?> FindLeaderboardAsync(string name)
		{
			Leaderboard? nullable;
			bool flag;
			LeaderboardFindResult_t? nullable1 = await SteamUserStats.Internal.FindLeaderboard(name);
			LeaderboardFindResult_t? nullable2 = nullable1;
			nullable1 = null;
			flag = (!nullable2.HasValue ? true : nullable2.Value.LeaderboardFound == 0);
			if (!flag)
			{
				nullable = new Leaderboard?(new Leaderboard()
				{
					Id = nullable2.Value.SteamLeaderboard
				});
			}
			else
			{
				nullable = null;
			}
			return nullable;
		}

		public static async Task<Leaderboard?> FindOrCreateLeaderboardAsync(string name, LeaderboardSort sort, LeaderboardDisplay display)
		{
			Leaderboard? nullable;
			bool flag;
			LeaderboardFindResult_t? nullable1 = await SteamUserStats.Internal.FindOrCreateLeaderboard(name, sort, display);
			LeaderboardFindResult_t? nullable2 = nullable1;
			nullable1 = null;
			flag = (!nullable2.HasValue ? true : nullable2.Value.LeaderboardFound == 0);
			if (!flag)
			{
				nullable = new Leaderboard?(new Leaderboard()
				{
					Id = nullable2.Value.SteamLeaderboard
				});
			}
			else
			{
				nullable = null;
			}
			return nullable;
		}

		public static float GetStatFloat(string name)
		{
			float single = 0f;
			SteamUserStats.Internal.GetStat2(name, ref single);
			return single;
		}

		public static int GetStatInt(string name)
		{
			int num = 0;
			SteamUserStats.Internal.GetStat1(name, ref num);
			return num;
		}

		internal static void InstallEvents()
		{
			UserStatsReceived_t.Install((UserStatsReceived_t x) => {
				if (x.SteamIDUser == SteamClient.SteamId)
				{
					SteamUserStats.StatsRecieved = true;
				}
				Action<SteamId, Result> onUserStatsReceived = SteamUserStats.OnUserStatsReceived;
				if (onUserStatsReceived != null)
				{
					onUserStatsReceived(x.SteamIDUser, x.Result);
				}
				else
				{
				}
			}, false);
			UserStatsStored_t.Install((UserStatsStored_t x) => {
				Action<Result> onUserStatsStored = SteamUserStats.OnUserStatsStored;
				if (onUserStatsStored != null)
				{
					onUserStatsStored(x.Result);
				}
				else
				{
				}
			}, false);
			UserAchievementStored_t.Install((UserAchievementStored_t x) => {
				Action<Achievement, int, int> onAchievementProgress = SteamUserStats.OnAchievementProgress;
				if (onAchievementProgress != null)
				{
					onAchievementProgress(new Achievement(x.AchievementName), x.CurProgress, x.MaxProgress);
				}
				else
				{
				}
			}, false);
			UserStatsUnloaded_t.Install((UserStatsUnloaded_t x) => {
				Action<SteamId> onUserStatsUnloaded = SteamUserStats.OnUserStatsUnloaded;
				if (onUserStatsUnloaded != null)
				{
					onUserStatsUnloaded(x.SteamIDUser);
				}
				else
				{
				}
			}, false);
			UserAchievementIconFetched_t.Install((UserAchievementIconFetched_t x) => {
				Action<string, int> onAchievementIconFetched = SteamUserStats.OnAchievementIconFetched;
				if (onAchievementIconFetched != null)
				{
					onAchievementIconFetched(x.AchievementName, x.IconHandle);
				}
				else
				{
				}
			}, false);
		}

		public static async Task<int> PlayerCountAsync()
		{
			bool flag;
			NumberOfCurrentPlayers_t? numberOfCurrentPlayers = await SteamUserStats.Internal.GetNumberOfCurrentPlayers();
			NumberOfCurrentPlayers_t? nullable = numberOfCurrentPlayers;
			numberOfCurrentPlayers = null;
			flag = (!nullable.HasValue ? true : nullable.Value.Success == 0);
			return (!flag ? nullable.Value.CPlayers : -1);
		}

		public static bool RequestCurrentStats()
		{
			return SteamUserStats.Internal.RequestCurrentStats();
		}

		public static bool ResetAll(bool includeAchievements)
		{
			return SteamUserStats.Internal.ResetAllStats(includeAchievements);
		}

		public static bool SetStat(string name, int value)
		{
			return SteamUserStats.Internal.SetStat1(name, value);
		}

		public static bool SetStat(string name, float value)
		{
			return SteamUserStats.Internal.SetStat2(name, value);
		}

		internal static void Shutdown()
		{
			SteamUserStats._internal = null;
		}

		public static bool StoreStats()
		{
			return SteamUserStats.Internal.StoreStats();
		}

		internal static event Action<string, int> OnAchievementIconFetched;

		public static event Action<Achievement, int, int> OnAchievementProgress;

		public static event Action<SteamId, Result> OnUserStatsReceived;

		public static event Action<Result> OnUserStatsStored;

		public static event Action<SteamId> OnUserStatsUnloaded;
	}
}