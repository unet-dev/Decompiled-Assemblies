using Facepunch.Steamworks.Interop;
using SteamNative;
using System;
using System.Runtime.CompilerServices;

namespace Facepunch.Steamworks
{
	public class ServerStats
	{
		internal Server server;

		internal ServerStats(Server s)
		{
			this.server = s;
		}

		public bool ClearAchievement(ulong steamid, string name)
		{
			return this.server.native.gameServerStats.ClearUserAchievement(steamid, name);
		}

		public void Commit(ulong steamid, Action<ulong, bool> Callback = null)
		{
			if (Callback == null)
			{
				this.server.native.gameServerStats.StoreUserStats(steamid, null);
				return;
			}
			this.server.native.gameServerStats.StoreUserStats(steamid, (GSStatsStored_t o, bool failed) => Callback(steamid, (o.Result != Result.OK ? false : !failed)));
		}

		public bool GetAchievement(ulong steamid, string name)
		{
			bool flag = false;
			if (!this.server.native.gameServerStats.GetUserAchievement(steamid, name, ref flag))
			{
				return false;
			}
			return flag;
		}

		public float GetFloat(ulong steamid, string name, float defaultValue = 0f)
		{
			float single = defaultValue;
			if (!this.server.native.gameServerStats.GetUserStat0(steamid, name, out single))
			{
				return defaultValue;
			}
			return single;
		}

		public int GetInt(ulong steamid, string name, int defaultValue = 0)
		{
			int num = defaultValue;
			if (!this.server.native.gameServerStats.GetUserStat(steamid, name, out num))
			{
				return defaultValue;
			}
			return num;
		}

		public void Refresh(ulong steamid, Action<ulong, bool> Callback = null)
		{
			if (Callback == null)
			{
				this.server.native.gameServerStats.RequestUserStats(steamid, null);
				return;
			}
			this.server.native.gameServerStats.RequestUserStats(steamid, (GSStatsReceived_t o, bool failed) => Callback(steamid, (o.Result != Result.OK ? false : !failed)));
		}

		public bool SetAchievement(ulong steamid, string name)
		{
			return this.server.native.gameServerStats.SetUserAchievement(steamid, name);
		}

		public bool SetFloat(ulong steamid, string name, float stat)
		{
			return this.server.native.gameServerStats.SetUserStat0(steamid, name, stat);
		}

		public bool SetInt(ulong steamid, string name, int stat)
		{
			return this.server.native.gameServerStats.SetUserStat(steamid, name, stat);
		}

		public struct StatsReceived
		{
			public int Result;

			public ulong SteamId;
		}
	}
}