using Facepunch.Steamworks;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SteamNative
{
	internal class SteamApps : IDisposable
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

		internal SteamApps(BaseSteamworks steamworks, IntPtr pointer)
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

		public bool BGetDLCDataByIndex(int iDLC, ref AppId_t pAppID, ref bool pbAvailable, out string pchName)
		{
			bool flag = false;
			pchName = string.Empty;
			StringBuilder stringBuilder = Helpers.TakeStringBuilder();
			int num = 4096;
			flag = this.platform.ISteamApps_BGetDLCDataByIndex(iDLC, ref pAppID.Value, ref pbAvailable, stringBuilder, num);
			if (!flag)
			{
				return flag;
			}
			pchName = stringBuilder.ToString();
			return flag;
		}

		public bool BIsAppInstalled(AppId_t appID)
		{
			return this.platform.ISteamApps_BIsAppInstalled(appID.Value);
		}

		public bool BIsCybercafe()
		{
			return this.platform.ISteamApps_BIsCybercafe();
		}

		public bool BIsDlcInstalled(AppId_t appID)
		{
			return this.platform.ISteamApps_BIsDlcInstalled(appID.Value);
		}

		public bool BIsLowViolence()
		{
			return this.platform.ISteamApps_BIsLowViolence();
		}

		public bool BIsSubscribed()
		{
			return this.platform.ISteamApps_BIsSubscribed();
		}

		public bool BIsSubscribedApp(AppId_t appID)
		{
			return this.platform.ISteamApps_BIsSubscribedApp(appID.Value);
		}

		public bool BIsSubscribedFromFreeWeekend()
		{
			return this.platform.ISteamApps_BIsSubscribedFromFreeWeekend();
		}

		public bool BIsVACBanned()
		{
			return this.platform.ISteamApps_BIsVACBanned();
		}

		public virtual void Dispose()
		{
			if (this.platform != null)
			{
				this.platform.Dispose();
				this.platform = null;
			}
		}

		public int GetAppBuildId()
		{
			return this.platform.ISteamApps_GetAppBuildId();
		}

		public string GetAppInstallDir(AppId_t appID)
		{
			StringBuilder stringBuilder = Helpers.TakeStringBuilder();
			if (this.platform.ISteamApps_GetAppInstallDir(appID.Value, stringBuilder, 4096) <= 0)
			{
				return null;
			}
			return stringBuilder.ToString();
		}

		public ulong GetAppOwner()
		{
			return this.platform.ISteamApps_GetAppOwner();
		}

		public string GetAvailableGameLanguages()
		{
			return Marshal.PtrToStringAnsi(this.platform.ISteamApps_GetAvailableGameLanguages());
		}

		public string GetCurrentBetaName()
		{
			StringBuilder stringBuilder = Helpers.TakeStringBuilder();
			if (!this.platform.ISteamApps_GetCurrentBetaName(stringBuilder, 4096))
			{
				return null;
			}
			return stringBuilder.ToString();
		}

		public string GetCurrentGameLanguage()
		{
			return Marshal.PtrToStringAnsi(this.platform.ISteamApps_GetCurrentGameLanguage());
		}

		public int GetDLCCount()
		{
			return this.platform.ISteamApps_GetDLCCount();
		}

		public bool GetDlcDownloadProgress(AppId_t nAppID, out ulong punBytesDownloaded, out ulong punBytesTotal)
		{
			return this.platform.ISteamApps_GetDlcDownloadProgress(nAppID.Value, out punBytesDownloaded, out punBytesTotal);
		}

		public uint GetEarliestPurchaseUnixTime(AppId_t nAppID)
		{
			return this.platform.ISteamApps_GetEarliestPurchaseUnixTime(nAppID.Value);
		}

		public CallbackHandle GetFileDetails(string pszFileName, Action<FileDetailsResult_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamApps_GetFileDetails(pszFileName);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return FileDetailsResult_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public uint GetInstalledDepots(AppId_t appID, IntPtr pvecDepots, uint cMaxDepots)
		{
			return this.platform.ISteamApps_GetInstalledDepots(appID.Value, pvecDepots, cMaxDepots);
		}

		public string GetLaunchQueryParam(string pchKey)
		{
			return Marshal.PtrToStringAnsi(this.platform.ISteamApps_GetLaunchQueryParam(pchKey));
		}

		public void InstallDLC(AppId_t nAppID)
		{
			this.platform.ISteamApps_InstallDLC(nAppID.Value);
		}

		public bool MarkContentCorrupt(bool bMissingFilesOnly)
		{
			return this.platform.ISteamApps_MarkContentCorrupt(bMissingFilesOnly);
		}

		public void RequestAllProofOfPurchaseKeys()
		{
			this.platform.ISteamApps_RequestAllProofOfPurchaseKeys();
		}

		public void RequestAppProofOfPurchaseKey(AppId_t nAppID)
		{
			this.platform.ISteamApps_RequestAppProofOfPurchaseKey(nAppID.Value);
		}

		public void UninstallDLC(AppId_t nAppID)
		{
			this.platform.ISteamApps_UninstallDLC(nAppID.Value);
		}
	}
}