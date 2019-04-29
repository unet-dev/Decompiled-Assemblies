using Facepunch.Steamworks.Interop;
using SteamNative;
using System;

namespace Facepunch.Steamworks
{
	public class Stats : IDisposable
	{
		internal Client client;

		internal Stats(Client c)
		{
			this.client = c;
		}

		public bool Add(string name, int amount = 1, bool store = true)
		{
			int num = this.GetInt(name);
			return this.Set(name, num + amount, store);
		}

		public void Dispose()
		{
			this.client = null;
		}

		public float GetFloat(string name)
		{
			float single = 0f;
			this.client.native.userstats.GetStat0(name, out single);
			return single;
		}

		public double GetGlobalFloat(string name)
		{
			double num = 0;
			this.client.native.userstats.GetGlobalStat0(name, out num);
			return num;
		}

		public long GetGlobalInt(string name)
		{
			long num = (long)0;
			this.client.native.userstats.GetGlobalStat(name, out num);
			return num;
		}

		public int GetInt(string name)
		{
			int num = 0;
			this.client.native.userstats.GetStat(name, out num);
			return num;
		}

		public bool ResetAll(bool includeAchievements)
		{
			return this.client.native.userstats.ResetAllStats(includeAchievements);
		}

		public bool Set(string name, int value, bool store = true)
		{
			bool flag = this.client.native.userstats.SetStat(name, value);
			if (!store)
			{
				return flag;
			}
			if (!flag)
			{
				return false;
			}
			return this.client.native.userstats.StoreStats();
		}

		public bool Set(string name, float value, bool store = true)
		{
			bool flag = this.client.native.userstats.SetStat0(name, value);
			if (!store)
			{
				return flag;
			}
			if (!flag)
			{
				return false;
			}
			return this.client.native.userstats.StoreStats();
		}

		public bool StoreStats()
		{
			return this.client.native.userstats.StoreStats();
		}

		public void UpdateGlobalStats(int days = 1)
		{
			this.client.native.userstats.GetNumberOfCurrentPlayers(null);
			this.client.native.userstats.RequestGlobalAchievementPercentages(null);
			this.client.native.userstats.RequestGlobalStats(days, null);
		}

		public void UpdateStats()
		{
			this.client.native.userstats.RequestCurrentStats();
		}
	}
}