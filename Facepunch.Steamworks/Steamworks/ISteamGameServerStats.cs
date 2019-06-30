using Steamworks.Data;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks
{
	internal class ISteamGameServerStats : SteamInterface
	{
		private ISteamGameServerStats.FRequestUserStats _RequestUserStats;

		private ISteamGameServerStats.FGetUserStat1 _GetUserStat1;

		private ISteamGameServerStats.FGetUserStat2 _GetUserStat2;

		private ISteamGameServerStats.FGetUserAchievement _GetUserAchievement;

		private ISteamGameServerStats.FSetUserStat1 _SetUserStat1;

		private ISteamGameServerStats.FSetUserStat2 _SetUserStat2;

		private ISteamGameServerStats.FUpdateUserAvgRateStat _UpdateUserAvgRateStat;

		private ISteamGameServerStats.FSetUserAchievement _SetUserAchievement;

		private ISteamGameServerStats.FClearUserAchievement _ClearUserAchievement;

		private ISteamGameServerStats.FStoreUserStats _StoreUserStats;

		public override string InterfaceName
		{
			get
			{
				return "SteamGameServerStats001";
			}
		}

		public ISteamGameServerStats()
		{
		}

		internal bool ClearUserAchievement(SteamId steamIDUser, string pchName)
		{
			return this._ClearUserAchievement(this.Self, steamIDUser, pchName);
		}

		internal bool GetUserAchievement(SteamId steamIDUser, string pchName, ref bool pbAchieved)
		{
			return this._GetUserAchievement(this.Self, steamIDUser, pchName, ref pbAchieved);
		}

		internal bool GetUserStat1(SteamId steamIDUser, string pchName, ref int pData)
		{
			return this._GetUserStat1(this.Self, steamIDUser, pchName, ref pData);
		}

		internal bool GetUserStat2(SteamId steamIDUser, string pchName, ref float pData)
		{
			return this._GetUserStat2(this.Self, steamIDUser, pchName, ref pData);
		}

		public override void InitInternals()
		{
			this._RequestUserStats = Marshal.GetDelegateForFunctionPointer<ISteamGameServerStats.FRequestUserStats>(Marshal.ReadIntPtr(this.VTable, 0));
			this._GetUserStat1 = Marshal.GetDelegateForFunctionPointer<ISteamGameServerStats.FGetUserStat1>(Marshal.ReadIntPtr(this.VTable, (Config.Os == OsType.Windows ? 16 : 8)));
			this._GetUserStat2 = Marshal.GetDelegateForFunctionPointer<ISteamGameServerStats.FGetUserStat2>(Marshal.ReadIntPtr(this.VTable, (Config.Os == OsType.Windows ? 8 : 16)));
			this._GetUserAchievement = Marshal.GetDelegateForFunctionPointer<ISteamGameServerStats.FGetUserAchievement>(Marshal.ReadIntPtr(this.VTable, 24));
			this._SetUserStat1 = Marshal.GetDelegateForFunctionPointer<ISteamGameServerStats.FSetUserStat1>(Marshal.ReadIntPtr(this.VTable, (Config.Os == OsType.Windows ? 40 : 32)));
			this._SetUserStat2 = Marshal.GetDelegateForFunctionPointer<ISteamGameServerStats.FSetUserStat2>(Marshal.ReadIntPtr(this.VTable, (Config.Os == OsType.Windows ? 32 : 40)));
			this._UpdateUserAvgRateStat = Marshal.GetDelegateForFunctionPointer<ISteamGameServerStats.FUpdateUserAvgRateStat>(Marshal.ReadIntPtr(this.VTable, 48));
			this._SetUserAchievement = Marshal.GetDelegateForFunctionPointer<ISteamGameServerStats.FSetUserAchievement>(Marshal.ReadIntPtr(this.VTable, 56));
			this._ClearUserAchievement = Marshal.GetDelegateForFunctionPointer<ISteamGameServerStats.FClearUserAchievement>(Marshal.ReadIntPtr(this.VTable, 64));
			this._StoreUserStats = Marshal.GetDelegateForFunctionPointer<ISteamGameServerStats.FStoreUserStats>(Marshal.ReadIntPtr(this.VTable, 72));
		}

		internal async Task<GSStatsReceived_t?> RequestUserStats(SteamId steamIDUser)
		{
			GSStatsReceived_t? resultAsync = await GSStatsReceived_t.GetResultAsync(this._RequestUserStats(this.Self, steamIDUser));
			return resultAsync;
		}

		internal bool SetUserAchievement(SteamId steamIDUser, string pchName)
		{
			return this._SetUserAchievement(this.Self, steamIDUser, pchName);
		}

		internal bool SetUserStat1(SteamId steamIDUser, string pchName, int nData)
		{
			return this._SetUserStat1(this.Self, steamIDUser, pchName, nData);
		}

		internal bool SetUserStat2(SteamId steamIDUser, string pchName, float fData)
		{
			return this._SetUserStat2(this.Self, steamIDUser, pchName, fData);
		}

		internal override void Shutdown()
		{
			base.Shutdown();
			this._RequestUserStats = null;
			this._GetUserStat1 = null;
			this._GetUserStat2 = null;
			this._GetUserAchievement = null;
			this._SetUserStat1 = null;
			this._SetUserStat2 = null;
			this._UpdateUserAvgRateStat = null;
			this._SetUserAchievement = null;
			this._ClearUserAchievement = null;
			this._StoreUserStats = null;
		}

		internal async Task<GSStatsStored_t?> StoreUserStats(SteamId steamIDUser)
		{
			GSStatsStored_t? resultAsync = await GSStatsStored_t.GetResultAsync(this._StoreUserStats(this.Self, steamIDUser));
			return resultAsync;
		}

		internal bool UpdateUserAvgRateStat(SteamId steamIDUser, string pchName, float flCountThisSession, double dSessionLength)
		{
			bool self = this._UpdateUserAvgRateStat(this.Self, steamIDUser, pchName, flCountThisSession, dSessionLength);
			return self;
		}

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FClearUserAchievement(IntPtr self, SteamId steamIDUser, string pchName);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetUserAchievement(IntPtr self, SteamId steamIDUser, string pchName, ref bool pbAchieved);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetUserStat1(IntPtr self, SteamId steamIDUser, string pchName, ref int pData);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetUserStat2(IntPtr self, SteamId steamIDUser, string pchName, ref float pData);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FRequestUserStats(IntPtr self, SteamId steamIDUser);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetUserAchievement(IntPtr self, SteamId steamIDUser, string pchName);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetUserStat1(IntPtr self, SteamId steamIDUser, string pchName, int nData);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetUserStat2(IntPtr self, SteamId steamIDUser, string pchName, float fData);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FStoreUserStats(IntPtr self, SteamId steamIDUser);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FUpdateUserAvgRateStat(IntPtr self, SteamId steamIDUser, string pchName, float flCountThisSession, double dSessionLength);
	}
}