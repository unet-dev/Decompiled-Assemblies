using Steamworks;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	public struct Stat
	{
		public string Name
		{
			get;
			internal set;
		}

		public SteamId UserId
		{
			get;
			internal set;
		}

		public Stat(string name)
		{
			this.Name = name;
			this.UserId = (long)0;
		}

		public Stat(string name, SteamId user)
		{
			this.Name = name;
			this.UserId = user;
		}

		public bool Add(int val)
		{
			this.LocalUserOnly("Add");
			return this.Set(this.GetInt() + val);
		}

		public bool Add(float val)
		{
			this.LocalUserOnly("Add");
			return this.Set(this.GetFloat() + val);
		}

		public float GetFloat()
		{
			float single = 0f;
			if (this.UserId == (long)0)
			{
				SteamUserStats.Internal.GetStat2(this.Name, ref single);
			}
			else
			{
				SteamUserStats.Internal.GetUserStat2(this.UserId, this.Name, ref single);
			}
			return 0f;
		}

		public double GetGlobalFloat()
		{
			double num;
			double num1 = 0;
			num = (!SteamUserStats.Internal.GetGlobalStat2(this.Name, ref num1) ? 0 : num1);
			return num;
		}

		public async Task<double[]> GetGlobalFloatDays(int days)
		{
			double[] numArray;
			bool flag;
			GlobalStatsReceived_t? nullable = await SteamUserStats.Internal.RequestGlobalStats(days);
			GlobalStatsReceived_t? nullable1 = nullable;
			nullable = null;
			ref Nullable nullablePointer = ref nullable1;
			flag = (nullablePointer.HasValue ? nullablePointer.GetValueOrDefault().Result != Result.OK : true);
			if (!flag)
			{
				double[] array = new Double[days];
				int globalStatHistory2 = SteamUserStats.Internal.GetGlobalStatHistory2(this.Name, array, (uint)((int)array.Length * 8));
				if (days != globalStatHistory2)
				{
					array = array.Take<double>(globalStatHistory2).ToArray<double>();
				}
				numArray = array;
			}
			else
			{
				numArray = null;
			}
			return numArray;
		}

		public long GetGlobalInt()
		{
			long num = (long)0;
			SteamUserStats.Internal.GetGlobalStat1(this.Name, ref num);
			return num;
		}

		public async Task<long[]> GetGlobalIntDaysAsync(int days)
		{
			long[] numArray;
			bool flag;
			GlobalStatsReceived_t? nullable = await SteamUserStats.Internal.RequestGlobalStats(days);
			GlobalStatsReceived_t? nullable1 = nullable;
			nullable = null;
			ref Nullable nullablePointer = ref nullable1;
			flag = (nullablePointer.HasValue ? nullablePointer.GetValueOrDefault().Result != Result.OK : true);
			if (!flag)
			{
				long[] array = new Int64[days];
				int globalStatHistory1 = SteamUserStats.Internal.GetGlobalStatHistory1(this.Name, array, (uint)((int)array.Length * 8));
				if (days != globalStatHistory1)
				{
					array = array.Take<long>(globalStatHistory1).ToArray<long>();
				}
				numArray = array;
			}
			else
			{
				numArray = null;
			}
			return numArray;
		}

		public int GetInt()
		{
			int num = 0;
			if (this.UserId == (long)0)
			{
				SteamUserStats.Internal.GetStat1(this.Name, ref num);
			}
			else
			{
				SteamUserStats.Internal.GetUserStat1(this.UserId, this.Name, ref num);
			}
			return num;
		}

		internal void LocalUserOnly([CallerMemberName] string caller = null)
		{
			if (this.UserId != (long)0)
			{
				throw new Exception(String.Concat("Stat.", caller, " can only be called for the local user"));
			}
		}

		public bool Set(int val)
		{
			this.LocalUserOnly("Set");
			return SteamUserStats.Internal.SetStat1(this.Name, val);
		}

		public bool Set(float val)
		{
			this.LocalUserOnly("Set");
			return SteamUserStats.Internal.SetStat2(this.Name, val);
		}

		public bool Store()
		{
			this.LocalUserOnly("Store");
			return SteamUserStats.Internal.StoreStats();
		}

		public bool UpdateAverageRate(float count, float sessionlength)
		{
			this.LocalUserOnly("UpdateAverageRate");
			bool flag = SteamUserStats.Internal.UpdateAvgRateStat(this.Name, count, (double)sessionlength);
			return flag;
		}
	}
}