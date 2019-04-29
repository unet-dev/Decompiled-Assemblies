using Facepunch.Steamworks.Interop;
using SteamNative;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Facepunch.Steamworks
{
	public class App : IDisposable
	{
		internal Client client;

		internal App(Client c)
		{
			this.client = c;
			this.client.RegisterCallback<DlcInstalled_t>(new Action<DlcInstalled_t>(this.DlcInstalled));
		}

		public void Dispose()
		{
			this.client = null;
		}

		private void DlcInstalled(DlcInstalled_t data)
		{
			if (this.OnDlcInstalled != null)
			{
				this.OnDlcInstalled(data.AppID);
			}
		}

		public int GetBuildId(uint appId)
		{
			return this.client.native.applist.GetAppBuildId(appId);
		}

		public string GetInstallFolder(uint appId)
		{
			return this.client.native.applist.GetAppInstallDir(appId);
		}

		public string GetName(uint appId)
		{
			string appName = this.client.native.applist.GetAppName(appId);
			if (appName == null)
			{
				return "error";
			}
			return appName;
		}

		public void InstallDlc(uint appId)
		{
			this.client.native.apps.InstallDLC(appId);
		}

		public bool IsDlcInstalled(uint appId)
		{
			return this.client.native.apps.BIsDlcInstalled(appId);
		}

		public bool IsInstalled(uint appId)
		{
			return this.client.native.apps.BIsAppInstalled(appId);
		}

		public bool IsSubscribed(uint appId)
		{
			return this.client.native.apps.BIsSubscribedApp(appId);
		}

		public void MarkContentCorrupt(bool missingFilesOnly = false)
		{
			this.client.native.apps.MarkContentCorrupt(missingFilesOnly);
		}

		public DateTime PurchaseTime(uint appId)
		{
			uint earliestPurchaseUnixTime = this.client.native.apps.GetEarliestPurchaseUnixTime(appId);
			if (earliestPurchaseUnixTime == 0)
			{
				return DateTime.MinValue;
			}
			return Utility.Epoch.ToDateTime(earliestPurchaseUnixTime);
		}

		public void UninstallDlc(uint appId)
		{
			this.client.native.apps.UninstallDLC(appId);
		}

		public event App.DlcInstalledDelegate OnDlcInstalled;

		public delegate void DlcInstalledDelegate(uint appid);
	}
}