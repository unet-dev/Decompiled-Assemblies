using Facepunch.Steamworks;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SteamNative
{
	internal class SteamUserStats : IDisposable
	{
		internal Platform.Interface platform;

		internal BaseSteamworks steamworks;

		public bool IsValid
		{
			get
			{
				if (this.platform == null)
				{
					return false;
				}
				return this.platform.IsValid;
			}
		}

		internal SteamUserStats(BaseSteamworks steamworks, IntPtr pointer)
		{
			this.steamworks = steamworks;
			if (Platform.IsWindows64)
			{
				this.platform = new Platform.Win64(pointer);
				return;
			}
			if (Platform.IsWindows32)
			{
				this.platform = new Platform.Win32(pointer);
				return;
			}
			if (Platform.IsLinux32)
			{
				this.platform = new Platform.Linux32(pointer);
				return;
			}
			if (Platform.IsLinux64)
			{
				this.platform = new Platform.Linux64(pointer);
				return;
			}
			if (Platform.IsOsx)
			{
				this.platform = new Platform.Mac(pointer);
			}
		}

		public CallbackHandle AttachLeaderboardUGC(SteamLeaderboard_t hSteamLeaderboard, UGCHandle_t hUGC, Action<LeaderboardUGCSet_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamUserStats_AttachLeaderboardUGC(hSteamLeaderboard.Value, hUGC.Value);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return LeaderboardUGCSet_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public bool ClearAchievement(string pchName)
		{
			return this.platform.ISteamUserStats_ClearAchievement(pchName);
		}

		public virtual void Dispose()
		{
			if (this.platform != null)
			{
				this.platform.Dispose();
				this.platform = null;
			}
		}

		public CallbackHandle DownloadLeaderboardEntries(SteamLeaderboard_t hSteamLeaderboard, LeaderboardDataRequest eLeaderboardDataRequest, int nRangeStart, int nRangeEnd, Action<LeaderboardScoresDownloaded_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamUserStats_DownloadLeaderboardEntries(hSteamLeaderboard.Value, eLeaderboardDataRequest, nRangeStart, nRangeEnd);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return LeaderboardScoresDownloaded_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public CallbackHandle DownloadLeaderboardEntriesForUsers(SteamLeaderboard_t hSteamLeaderboard, IntPtr prgUsers, int cUsers, Action<LeaderboardScoresDownloaded_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamUserStats_DownloadLeaderboardEntriesForUsers(hSteamLeaderboard.Value, prgUsers, cUsers);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return LeaderboardScoresDownloaded_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public CallbackHandle FindLeaderboard(string pchLeaderboardName, Action<LeaderboardFindResult_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamUserStats_FindLeaderboard(pchLeaderboardName);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return LeaderboardFindResult_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public CallbackHandle FindOrCreateLeaderboard(string pchLeaderboardName, LeaderboardSortMethod eLeaderboardSortMethod, LeaderboardDisplayType eLeaderboardDisplayType, Action<LeaderboardFindResult_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamUserStats_FindOrCreateLeaderboard(pchLeaderboardName, eLeaderboardSortMethod, eLeaderboardDisplayType);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return LeaderboardFindResult_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public bool GetAchievement(string pchName, ref bool pbAchieved)
		{
			return this.platform.ISteamUserStats_GetAchievement(pchName, ref pbAchieved);
		}

		public bool GetAchievementAchievedPercent(string pchName, out float pflPercent)
		{
			return this.platform.ISteamUserStats_GetAchievementAchievedPercent(pchName, out pflPercent);
		}

		public bool GetAchievementAndUnlockTime(string pchName, ref bool pbAchieved, out uint punUnlockTime)
		{
			return this.platform.ISteamUserStats_GetAchievementAndUnlockTime(pchName, ref pbAchieved, out punUnlockTime);
		}

		public string GetAchievementDisplayAttribute(string pchName, string pchKey)
		{
			return Marshal.PtrToStringAnsi(this.platform.ISteamUserStats_GetAchievementDisplayAttribute(pchName, pchKey));
		}

		public int GetAchievementIcon(string pchName)
		{
			return this.platform.ISteamUserStats_GetAchievementIcon(pchName);
		}

		public string GetAchievementName(uint iAchievement)
		{
			return Marshal.PtrToStringAnsi(this.platform.ISteamUserStats_GetAchievementName(iAchievement));
		}

		public bool GetDownloadedLeaderboardEntry(SteamLeaderboardEntries_t hSteamLeaderboardEntries, int index, ref LeaderboardEntry_t pLeaderboardEntry, IntPtr pDetails, int cDetailsMax)
		{
			return this.platform.ISteamUserStats_GetDownloadedLeaderboardEntry(hSteamLeaderboardEntries.Value, index, ref pLeaderboardEntry, pDetails, cDetailsMax);
		}

		public bool GetGlobalStat(string pchStatName, out long pData)
		{
			return this.platform.ISteamUserStats_GetGlobalStat(pchStatName, out pData);
		}

		public bool GetGlobalStat0(string pchStatName, out double pData)
		{
			return this.platform.ISteamUserStats_GetGlobalStat0(pchStatName, out pData);
		}

		public int GetGlobalStatHistory(string pchStatName, out long pData, uint cubData)
		{
			return this.platform.ISteamUserStats_GetGlobalStatHistory(pchStatName, out pData, cubData);
		}

		public int GetGlobalStatHistory0(string pchStatName, out double pData, uint cubData)
		{
			return this.platform.ISteamUserStats_GetGlobalStatHistory0(pchStatName, out pData, cubData);
		}

		public LeaderboardDisplayType GetLeaderboardDisplayType(SteamLeaderboard_t hSteamLeaderboard)
		{
			return this.platform.ISteamUserStats_GetLeaderboardDisplayType(hSteamLeaderboard.Value);
		}

		public int GetLeaderboardEntryCount(SteamLeaderboard_t hSteamLeaderboard)
		{
			return this.platform.ISteamUserStats_GetLeaderboardEntryCount(hSteamLeaderboard.Value);
		}

		public string GetLeaderboardName(SteamLeaderboard_t hSteamLeaderboard)
		{
			return Marshal.PtrToStringAnsi(this.platform.ISteamUserStats_GetLeaderboardName(hSteamLeaderboard.Value));
		}

		public LeaderboardSortMethod GetLeaderboardSortMethod(SteamLeaderboard_t hSteamLeaderboard)
		{
			return this.platform.ISteamUserStats_GetLeaderboardSortMethod(hSteamLeaderboard.Value);
		}

		public int GetMostAchievedAchievementInfo(out string pchName, out float pflPercent, ref bool pbAchieved)
		{
			int num = 0;
			pchName = string.Empty;
			StringBuilder stringBuilder = Helpers.TakeStringBuilder();
			num = this.platform.ISteamUserStats_GetMostAchievedAchievementInfo(stringBuilder, 4096, out pflPercent, ref pbAchieved);
			if (num <= 0)
			{
				return num;
			}
			pchName = stringBuilder.ToString();
			return num;
		}

		public int GetNextMostAchievedAchievementInfo(int iIteratorPrevious, out string pchName, out float pflPercent, ref bool pbAchieved)
		{
			int num = 0;
			pchName = string.Empty;
			StringBuilder stringBuilder = Helpers.TakeStringBuilder();
			uint num1 = 4096;
			num = this.platform.ISteamUserStats_GetNextMostAchievedAchievementInfo(iIteratorPrevious, stringBuilder, num1, out pflPercent, ref pbAchieved);
			if (num <= 0)
			{
				return num;
			}
			pchName = stringBuilder.ToString();
			return num;
		}

		public uint GetNumAchievements()
		{
			return this.platform.ISteamUserStats_GetNumAchievements();
		}

		public CallbackHandle GetNumberOfCurrentPlayers(Action<NumberOfCurrentPlayers_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamUserStats_GetNumberOfCurrentPlayers();
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return NumberOfCurrentPlayers_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public bool GetStat(string pchName, out int pData)
		{
			return this.platform.ISteamUserStats_GetStat(pchName, out pData);
		}

		public bool GetStat0(string pchName, out float pData)
		{
			return this.platform.ISteamUserStats_GetStat0(pchName, out pData);
		}

		public bool GetUserAchievement(CSteamID steamIDUser, string pchName, ref bool pbAchieved)
		{
			return this.platform.ISteamUserStats_GetUserAchievement(steamIDUser.Value, pchName, ref pbAchieved);
		}

		public bool GetUserAchievementAndUnlockTime(CSteamID steamIDUser, string pchName, ref bool pbAchieved, out uint punUnlockTime)
		{
			return this.platform.ISteamUserStats_GetUserAchievementAndUnlockTime(steamIDUser.Value, pchName, ref pbAchieved, out punUnlockTime);
		}

		public bool GetUserStat(CSteamID steamIDUser, string pchName, out int pData)
		{
			return this.platform.ISteamUserStats_GetUserStat(steamIDUser.Value, pchName, out pData);
		}

		public bool GetUserStat0(CSteamID steamIDUser, string pchName, out float pData)
		{
			return this.platform.ISteamUserStats_GetUserStat0(steamIDUser.Value, pchName, out pData);
		}

		public bool IndicateAchievementProgress(string pchName, uint nCurProgress, uint nMaxProgress)
		{
			return this.platform.ISteamUserStats_IndicateAchievementProgress(pchName, nCurProgress, nMaxProgress);
		}

		public bool RequestCurrentStats()
		{
			return this.platform.ISteamUserStats_RequestCurrentStats();
		}

		public CallbackHandle RequestGlobalAchievementPercentages(Action<GlobalAchievementPercentagesReady_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamUserStats_RequestGlobalAchievementPercentages();
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return GlobalAchievementPercentagesReady_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public CallbackHandle RequestGlobalStats(int nHistoryDays, Action<GlobalStatsReceived_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamUserStats_RequestGlobalStats(nHistoryDays);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return GlobalStatsReceived_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public CallbackHandle RequestUserStats(CSteamID steamIDUser, Action<UserStatsReceived_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamUserStats_RequestUserStats(steamIDUser.Value);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return UserStatsReceived_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public bool ResetAllStats(bool bAchievementsToo)
		{
			return this.platform.ISteamUserStats_ResetAllStats(bAchievementsToo);
		}

		public bool SetAchievement(string pchName)
		{
			return this.platform.ISteamUserStats_SetAchievement(pchName);
		}

		public bool SetStat(string pchName, int nData)
		{
			return this.platform.ISteamUserStats_SetStat(pchName, nData);
		}

		public bool SetStat0(string pchName, float fData)
		{
			return this.platform.ISteamUserStats_SetStat0(pchName, fData);
		}

		public bool StoreStats()
		{
			return this.platform.ISteamUserStats_StoreStats();
		}

		public bool UpdateAvgRateStat(string pchName, float flCountThisSession, double dSessionLength)
		{
			return this.platform.ISteamUserStats_UpdateAvgRateStat(pchName, flCountThisSession, dSessionLength);
		}

		public CallbackHandle UploadLeaderboardScore(SteamLeaderboard_t hSteamLeaderboard, LeaderboardUploadScoreMethod eLeaderboardUploadScoreMethod, int nScore, int[] pScoreDetails, int cScoreDetailsCount, Action<LeaderboardScoreUploaded_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamUserStats_UploadLeaderboardScore(hSteamLeaderboard.Value, eLeaderboardUploadScoreMethod, nScore, pScoreDetails, cScoreDetailsCount);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return LeaderboardScoreUploaded_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}
	}
}