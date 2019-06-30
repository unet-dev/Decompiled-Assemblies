using Steamworks.Data;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Steamworks
{
	internal class ISteamUserStats : SteamInterface
	{
		private ISteamUserStats.FRequestCurrentStats _RequestCurrentStats;

		private ISteamUserStats.FGetStat1 _GetStat1;

		private ISteamUserStats.FGetStat2 _GetStat2;

		private ISteamUserStats.FSetStat1 _SetStat1;

		private ISteamUserStats.FSetStat2 _SetStat2;

		private ISteamUserStats.FUpdateAvgRateStat _UpdateAvgRateStat;

		private ISteamUserStats.FGetAchievement _GetAchievement;

		private ISteamUserStats.FSetAchievement _SetAchievement;

		private ISteamUserStats.FClearAchievement _ClearAchievement;

		private ISteamUserStats.FGetAchievementAndUnlockTime _GetAchievementAndUnlockTime;

		private ISteamUserStats.FStoreStats _StoreStats;

		private ISteamUserStats.FGetAchievementIcon _GetAchievementIcon;

		private ISteamUserStats.FGetAchievementDisplayAttribute _GetAchievementDisplayAttribute;

		private ISteamUserStats.FIndicateAchievementProgress _IndicateAchievementProgress;

		private ISteamUserStats.FGetNumAchievements _GetNumAchievements;

		private ISteamUserStats.FGetAchievementName _GetAchievementName;

		private ISteamUserStats.FRequestUserStats _RequestUserStats;

		private ISteamUserStats.FGetUserStat1 _GetUserStat1;

		private ISteamUserStats.FGetUserStat2 _GetUserStat2;

		private ISteamUserStats.FGetUserAchievement _GetUserAchievement;

		private ISteamUserStats.FGetUserAchievementAndUnlockTime _GetUserAchievementAndUnlockTime;

		private ISteamUserStats.FResetAllStats _ResetAllStats;

		private ISteamUserStats.FFindOrCreateLeaderboard _FindOrCreateLeaderboard;

		private ISteamUserStats.FFindLeaderboard _FindLeaderboard;

		private ISteamUserStats.FGetLeaderboardName _GetLeaderboardName;

		private ISteamUserStats.FGetLeaderboardEntryCount _GetLeaderboardEntryCount;

		private ISteamUserStats.FGetLeaderboardSortMethod _GetLeaderboardSortMethod;

		private ISteamUserStats.FGetLeaderboardDisplayType _GetLeaderboardDisplayType;

		private ISteamUserStats.FDownloadLeaderboardEntries _DownloadLeaderboardEntries;

		private ISteamUserStats.FDownloadLeaderboardEntriesForUsers _DownloadLeaderboardEntriesForUsers;

		private ISteamUserStats.FGetDownloadedLeaderboardEntry _GetDownloadedLeaderboardEntry;

		private ISteamUserStats.FUploadLeaderboardScore _UploadLeaderboardScore;

		private ISteamUserStats.FAttachLeaderboardUGC _AttachLeaderboardUGC;

		private ISteamUserStats.FGetNumberOfCurrentPlayers _GetNumberOfCurrentPlayers;

		private ISteamUserStats.FRequestGlobalAchievementPercentages _RequestGlobalAchievementPercentages;

		private ISteamUserStats.FGetMostAchievedAchievementInfo _GetMostAchievedAchievementInfo;

		private ISteamUserStats.FGetNextMostAchievedAchievementInfo _GetNextMostAchievedAchievementInfo;

		private ISteamUserStats.FGetAchievementAchievedPercent _GetAchievementAchievedPercent;

		private ISteamUserStats.FRequestGlobalStats _RequestGlobalStats;

		private ISteamUserStats.FGetGlobalStat1 _GetGlobalStat1;

		private ISteamUserStats.FGetGlobalStat2 _GetGlobalStat2;

		private ISteamUserStats.FGetGlobalStatHistory1 _GetGlobalStatHistory1;

		private ISteamUserStats.FGetGlobalStatHistory2 _GetGlobalStatHistory2;

		public override string InterfaceName
		{
			get
			{
				return "STEAMUSERSTATS_INTERFACE_VERSION011";
			}
		}

		public ISteamUserStats()
		{
		}

		internal async Task<LeaderboardUGCSet_t?> AttachLeaderboardUGC(SteamLeaderboard_t hSteamLeaderboard, UGCHandle_t hUGC)
		{
			LeaderboardUGCSet_t? resultAsync = await LeaderboardUGCSet_t.GetResultAsync(this._AttachLeaderboardUGC(this.Self, hSteamLeaderboard, hUGC));
			return resultAsync;
		}

		internal bool ClearAchievement(string pchName)
		{
			return this._ClearAchievement(this.Self, pchName);
		}

		internal async Task<LeaderboardScoresDownloaded_t?> DownloadLeaderboardEntries(SteamLeaderboard_t hSteamLeaderboard, LeaderboardDataRequest eLeaderboardDataRequest, int nRangeStart, int nRangeEnd)
		{
			LeaderboardScoresDownloaded_t? resultAsync = await LeaderboardScoresDownloaded_t.GetResultAsync(this._DownloadLeaderboardEntries(this.Self, hSteamLeaderboard, eLeaderboardDataRequest, nRangeStart, nRangeEnd));
			return resultAsync;
		}

		internal async Task<LeaderboardScoresDownloaded_t?> DownloadLeaderboardEntriesForUsers(SteamLeaderboard_t hSteamLeaderboard, [In][Out] SteamId[] prgUsers, int cUsers)
		{
			LeaderboardScoresDownloaded_t? resultAsync = await LeaderboardScoresDownloaded_t.GetResultAsync(this._DownloadLeaderboardEntriesForUsers(this.Self, hSteamLeaderboard, prgUsers, cUsers));
			return resultAsync;
		}

		internal async Task<LeaderboardFindResult_t?> FindLeaderboard(string pchLeaderboardName)
		{
			LeaderboardFindResult_t? resultAsync = await LeaderboardFindResult_t.GetResultAsync(this._FindLeaderboard(this.Self, pchLeaderboardName));
			return resultAsync;
		}

		internal async Task<LeaderboardFindResult_t?> FindOrCreateLeaderboard(string pchLeaderboardName, LeaderboardSort eLeaderboardSortMethod, LeaderboardDisplay eLeaderboardDisplayType)
		{
			LeaderboardFindResult_t? resultAsync = await LeaderboardFindResult_t.GetResultAsync(this._FindOrCreateLeaderboard(this.Self, pchLeaderboardName, eLeaderboardSortMethod, eLeaderboardDisplayType));
			return resultAsync;
		}

		internal bool GetAchievement(string pchName, ref bool pbAchieved)
		{
			return this._GetAchievement(this.Self, pchName, ref pbAchieved);
		}

		internal bool GetAchievementAchievedPercent(string pchName, ref float pflPercent)
		{
			return this._GetAchievementAchievedPercent(this.Self, pchName, ref pflPercent);
		}

		internal bool GetAchievementAndUnlockTime(string pchName, ref bool pbAchieved, ref uint punUnlockTime)
		{
			return this._GetAchievementAndUnlockTime(this.Self, pchName, ref pbAchieved, ref punUnlockTime);
		}

		internal string GetAchievementDisplayAttribute(string pchName, string pchKey)
		{
			string str = base.GetString(this._GetAchievementDisplayAttribute(this.Self, pchName, pchKey));
			return str;
		}

		internal int GetAchievementIcon(string pchName)
		{
			return this._GetAchievementIcon(this.Self, pchName);
		}

		internal string GetAchievementName(uint iAchievement)
		{
			string str = base.GetString(this._GetAchievementName(this.Self, iAchievement));
			return str;
		}

		internal bool GetDownloadedLeaderboardEntry(SteamLeaderboardEntries_t hSteamLeaderboardEntries, int index, ref LeaderboardEntry_t pLeaderboardEntry, [In][Out] int[] pDetails, int cDetailsMax)
		{
			bool self = this._GetDownloadedLeaderboardEntry(this.Self, hSteamLeaderboardEntries, index, ref pLeaderboardEntry, pDetails, cDetailsMax);
			return self;
		}

		internal bool GetGlobalStat1(string pchStatName, ref long pData)
		{
			return this._GetGlobalStat1(this.Self, pchStatName, ref pData);
		}

		internal bool GetGlobalStat2(string pchStatName, ref double pData)
		{
			return this._GetGlobalStat2(this.Self, pchStatName, ref pData);
		}

		internal int GetGlobalStatHistory1(string pchStatName, [In][Out] long[] pData, uint cubData)
		{
			return this._GetGlobalStatHistory1(this.Self, pchStatName, pData, cubData);
		}

		internal int GetGlobalStatHistory2(string pchStatName, [In][Out] double[] pData, uint cubData)
		{
			return this._GetGlobalStatHistory2(this.Self, pchStatName, pData, cubData);
		}

		internal LeaderboardDisplay GetLeaderboardDisplayType(SteamLeaderboard_t hSteamLeaderboard)
		{
			return this._GetLeaderboardDisplayType(this.Self, hSteamLeaderboard);
		}

		internal int GetLeaderboardEntryCount(SteamLeaderboard_t hSteamLeaderboard)
		{
			return this._GetLeaderboardEntryCount(this.Self, hSteamLeaderboard);
		}

		internal string GetLeaderboardName(SteamLeaderboard_t hSteamLeaderboard)
		{
			string str = base.GetString(this._GetLeaderboardName(this.Self, hSteamLeaderboard));
			return str;
		}

		internal LeaderboardSort GetLeaderboardSortMethod(SteamLeaderboard_t hSteamLeaderboard)
		{
			return this._GetLeaderboardSortMethod(this.Self, hSteamLeaderboard);
		}

		internal int GetMostAchievedAchievementInfo(StringBuilder pchName, uint unNameBufLen, ref float pflPercent, ref bool pbAchieved)
		{
			int self = this._GetMostAchievedAchievementInfo(this.Self, pchName, unNameBufLen, ref pflPercent, ref pbAchieved);
			return self;
		}

		internal int GetNextMostAchievedAchievementInfo(int iIteratorPrevious, StringBuilder pchName, uint unNameBufLen, ref float pflPercent, ref bool pbAchieved)
		{
			int self = this._GetNextMostAchievedAchievementInfo(this.Self, iIteratorPrevious, pchName, unNameBufLen, ref pflPercent, ref pbAchieved);
			return self;
		}

		internal uint GetNumAchievements()
		{
			return this._GetNumAchievements(this.Self);
		}

		internal async Task<NumberOfCurrentPlayers_t?> GetNumberOfCurrentPlayers()
		{
			NumberOfCurrentPlayers_t? resultAsync = await NumberOfCurrentPlayers_t.GetResultAsync(this._GetNumberOfCurrentPlayers(this.Self));
			return resultAsync;
		}

		internal bool GetStat1(string pchName, ref int pData)
		{
			return this._GetStat1(this.Self, pchName, ref pData);
		}

		internal bool GetStat2(string pchName, ref float pData)
		{
			return this._GetStat2(this.Self, pchName, ref pData);
		}

		internal bool GetUserAchievement(SteamId steamIDUser, string pchName, ref bool pbAchieved)
		{
			return this._GetUserAchievement(this.Self, steamIDUser, pchName, ref pbAchieved);
		}

		internal bool GetUserAchievementAndUnlockTime(SteamId steamIDUser, string pchName, ref bool pbAchieved, ref uint punUnlockTime)
		{
			bool self = this._GetUserAchievementAndUnlockTime(this.Self, steamIDUser, pchName, ref pbAchieved, ref punUnlockTime);
			return self;
		}

		internal bool GetUserStat1(SteamId steamIDUser, string pchName, ref int pData)
		{
			return this._GetUserStat1(this.Self, steamIDUser, pchName, ref pData);
		}

		internal bool GetUserStat2(SteamId steamIDUser, string pchName, ref float pData)
		{
			return this._GetUserStat2(this.Self, steamIDUser, pchName, ref pData);
		}

		internal bool IndicateAchievementProgress(string pchName, uint nCurProgress, uint nMaxProgress)
		{
			return this._IndicateAchievementProgress(this.Self, pchName, nCurProgress, nMaxProgress);
		}

		public override void InitInternals()
		{
			this._RequestCurrentStats = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FRequestCurrentStats>(Marshal.ReadIntPtr(this.VTable, 0));
			this._GetStat1 = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FGetStat1>(Marshal.ReadIntPtr(this.VTable, (Config.Os == OsType.Windows ? 16 : 8)));
			this._GetStat2 = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FGetStat2>(Marshal.ReadIntPtr(this.VTable, (Config.Os == OsType.Windows ? 8 : 16)));
			this._SetStat1 = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FSetStat1>(Marshal.ReadIntPtr(this.VTable, (Config.Os == OsType.Windows ? 32 : 24)));
			this._SetStat2 = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FSetStat2>(Marshal.ReadIntPtr(this.VTable, (Config.Os == OsType.Windows ? 24 : 32)));
			this._UpdateAvgRateStat = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FUpdateAvgRateStat>(Marshal.ReadIntPtr(this.VTable, 40));
			this._GetAchievement = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FGetAchievement>(Marshal.ReadIntPtr(this.VTable, 48));
			this._SetAchievement = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FSetAchievement>(Marshal.ReadIntPtr(this.VTable, 56));
			this._ClearAchievement = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FClearAchievement>(Marshal.ReadIntPtr(this.VTable, 64));
			this._GetAchievementAndUnlockTime = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FGetAchievementAndUnlockTime>(Marshal.ReadIntPtr(this.VTable, 72));
			this._StoreStats = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FStoreStats>(Marshal.ReadIntPtr(this.VTable, 80));
			this._GetAchievementIcon = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FGetAchievementIcon>(Marshal.ReadIntPtr(this.VTable, 88));
			this._GetAchievementDisplayAttribute = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FGetAchievementDisplayAttribute>(Marshal.ReadIntPtr(this.VTable, 96));
			this._IndicateAchievementProgress = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FIndicateAchievementProgress>(Marshal.ReadIntPtr(this.VTable, 104));
			this._GetNumAchievements = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FGetNumAchievements>(Marshal.ReadIntPtr(this.VTable, 112));
			this._GetAchievementName = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FGetAchievementName>(Marshal.ReadIntPtr(this.VTable, 120));
			this._RequestUserStats = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FRequestUserStats>(Marshal.ReadIntPtr(this.VTable, 128));
			this._GetUserStat1 = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FGetUserStat1>(Marshal.ReadIntPtr(this.VTable, (Config.Os == OsType.Windows ? 144 : 136)));
			this._GetUserStat2 = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FGetUserStat2>(Marshal.ReadIntPtr(this.VTable, (Config.Os == OsType.Windows ? 136 : 144)));
			this._GetUserAchievement = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FGetUserAchievement>(Marshal.ReadIntPtr(this.VTable, 152));
			this._GetUserAchievementAndUnlockTime = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FGetUserAchievementAndUnlockTime>(Marshal.ReadIntPtr(this.VTable, 160));
			this._ResetAllStats = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FResetAllStats>(Marshal.ReadIntPtr(this.VTable, 168));
			this._FindOrCreateLeaderboard = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FFindOrCreateLeaderboard>(Marshal.ReadIntPtr(this.VTable, 176));
			this._FindLeaderboard = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FFindLeaderboard>(Marshal.ReadIntPtr(this.VTable, 184));
			this._GetLeaderboardName = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FGetLeaderboardName>(Marshal.ReadIntPtr(this.VTable, 192));
			this._GetLeaderboardEntryCount = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FGetLeaderboardEntryCount>(Marshal.ReadIntPtr(this.VTable, 200));
			this._GetLeaderboardSortMethod = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FGetLeaderboardSortMethod>(Marshal.ReadIntPtr(this.VTable, 208));
			this._GetLeaderboardDisplayType = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FGetLeaderboardDisplayType>(Marshal.ReadIntPtr(this.VTable, 216));
			this._DownloadLeaderboardEntries = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FDownloadLeaderboardEntries>(Marshal.ReadIntPtr(this.VTable, 224));
			this._DownloadLeaderboardEntriesForUsers = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FDownloadLeaderboardEntriesForUsers>(Marshal.ReadIntPtr(this.VTable, 232));
			this._GetDownloadedLeaderboardEntry = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FGetDownloadedLeaderboardEntry>(Marshal.ReadIntPtr(this.VTable, 240));
			this._UploadLeaderboardScore = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FUploadLeaderboardScore>(Marshal.ReadIntPtr(this.VTable, 248));
			this._AttachLeaderboardUGC = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FAttachLeaderboardUGC>(Marshal.ReadIntPtr(this.VTable, 256));
			this._GetNumberOfCurrentPlayers = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FGetNumberOfCurrentPlayers>(Marshal.ReadIntPtr(this.VTable, 264));
			this._RequestGlobalAchievementPercentages = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FRequestGlobalAchievementPercentages>(Marshal.ReadIntPtr(this.VTable, 272));
			this._GetMostAchievedAchievementInfo = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FGetMostAchievedAchievementInfo>(Marshal.ReadIntPtr(this.VTable, 280));
			this._GetNextMostAchievedAchievementInfo = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FGetNextMostAchievedAchievementInfo>(Marshal.ReadIntPtr(this.VTable, 288));
			this._GetAchievementAchievedPercent = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FGetAchievementAchievedPercent>(Marshal.ReadIntPtr(this.VTable, 296));
			this._RequestGlobalStats = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FRequestGlobalStats>(Marshal.ReadIntPtr(this.VTable, 304));
			this._GetGlobalStat1 = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FGetGlobalStat1>(Marshal.ReadIntPtr(this.VTable, (Config.Os == OsType.Windows ? 320 : 312)));
			this._GetGlobalStat2 = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FGetGlobalStat2>(Marshal.ReadIntPtr(this.VTable, (Config.Os == OsType.Windows ? 312 : 320)));
			this._GetGlobalStatHistory1 = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FGetGlobalStatHistory1>(Marshal.ReadIntPtr(this.VTable, (Config.Os == OsType.Windows ? 336 : 328)));
			this._GetGlobalStatHistory2 = Marshal.GetDelegateForFunctionPointer<ISteamUserStats.FGetGlobalStatHistory2>(Marshal.ReadIntPtr(this.VTable, (Config.Os == OsType.Windows ? 328 : 336)));
		}

		internal bool RequestCurrentStats()
		{
			return this._RequestCurrentStats(this.Self);
		}

		internal async Task<GlobalAchievementPercentagesReady_t?> RequestGlobalAchievementPercentages()
		{
			GlobalAchievementPercentagesReady_t? resultAsync = await GlobalAchievementPercentagesReady_t.GetResultAsync(this._RequestGlobalAchievementPercentages(this.Self));
			return resultAsync;
		}

		internal async Task<GlobalStatsReceived_t?> RequestGlobalStats(int nHistoryDays)
		{
			GlobalStatsReceived_t? resultAsync = await GlobalStatsReceived_t.GetResultAsync(this._RequestGlobalStats(this.Self, nHistoryDays));
			return resultAsync;
		}

		internal async Task<UserStatsReceived_t?> RequestUserStats(SteamId steamIDUser)
		{
			UserStatsReceived_t? resultAsync = await UserStatsReceived_t.GetResultAsync(this._RequestUserStats(this.Self, steamIDUser));
			return resultAsync;
		}

		internal bool ResetAllStats(bool bAchievementsToo)
		{
			return this._ResetAllStats(this.Self, bAchievementsToo);
		}

		internal bool SetAchievement(string pchName)
		{
			return this._SetAchievement(this.Self, pchName);
		}

		internal bool SetStat1(string pchName, int nData)
		{
			return this._SetStat1(this.Self, pchName, nData);
		}

		internal bool SetStat2(string pchName, float fData)
		{
			return this._SetStat2(this.Self, pchName, fData);
		}

		internal override void Shutdown()
		{
			base.Shutdown();
			this._RequestCurrentStats = null;
			this._GetStat1 = null;
			this._GetStat2 = null;
			this._SetStat1 = null;
			this._SetStat2 = null;
			this._UpdateAvgRateStat = null;
			this._GetAchievement = null;
			this._SetAchievement = null;
			this._ClearAchievement = null;
			this._GetAchievementAndUnlockTime = null;
			this._StoreStats = null;
			this._GetAchievementIcon = null;
			this._GetAchievementDisplayAttribute = null;
			this._IndicateAchievementProgress = null;
			this._GetNumAchievements = null;
			this._GetAchievementName = null;
			this._RequestUserStats = null;
			this._GetUserStat1 = null;
			this._GetUserStat2 = null;
			this._GetUserAchievement = null;
			this._GetUserAchievementAndUnlockTime = null;
			this._ResetAllStats = null;
			this._FindOrCreateLeaderboard = null;
			this._FindLeaderboard = null;
			this._GetLeaderboardName = null;
			this._GetLeaderboardEntryCount = null;
			this._GetLeaderboardSortMethod = null;
			this._GetLeaderboardDisplayType = null;
			this._DownloadLeaderboardEntries = null;
			this._DownloadLeaderboardEntriesForUsers = null;
			this._GetDownloadedLeaderboardEntry = null;
			this._UploadLeaderboardScore = null;
			this._AttachLeaderboardUGC = null;
			this._GetNumberOfCurrentPlayers = null;
			this._RequestGlobalAchievementPercentages = null;
			this._GetMostAchievedAchievementInfo = null;
			this._GetNextMostAchievedAchievementInfo = null;
			this._GetAchievementAchievedPercent = null;
			this._RequestGlobalStats = null;
			this._GetGlobalStat1 = null;
			this._GetGlobalStat2 = null;
			this._GetGlobalStatHistory1 = null;
			this._GetGlobalStatHistory2 = null;
		}

		internal bool StoreStats()
		{
			return this._StoreStats(this.Self);
		}

		internal bool UpdateAvgRateStat(string pchName, float flCountThisSession, double dSessionLength)
		{
			return this._UpdateAvgRateStat(this.Self, pchName, flCountThisSession, dSessionLength);
		}

		internal async Task<LeaderboardScoreUploaded_t?> UploadLeaderboardScore(SteamLeaderboard_t hSteamLeaderboard, LeaderboardUploadScoreMethod eLeaderboardUploadScoreMethod, int nScore, [In][Out] int[] pScoreDetails, int cScoreDetailsCount)
		{
			LeaderboardScoreUploaded_t? resultAsync = await LeaderboardScoreUploaded_t.GetResultAsync(this._UploadLeaderboardScore(this.Self, hSteamLeaderboard, eLeaderboardUploadScoreMethod, nScore, pScoreDetails, cScoreDetailsCount));
			return resultAsync;
		}

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FAttachLeaderboardUGC(IntPtr self, SteamLeaderboard_t hSteamLeaderboard, UGCHandle_t hUGC);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FClearAchievement(IntPtr self, string pchName);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FDownloadLeaderboardEntries(IntPtr self, SteamLeaderboard_t hSteamLeaderboard, LeaderboardDataRequest eLeaderboardDataRequest, int nRangeStart, int nRangeEnd);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FDownloadLeaderboardEntriesForUsers(IntPtr self, SteamLeaderboard_t hSteamLeaderboard, [In][Out] SteamId[] prgUsers, int cUsers);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FFindLeaderboard(IntPtr self, string pchLeaderboardName);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FFindOrCreateLeaderboard(IntPtr self, string pchLeaderboardName, LeaderboardSort eLeaderboardSortMethod, LeaderboardDisplay eLeaderboardDisplayType);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetAchievement(IntPtr self, string pchName, ref bool pbAchieved);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetAchievementAchievedPercent(IntPtr self, string pchName, ref float pflPercent);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetAchievementAndUnlockTime(IntPtr self, string pchName, ref bool pbAchieved, ref uint punUnlockTime);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate IntPtr FGetAchievementDisplayAttribute(IntPtr self, string pchName, string pchKey);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetAchievementIcon(IntPtr self, string pchName);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate IntPtr FGetAchievementName(IntPtr self, uint iAchievement);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetDownloadedLeaderboardEntry(IntPtr self, SteamLeaderboardEntries_t hSteamLeaderboardEntries, int index, ref LeaderboardEntry_t pLeaderboardEntry, [In][Out] int[] pDetails, int cDetailsMax);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetGlobalStat1(IntPtr self, string pchStatName, ref long pData);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetGlobalStat2(IntPtr self, string pchStatName, ref double pData);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetGlobalStatHistory1(IntPtr self, string pchStatName, [In][Out] long[] pData, uint cubData);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetGlobalStatHistory2(IntPtr self, string pchStatName, [In][Out] double[] pData, uint cubData);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate LeaderboardDisplay FGetLeaderboardDisplayType(IntPtr self, SteamLeaderboard_t hSteamLeaderboard);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetLeaderboardEntryCount(IntPtr self, SteamLeaderboard_t hSteamLeaderboard);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate IntPtr FGetLeaderboardName(IntPtr self, SteamLeaderboard_t hSteamLeaderboard);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate LeaderboardSort FGetLeaderboardSortMethod(IntPtr self, SteamLeaderboard_t hSteamLeaderboard);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetMostAchievedAchievementInfo(IntPtr self, StringBuilder pchName, uint unNameBufLen, ref float pflPercent, ref bool pbAchieved);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetNextMostAchievedAchievementInfo(IntPtr self, int iIteratorPrevious, StringBuilder pchName, uint unNameBufLen, ref float pflPercent, ref bool pbAchieved);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate uint FGetNumAchievements(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FGetNumberOfCurrentPlayers(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetStat1(IntPtr self, string pchName, ref int pData);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetStat2(IntPtr self, string pchName, ref float pData);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetUserAchievement(IntPtr self, SteamId steamIDUser, string pchName, ref bool pbAchieved);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetUserAchievementAndUnlockTime(IntPtr self, SteamId steamIDUser, string pchName, ref bool pbAchieved, ref uint punUnlockTime);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetUserStat1(IntPtr self, SteamId steamIDUser, string pchName, ref int pData);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetUserStat2(IntPtr self, SteamId steamIDUser, string pchName, ref float pData);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FIndicateAchievementProgress(IntPtr self, string pchName, uint nCurProgress, uint nMaxProgress);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FRequestCurrentStats(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FRequestGlobalAchievementPercentages(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FRequestGlobalStats(IntPtr self, int nHistoryDays);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FRequestUserStats(IntPtr self, SteamId steamIDUser);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FResetAllStats(IntPtr self, bool bAchievementsToo);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetAchievement(IntPtr self, string pchName);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetStat1(IntPtr self, string pchName, int nData);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetStat2(IntPtr self, string pchName, float fData);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FStoreStats(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FUpdateAvgRateStat(IntPtr self, string pchName, float flCountThisSession, double dSessionLength);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FUploadLeaderboardScore(IntPtr self, SteamLeaderboard_t hSteamLeaderboard, LeaderboardUploadScoreMethod eLeaderboardUploadScoreMethod, int nScore, [In][Out] int[] pScoreDetails, int cScoreDetailsCount);
	}
}