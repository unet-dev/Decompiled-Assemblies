using Facepunch.Steamworks;
using System;

namespace SteamNative
{
	internal class SteamParentalSettings : IDisposable
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

		internal SteamParentalSettings(BaseSteamworks steamworks, IntPtr pointer)
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

		public bool BIsAppBlocked(AppId_t nAppID)
		{
			return this.platform.ISteamParentalSettings_BIsAppBlocked(nAppID.Value);
		}

		public bool BIsAppInBlockList(AppId_t nAppID)
		{
			return this.platform.ISteamParentalSettings_BIsAppInBlockList(nAppID.Value);
		}

		public bool BIsFeatureBlocked(ParentalFeature eFeature)
		{
			return this.platform.ISteamParentalSettings_BIsFeatureBlocked(eFeature);
		}

		public bool BIsFeatureInBlockList(ParentalFeature eFeature)
		{
			return this.platform.ISteamParentalSettings_BIsFeatureInBlockList(eFeature);
		}

		public bool BIsParentalLockEnabled()
		{
			return this.platform.ISteamParentalSettings_BIsParentalLockEnabled();
		}

		public bool BIsParentalLockLocked()
		{
			return this.platform.ISteamParentalSettings_BIsParentalLockLocked();
		}

		public virtual void Dispose()
		{
			if (this.platform != null)
			{
				this.platform.Dispose();
				this.platform = null;
			}
		}
	}
}