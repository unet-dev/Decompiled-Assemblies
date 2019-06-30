using Steamworks.Data;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Steamworks
{
	public static class SteamServerStats
	{
		private static ISteamGameServerStats _internal;

		internal static ISteamGameServerStats Internal
		{
			get
			{
				if (SteamServerStats._internal == null)
				{
					SteamServerStats._internal = new ISteamGameServerStats();
					SteamServerStats._internal.InitServer();
				}
				return SteamServerStats._internal;
			}
		}

		public static bool ClearAchievement(SteamId steamid, string name)
		{
			return SteamServerStats.Internal.ClearUserAchievement(steamid, name);
		}

		public static bool GetAchievement(SteamId steamid, string name)
		{
			bool flag;
			bool flag1 = false;
			flag = (SteamServerStats.Internal.GetUserAchievement(steamid, name, ref flag1) ? flag1 : false);
			return flag;
		}

		public static float GetFloat(SteamId steamid, string name, float defaultValue = 0f)
		{
			float single;
			float single1 = defaultValue;
			single = (SteamServerStats.Internal.GetUserStat2(steamid, name, ref single1) ? single1 : defaultValue);
			return single;
		}

		public static int GetInt(SteamId steamid, string name, int defaultValue = 0)
		{
			int num;
			int num1 = defaultValue;
			num = (SteamServerStats.Internal.GetUserStat1(steamid, name, ref num1) ? num1 : defaultValue);
			return num;
		}

		public static async Task<Result> RequestUserStats(SteamId steamid)
		{
			GSStatsReceived_t? nullable = await SteamServerStats.Internal.RequestUserStats(steamid);
			GSStatsReceived_t? nullable1 = nullable;
			nullable = null;
			return (nullable1.HasValue ? nullable1.Value.Result : Result.Fail);
		}

		public static bool SetAchievement(SteamId steamid, string name)
		{
			return SteamServerStats.Internal.SetUserAchievement(steamid, name);
		}

		public static bool SetFloat(SteamId steamid, string name, float stat)
		{
			return SteamServerStats.Internal.SetUserStat2(steamid, name, stat);
		}

		public static bool SetInt(SteamId steamid, string name, int stat)
		{
			return SteamServerStats.Internal.SetUserStat1(steamid, name, stat);
		}

		internal static void Shutdown()
		{
			SteamServerStats._internal = null;
		}

		public static async Task<Result> StoreUserStats(SteamId steamid)
		{
			GSStatsStored_t? nullable = await SteamServerStats.Internal.StoreUserStats(steamid);
			GSStatsStored_t? nullable1 = nullable;
			nullable = null;
			return (nullable1.HasValue ? nullable1.Value.Result : Result.Fail);
		}
	}
}