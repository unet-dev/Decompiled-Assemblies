using Steamworks.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Steamworks
{
	public static class SteamApps
	{
		private static ISteamApps _internal;

		public static SteamId AppOwner
		{
			get
			{
				return SteamApps.Internal.GetAppOwner().Value;
			}
		}

		public static string[] AvailablLanguages
		{
			get
			{
				return SteamApps.Internal.GetAvailableGameLanguages().Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			}
		}

		public static int BuildId
		{
			get
			{
				return SteamApps.Internal.GetAppBuildId();
			}
		}

		public static string CommandLine
		{
			get
			{
				StringBuilder stringBuilder = Helpers.TakeStringBuilder();
				SteamApps.Internal.GetLaunchCommandLine(stringBuilder, stringBuilder.Capacity);
				return stringBuilder.ToString();
			}
		}

		public static string CurrentBetaName
		{
			get
			{
				string str;
				StringBuilder stringBuilder = Helpers.TakeStringBuilder();
				if (SteamApps.Internal.GetCurrentBetaName(stringBuilder, stringBuilder.Capacity))
				{
					str = stringBuilder.ToString();
				}
				else
				{
					str = null;
				}
				return str;
			}
		}

		public static string GameLanguage
		{
			get
			{
				return SteamApps.Internal.GetCurrentGameLanguage();
			}
		}

		internal static ISteamApps Internal
		{
			get
			{
				if (SteamApps._internal == null)
				{
					SteamApps._internal = new ISteamApps();
					SteamApps._internal.Init();
				}
				return SteamApps._internal;
			}
		}

		public static bool IsCybercafe
		{
			get
			{
				return SteamApps.Internal.BIsCybercafe();
			}
		}

		public static bool IsLowVoilence
		{
			get
			{
				return SteamApps.Internal.BIsLowViolence();
			}
		}

		public static bool IsSubscribed
		{
			get
			{
				return SteamApps.Internal.BIsSubscribed();
			}
		}

		public static bool IsSubscribedFromFamilySharing
		{
			get
			{
				return SteamApps.Internal.BIsSubscribedFromFamilySharing();
			}
		}

		public static bool IsSubscribedFromFreeWeekend
		{
			get
			{
				return SteamApps.Internal.BIsSubscribedFromFreeWeekend();
			}
		}

		public static bool IsVACBanned
		{
			get
			{
				return SteamApps.Internal.BIsVACBanned();
			}
		}

		public static string AppInstallDir(AppId appid = null)
		{
			string str;
			if (appid == 0)
			{
				appid = SteamClient.AppId;
			}
			StringBuilder stringBuilder = Helpers.TakeStringBuilder();
			if (SteamApps.Internal.GetAppInstallDir(appid.Value, stringBuilder, (uint)stringBuilder.Capacity) != 0)
			{
				str = stringBuilder.ToString();
			}
			else
			{
				str = null;
			}
			return str;
		}

		public static DownloadProgress DlcDownloadProgress(AppId appid)
		{
			DownloadProgress downloadProgress;
			DownloadProgress downloadProgress1;
			ulong num = (ulong)0;
			ulong num1 = (ulong)0;
			if (SteamApps.Internal.GetDlcDownloadProgress(appid.Value, ref num, ref num1))
			{
				downloadProgress = new DownloadProgress()
				{
					BytesDownloaded = num,
					BytesTotal = num1,
					Active = true
				};
				downloadProgress1 = downloadProgress;
			}
			else
			{
				downloadProgress = new DownloadProgress();
				downloadProgress1 = downloadProgress;
			}
			return downloadProgress1;
		}

		public static IEnumerable<DlcInformation> DlcInformation()
		{
			AppId appId = new AppId();
			bool flag = false;
			for (int i = 0; i < SteamApps.Internal.GetDLCCount(); i++)
			{
				StringBuilder stringBuilder = Helpers.TakeStringBuilder();
				if (SteamApps.Internal.BGetDLCDataByIndex(i, ref appId, ref flag, stringBuilder, stringBuilder.Capacity))
				{
					DlcInformation dlcInformation = new DlcInformation()
					{
						AppId = appId.Value,
						Name = stringBuilder.ToString(),
						Available = flag
					};
					yield return dlcInformation;
					stringBuilder = null;
				}
			}
		}

		public static async Task<FileDetails?> GetFileDetailsAsync(string filename)
		{
			FileDetails? nullable;
			bool flag;
			FileDetailsResult_t? fileDetails = await SteamApps.Internal.GetFileDetails(filename);
			FileDetailsResult_t? nullable1 = fileDetails;
			fileDetails = null;
			flag = (!nullable1.HasValue ? true : nullable1.Value.Result != Result.OK);
			if (!flag)
			{
				FileDetails fileDetail = new FileDetails()
				{
					SizeInBytes = nullable1.Value.FileSize,
					Flags = nullable1.Value.Flags
				};
				byte[] fileSHA = nullable1.Value.FileSHA;
				fileDetail.Sha1 = String.Join("", 
					from x in fileSHA
					select x.ToString("x"));
				nullable = new FileDetails?(fileDetail);
			}
			else
			{
				nullable = null;
			}
			return nullable;
		}

		public static string GetLaunchParam(string param)
		{
			return SteamApps.Internal.GetLaunchQueryParam(param);
		}

		public static void InstallDlc(AppId appid)
		{
			SteamApps.Internal.InstallDLC(appid.Value);
		}

		public static IEnumerable<DepotId> InstalledDepots(AppId appid = null)
		{
			if (appid == 0)
			{
				appid = SteamClient.AppId;
			}
			DepotId_t[] depotIdTArray = new DepotId_t[32];
			uint installedDepots = 0;
			installedDepots = SteamApps.Internal.GetInstalledDepots(appid.Value, depotIdTArray, (uint)depotIdTArray.Length);
			for (int i = 0; (long)i < (ulong)installedDepots; i++)
			{
				yield return new DepotId()
				{
					Value = depotIdTArray[i].Value
				};
			}
		}

		internal static void InstallEvents()
		{
			DlcInstalled_t.Install((DlcInstalled_t x) => {
				Action<AppId> onDlcInstalled = SteamApps.OnDlcInstalled;
				if (onDlcInstalled != null)
				{
					onDlcInstalled(x.AppID);
				}
				else
				{
				}
			}, false);
			NewUrlLaunchParameters_t.Install((NewUrlLaunchParameters_t x) => {
				Action onNewLaunchParameters = SteamApps.OnNewLaunchParameters;
				if (onNewLaunchParameters != null)
				{
					onNewLaunchParameters();
				}
				else
				{
				}
			}, false);
		}

		public static bool IsAppInstalled(AppId appid)
		{
			return SteamApps.Internal.BIsAppInstalled(appid.Value);
		}

		public static bool IsDlcInstalled(AppId appid)
		{
			return SteamApps.Internal.BIsDlcInstalled(appid.Value);
		}

		public static bool IsSubscribedToApp(AppId appid)
		{
			return SteamApps.Internal.BIsSubscribedApp(appid.Value);
		}

		public static void MarkContentCorrupt(bool missingFilesOnly)
		{
			SteamApps.Internal.MarkContentCorrupt(missingFilesOnly);
		}

		public static DateTime PurchaseTime(AppId appid = null)
		{
			if (appid == 0)
			{
				appid = SteamClient.AppId;
			}
			return Epoch.ToDateTime(SteamApps.Internal.GetEarliestPurchaseUnixTime(appid.Value));
		}

		internal static void Shutdown()
		{
			SteamApps._internal = null;
		}

		public static void UninstallDlc(AppId appid)
		{
			SteamApps.Internal.UninstallDLC(appid.Value);
		}

		public static event Action<AppId> OnDlcInstalled;

		public static event Action OnNewLaunchParameters;
	}
}