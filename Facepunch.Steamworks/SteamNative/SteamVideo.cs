using Facepunch.Steamworks;
using System;
using System.Text;

namespace SteamNative
{
	internal class SteamVideo : IDisposable
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

		internal SteamVideo(BaseSteamworks steamworks, IntPtr pointer)
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

		public virtual void Dispose()
		{
			if (this.platform != null)
			{
				this.platform.Dispose();
				this.platform = null;
			}
		}

		public void GetOPFSettings(AppId_t unVideoAppID)
		{
			this.platform.ISteamVideo_GetOPFSettings(unVideoAppID.Value);
		}

		public string GetOPFStringForApp(AppId_t unVideoAppID)
		{
			StringBuilder stringBuilder = Helpers.TakeStringBuilder();
			int num = 4096;
			if (!this.platform.ISteamVideo_GetOPFStringForApp(unVideoAppID.Value, stringBuilder, out num))
			{
				return null;
			}
			return stringBuilder.ToString();
		}

		public void GetVideoURL(AppId_t unVideoAppID)
		{
			this.platform.ISteamVideo_GetVideoURL(unVideoAppID.Value);
		}

		public bool IsBroadcasting(IntPtr pnNumViewers)
		{
			return this.platform.ISteamVideo_IsBroadcasting(pnNumViewers);
		}
	}
}