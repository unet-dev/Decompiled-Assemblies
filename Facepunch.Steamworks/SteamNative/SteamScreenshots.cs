using Facepunch.Steamworks;
using System;

namespace SteamNative
{
	internal class SteamScreenshots : IDisposable
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

		internal SteamScreenshots(BaseSteamworks steamworks, IntPtr pointer)
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

		public ScreenshotHandle AddScreenshotToLibrary(string pchFilename, string pchThumbnailFilename, int nWidth, int nHeight)
		{
			return this.platform.ISteamScreenshots_AddScreenshotToLibrary(pchFilename, pchThumbnailFilename, nWidth, nHeight);
		}

		public ScreenshotHandle AddVRScreenshotToLibrary(VRScreenshotType eType, string pchFilename, string pchVRFilename)
		{
			return this.platform.ISteamScreenshots_AddVRScreenshotToLibrary(eType, pchFilename, pchVRFilename);
		}

		public virtual void Dispose()
		{
			if (this.platform != null)
			{
				this.platform.Dispose();
				this.platform = null;
			}
		}

		public void HookScreenshots(bool bHook)
		{
			this.platform.ISteamScreenshots_HookScreenshots(bHook);
		}

		public bool IsScreenshotsHooked()
		{
			return this.platform.ISteamScreenshots_IsScreenshotsHooked();
		}

		public bool SetLocation(ScreenshotHandle hScreenshot, string pchLocation)
		{
			return this.platform.ISteamScreenshots_SetLocation(hScreenshot.Value, pchLocation);
		}

		public bool TagPublishedFile(ScreenshotHandle hScreenshot, PublishedFileId_t unPublishedFileID)
		{
			return this.platform.ISteamScreenshots_TagPublishedFile(hScreenshot.Value, unPublishedFileID.Value);
		}

		public bool TagUser(ScreenshotHandle hScreenshot, CSteamID steamID)
		{
			return this.platform.ISteamScreenshots_TagUser(hScreenshot.Value, steamID.Value);
		}

		public void TriggerScreenshot()
		{
			this.platform.ISteamScreenshots_TriggerScreenshot();
		}

		public ScreenshotHandle WriteScreenshot(IntPtr pubRGB, uint cubRGB, int nWidth, int nHeight)
		{
			return this.platform.ISteamScreenshots_WriteScreenshot(pubRGB, cubRGB, nWidth, nHeight);
		}
	}
}