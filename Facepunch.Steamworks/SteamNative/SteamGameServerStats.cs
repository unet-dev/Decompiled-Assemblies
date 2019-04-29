using Facepunch.Steamworks;
using System;

namespace SteamNative
{
	internal class SteamGameServerStats : IDisposable
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

		internal SteamGameServerStats(BaseSteamworks steamworks, IntPtr pointer)
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

		public bool ClearUserAchievement(CSteamID steamIDUser, string pchName)
		{
			return this.platform.ISteamGameServerStats_ClearUserAchievement(steamIDUser.Value, pchName);
		}

		public virtual void Dispose()
		{
			if (this.platform != null)
			{
				this.platform.Dispose();
				this.platform = null;
			}
		}

		public bool GetUserAchievement(CSteamID steamIDUser, string pchName, ref bool pbAchieved)
		{
			return this.platform.ISteamGameServerStats_GetUserAchievement(steamIDUser.Value, pchName, ref pbAchieved);
		}

		public bool GetUserStat(CSteamID steamIDUser, string pchName, out int pData)
		{
			return this.platform.ISteamGameServerStats_GetUserStat(steamIDUser.Value, pchName, out pData);
		}

		public bool GetUserStat0(CSteamID steamIDUser, string pchName, out float pData)
		{
			return this.platform.ISteamGameServerStats_GetUserStat0(steamIDUser.Value, pchName, out pData);
		}

		public CallbackHandle RequestUserStats(CSteamID steamIDUser, Action<GSStatsReceived_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamGameServerStats_RequestUserStats(steamIDUser.Value);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return GSStatsReceived_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public bool SetUserAchievement(CSteamID steamIDUser, string pchName)
		{
			return this.platform.ISteamGameServerStats_SetUserAchievement(steamIDUser.Value, pchName);
		}

		public bool SetUserStat(CSteamID steamIDUser, string pchName, int nData)
		{
			return this.platform.ISteamGameServerStats_SetUserStat(steamIDUser.Value, pchName, nData);
		}

		public bool SetUserStat0(CSteamID steamIDUser, string pchName, float fData)
		{
			return this.platform.ISteamGameServerStats_SetUserStat0(steamIDUser.Value, pchName, fData);
		}

		public CallbackHandle StoreUserStats(CSteamID steamIDUser, Action<GSStatsStored_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamGameServerStats_StoreUserStats(steamIDUser.Value);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return GSStatsStored_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public bool UpdateUserAvgRateStat(CSteamID steamIDUser, string pchName, float flCountThisSession, double dSessionLength)
		{
			return this.platform.ISteamGameServerStats_UpdateUserAvgRateStat(steamIDUser.Value, pchName, flCountThisSession, dSessionLength);
		}
	}
}