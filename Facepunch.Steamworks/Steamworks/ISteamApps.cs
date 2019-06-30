using Steamworks.Data;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Steamworks
{
	internal class ISteamApps : SteamInterface
	{
		private ISteamApps.FBIsSubscribed _BIsSubscribed;

		private ISteamApps.FBIsLowViolence _BIsLowViolence;

		private ISteamApps.FBIsCybercafe _BIsCybercafe;

		private ISteamApps.FBIsVACBanned _BIsVACBanned;

		private ISteamApps.FGetCurrentGameLanguage _GetCurrentGameLanguage;

		private ISteamApps.FGetAvailableGameLanguages _GetAvailableGameLanguages;

		private ISteamApps.FBIsSubscribedApp _BIsSubscribedApp;

		private ISteamApps.FBIsDlcInstalled _BIsDlcInstalled;

		private ISteamApps.FGetEarliestPurchaseUnixTime _GetEarliestPurchaseUnixTime;

		private ISteamApps.FBIsSubscribedFromFreeWeekend _BIsSubscribedFromFreeWeekend;

		private ISteamApps.FGetDLCCount _GetDLCCount;

		private ISteamApps.FBGetDLCDataByIndex _BGetDLCDataByIndex;

		private ISteamApps.FInstallDLC _InstallDLC;

		private ISteamApps.FUninstallDLC _UninstallDLC;

		private ISteamApps.FRequestAppProofOfPurchaseKey _RequestAppProofOfPurchaseKey;

		private ISteamApps.FGetCurrentBetaName _GetCurrentBetaName;

		private ISteamApps.FMarkContentCorrupt _MarkContentCorrupt;

		private ISteamApps.FGetInstalledDepots _GetInstalledDepots;

		private ISteamApps.FGetAppInstallDir _GetAppInstallDir;

		private ISteamApps.FBIsAppInstalled _BIsAppInstalled;

		private ISteamApps.FGetAppOwner _GetAppOwner;

		private ISteamApps.FGetAppOwner_Windows _GetAppOwner_Windows;

		private ISteamApps.FGetLaunchQueryParam _GetLaunchQueryParam;

		private ISteamApps.FGetDlcDownloadProgress _GetDlcDownloadProgress;

		private ISteamApps.FGetAppBuildId _GetAppBuildId;

		private ISteamApps.FRequestAllProofOfPurchaseKeys _RequestAllProofOfPurchaseKeys;

		private ISteamApps.FGetFileDetails _GetFileDetails;

		private ISteamApps.FGetLaunchCommandLine _GetLaunchCommandLine;

		private ISteamApps.FBIsSubscribedFromFamilySharing _BIsSubscribedFromFamilySharing;

		public override string InterfaceName
		{
			get
			{
				return "STEAMAPPS_INTERFACE_VERSION008";
			}
		}

		public ISteamApps()
		{
		}

		internal bool BGetDLCDataByIndex(int iDLC, ref AppId pAppID, ref bool pbAvailable, StringBuilder pchName, int cchNameBufferSize)
		{
			bool self = this._BGetDLCDataByIndex(this.Self, iDLC, ref pAppID, ref pbAvailable, pchName, cchNameBufferSize);
			return self;
		}

		internal bool BIsAppInstalled(AppId appID)
		{
			return this._BIsAppInstalled(this.Self, appID);
		}

		internal bool BIsCybercafe()
		{
			return this._BIsCybercafe(this.Self);
		}

		internal bool BIsDlcInstalled(AppId appID)
		{
			return this._BIsDlcInstalled(this.Self, appID);
		}

		internal bool BIsLowViolence()
		{
			return this._BIsLowViolence(this.Self);
		}

		internal bool BIsSubscribed()
		{
			return this._BIsSubscribed(this.Self);
		}

		internal bool BIsSubscribedApp(AppId appID)
		{
			return this._BIsSubscribedApp(this.Self, appID);
		}

		internal bool BIsSubscribedFromFamilySharing()
		{
			return this._BIsSubscribedFromFamilySharing(this.Self);
		}

		internal bool BIsSubscribedFromFreeWeekend()
		{
			return this._BIsSubscribedFromFreeWeekend(this.Self);
		}

		internal bool BIsVACBanned()
		{
			return this._BIsVACBanned(this.Self);
		}

		internal int GetAppBuildId()
		{
			return this._GetAppBuildId(this.Self);
		}

		internal uint GetAppInstallDir(AppId appID, StringBuilder pchFolder, uint cchFolderBufferSize)
		{
			return this._GetAppInstallDir(this.Self, appID, pchFolder, cchFolderBufferSize);
		}

		internal SteamId GetAppOwner()
		{
			SteamId self;
			if (Config.Os != OsType.Windows)
			{
				self = this._GetAppOwner(this.Self);
			}
			else
			{
				SteamId steamId = new SteamId();
				this._GetAppOwner_Windows(this.Self, ref steamId);
				self = steamId;
			}
			return self;
		}

		internal string GetAvailableGameLanguages()
		{
			return base.GetString(this._GetAvailableGameLanguages(this.Self));
		}

		internal bool GetCurrentBetaName(StringBuilder pchName, int cchNameBufferSize)
		{
			return this._GetCurrentBetaName(this.Self, pchName, cchNameBufferSize);
		}

		internal string GetCurrentGameLanguage()
		{
			return base.GetString(this._GetCurrentGameLanguage(this.Self));
		}

		internal int GetDLCCount()
		{
			return this._GetDLCCount(this.Self);
		}

		internal bool GetDlcDownloadProgress(AppId nAppID, ref ulong punBytesDownloaded, ref ulong punBytesTotal)
		{
			return this._GetDlcDownloadProgress(this.Self, nAppID, ref punBytesDownloaded, ref punBytesTotal);
		}

		internal uint GetEarliestPurchaseUnixTime(AppId nAppID)
		{
			return this._GetEarliestPurchaseUnixTime(this.Self, nAppID);
		}

		internal async Task<FileDetailsResult_t?> GetFileDetails(string pszFileName)
		{
			FileDetailsResult_t? resultAsync = await FileDetailsResult_t.GetResultAsync(this._GetFileDetails(this.Self, pszFileName));
			return resultAsync;
		}

		internal uint GetInstalledDepots(AppId appID, [In][Out] DepotId_t[] pvecDepots, uint cMaxDepots)
		{
			return this._GetInstalledDepots(this.Self, appID, pvecDepots, cMaxDepots);
		}

		internal int GetLaunchCommandLine(StringBuilder pszCommandLine, int cubCommandLine)
		{
			return this._GetLaunchCommandLine(this.Self, pszCommandLine, cubCommandLine);
		}

		internal string GetLaunchQueryParam(string pchKey)
		{
			string str = base.GetString(this._GetLaunchQueryParam(this.Self, pchKey));
			return str;
		}

		public override void InitInternals()
		{
			this._BIsSubscribed = Marshal.GetDelegateForFunctionPointer<ISteamApps.FBIsSubscribed>(Marshal.ReadIntPtr(this.VTable, 0));
			this._BIsLowViolence = Marshal.GetDelegateForFunctionPointer<ISteamApps.FBIsLowViolence>(Marshal.ReadIntPtr(this.VTable, 8));
			this._BIsCybercafe = Marshal.GetDelegateForFunctionPointer<ISteamApps.FBIsCybercafe>(Marshal.ReadIntPtr(this.VTable, 16));
			this._BIsVACBanned = Marshal.GetDelegateForFunctionPointer<ISteamApps.FBIsVACBanned>(Marshal.ReadIntPtr(this.VTable, 24));
			this._GetCurrentGameLanguage = Marshal.GetDelegateForFunctionPointer<ISteamApps.FGetCurrentGameLanguage>(Marshal.ReadIntPtr(this.VTable, 32));
			this._GetAvailableGameLanguages = Marshal.GetDelegateForFunctionPointer<ISteamApps.FGetAvailableGameLanguages>(Marshal.ReadIntPtr(this.VTable, 40));
			this._BIsSubscribedApp = Marshal.GetDelegateForFunctionPointer<ISteamApps.FBIsSubscribedApp>(Marshal.ReadIntPtr(this.VTable, 48));
			this._BIsDlcInstalled = Marshal.GetDelegateForFunctionPointer<ISteamApps.FBIsDlcInstalled>(Marshal.ReadIntPtr(this.VTable, 56));
			this._GetEarliestPurchaseUnixTime = Marshal.GetDelegateForFunctionPointer<ISteamApps.FGetEarliestPurchaseUnixTime>(Marshal.ReadIntPtr(this.VTable, 64));
			this._BIsSubscribedFromFreeWeekend = Marshal.GetDelegateForFunctionPointer<ISteamApps.FBIsSubscribedFromFreeWeekend>(Marshal.ReadIntPtr(this.VTable, 72));
			this._GetDLCCount = Marshal.GetDelegateForFunctionPointer<ISteamApps.FGetDLCCount>(Marshal.ReadIntPtr(this.VTable, 80));
			this._BGetDLCDataByIndex = Marshal.GetDelegateForFunctionPointer<ISteamApps.FBGetDLCDataByIndex>(Marshal.ReadIntPtr(this.VTable, 88));
			this._InstallDLC = Marshal.GetDelegateForFunctionPointer<ISteamApps.FInstallDLC>(Marshal.ReadIntPtr(this.VTable, 96));
			this._UninstallDLC = Marshal.GetDelegateForFunctionPointer<ISteamApps.FUninstallDLC>(Marshal.ReadIntPtr(this.VTable, 104));
			this._RequestAppProofOfPurchaseKey = Marshal.GetDelegateForFunctionPointer<ISteamApps.FRequestAppProofOfPurchaseKey>(Marshal.ReadIntPtr(this.VTable, 112));
			this._GetCurrentBetaName = Marshal.GetDelegateForFunctionPointer<ISteamApps.FGetCurrentBetaName>(Marshal.ReadIntPtr(this.VTable, 120));
			this._MarkContentCorrupt = Marshal.GetDelegateForFunctionPointer<ISteamApps.FMarkContentCorrupt>(Marshal.ReadIntPtr(this.VTable, 128));
			this._GetInstalledDepots = Marshal.GetDelegateForFunctionPointer<ISteamApps.FGetInstalledDepots>(Marshal.ReadIntPtr(this.VTable, 136));
			this._GetAppInstallDir = Marshal.GetDelegateForFunctionPointer<ISteamApps.FGetAppInstallDir>(Marshal.ReadIntPtr(this.VTable, 144));
			this._BIsAppInstalled = Marshal.GetDelegateForFunctionPointer<ISteamApps.FBIsAppInstalled>(Marshal.ReadIntPtr(this.VTable, 152));
			this._GetAppOwner = Marshal.GetDelegateForFunctionPointer<ISteamApps.FGetAppOwner>(Marshal.ReadIntPtr(this.VTable, 160));
			this._GetAppOwner_Windows = Marshal.GetDelegateForFunctionPointer<ISteamApps.FGetAppOwner_Windows>(Marshal.ReadIntPtr(this.VTable, 160));
			this._GetLaunchQueryParam = Marshal.GetDelegateForFunctionPointer<ISteamApps.FGetLaunchQueryParam>(Marshal.ReadIntPtr(this.VTable, 168));
			this._GetDlcDownloadProgress = Marshal.GetDelegateForFunctionPointer<ISteamApps.FGetDlcDownloadProgress>(Marshal.ReadIntPtr(this.VTable, 176));
			this._GetAppBuildId = Marshal.GetDelegateForFunctionPointer<ISteamApps.FGetAppBuildId>(Marshal.ReadIntPtr(this.VTable, 184));
			this._RequestAllProofOfPurchaseKeys = Marshal.GetDelegateForFunctionPointer<ISteamApps.FRequestAllProofOfPurchaseKeys>(Marshal.ReadIntPtr(this.VTable, 192));
			this._GetFileDetails = Marshal.GetDelegateForFunctionPointer<ISteamApps.FGetFileDetails>(Marshal.ReadIntPtr(this.VTable, 200));
			this._GetLaunchCommandLine = Marshal.GetDelegateForFunctionPointer<ISteamApps.FGetLaunchCommandLine>(Marshal.ReadIntPtr(this.VTable, 208));
			this._BIsSubscribedFromFamilySharing = Marshal.GetDelegateForFunctionPointer<ISteamApps.FBIsSubscribedFromFamilySharing>(Marshal.ReadIntPtr(this.VTable, 216));
		}

		internal void InstallDLC(AppId nAppID)
		{
			this._InstallDLC(this.Self, nAppID);
		}

		internal bool MarkContentCorrupt(bool bMissingFilesOnly)
		{
			return this._MarkContentCorrupt(this.Self, bMissingFilesOnly);
		}

		internal void RequestAllProofOfPurchaseKeys()
		{
			this._RequestAllProofOfPurchaseKeys(this.Self);
		}

		internal void RequestAppProofOfPurchaseKey(AppId nAppID)
		{
			this._RequestAppProofOfPurchaseKey(this.Self, nAppID);
		}

		internal override void Shutdown()
		{
			base.Shutdown();
			this._BIsSubscribed = null;
			this._BIsLowViolence = null;
			this._BIsCybercafe = null;
			this._BIsVACBanned = null;
			this._GetCurrentGameLanguage = null;
			this._GetAvailableGameLanguages = null;
			this._BIsSubscribedApp = null;
			this._BIsDlcInstalled = null;
			this._GetEarliestPurchaseUnixTime = null;
			this._BIsSubscribedFromFreeWeekend = null;
			this._GetDLCCount = null;
			this._BGetDLCDataByIndex = null;
			this._InstallDLC = null;
			this._UninstallDLC = null;
			this._RequestAppProofOfPurchaseKey = null;
			this._GetCurrentBetaName = null;
			this._MarkContentCorrupt = null;
			this._GetInstalledDepots = null;
			this._GetAppInstallDir = null;
			this._BIsAppInstalled = null;
			this._GetAppOwner = null;
			this._GetAppOwner_Windows = null;
			this._GetLaunchQueryParam = null;
			this._GetDlcDownloadProgress = null;
			this._GetAppBuildId = null;
			this._RequestAllProofOfPurchaseKeys = null;
			this._GetFileDetails = null;
			this._GetLaunchCommandLine = null;
			this._BIsSubscribedFromFamilySharing = null;
		}

		internal void UninstallDLC(AppId nAppID)
		{
			this._UninstallDLC(this.Self, nAppID);
		}

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FBGetDLCDataByIndex(IntPtr self, int iDLC, ref AppId pAppID, ref bool pbAvailable, StringBuilder pchName, int cchNameBufferSize);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FBIsAppInstalled(IntPtr self, AppId appID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FBIsCybercafe(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FBIsDlcInstalled(IntPtr self, AppId appID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FBIsLowViolence(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FBIsSubscribed(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FBIsSubscribedApp(IntPtr self, AppId appID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FBIsSubscribedFromFamilySharing(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FBIsSubscribedFromFreeWeekend(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FBIsVACBanned(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetAppBuildId(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate uint FGetAppInstallDir(IntPtr self, AppId appID, StringBuilder pchFolder, uint cchFolderBufferSize);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamId FGetAppOwner(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FGetAppOwner_Windows(IntPtr self, ref SteamId retVal);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate IntPtr FGetAvailableGameLanguages(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetCurrentBetaName(IntPtr self, StringBuilder pchName, int cchNameBufferSize);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate IntPtr FGetCurrentGameLanguage(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetDLCCount(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetDlcDownloadProgress(IntPtr self, AppId nAppID, ref ulong punBytesDownloaded, ref ulong punBytesTotal);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate uint FGetEarliestPurchaseUnixTime(IntPtr self, AppId nAppID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FGetFileDetails(IntPtr self, string pszFileName);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate uint FGetInstalledDepots(IntPtr self, AppId appID, [In][Out] DepotId_t[] pvecDepots, uint cMaxDepots);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetLaunchCommandLine(IntPtr self, StringBuilder pszCommandLine, int cubCommandLine);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate IntPtr FGetLaunchQueryParam(IntPtr self, string pchKey);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FInstallDLC(IntPtr self, AppId nAppID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FMarkContentCorrupt(IntPtr self, bool bMissingFilesOnly);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FRequestAllProofOfPurchaseKeys(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FRequestAppProofOfPurchaseKey(IntPtr self, AppId nAppID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FUninstallDLC(IntPtr self, AppId nAppID);
	}
}