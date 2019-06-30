using Steamworks.Data;
using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	internal class ISteamScreenshots : SteamInterface
	{
		private ISteamScreenshots.FWriteScreenshot _WriteScreenshot;

		private ISteamScreenshots.FAddScreenshotToLibrary _AddScreenshotToLibrary;

		private ISteamScreenshots.FTriggerScreenshot _TriggerScreenshot;

		private ISteamScreenshots.FHookScreenshots _HookScreenshots;

		private ISteamScreenshots.FSetLocation _SetLocation;

		private ISteamScreenshots.FTagUser _TagUser;

		private ISteamScreenshots.FTagPublishedFile _TagPublishedFile;

		private ISteamScreenshots.FIsScreenshotsHooked _IsScreenshotsHooked;

		private ISteamScreenshots.FAddVRScreenshotToLibrary _AddVRScreenshotToLibrary;

		public override string InterfaceName
		{
			get
			{
				return "STEAMSCREENSHOTS_INTERFACE_VERSION003";
			}
		}

		public ISteamScreenshots()
		{
		}

		internal ScreenshotHandle AddScreenshotToLibrary(string pchFilename, string pchThumbnailFilename, int nWidth, int nHeight)
		{
			ScreenshotHandle self = this._AddScreenshotToLibrary(this.Self, pchFilename, pchThumbnailFilename, nWidth, nHeight);
			return self;
		}

		internal ScreenshotHandle AddVRScreenshotToLibrary(VRScreenshotType eType, string pchFilename, string pchVRFilename)
		{
			return this._AddVRScreenshotToLibrary(this.Self, eType, pchFilename, pchVRFilename);
		}

		internal void HookScreenshots(bool bHook)
		{
			this._HookScreenshots(this.Self, bHook);
		}

		public override void InitInternals()
		{
			this._WriteScreenshot = Marshal.GetDelegateForFunctionPointer<ISteamScreenshots.FWriteScreenshot>(Marshal.ReadIntPtr(this.VTable, 0));
			this._AddScreenshotToLibrary = Marshal.GetDelegateForFunctionPointer<ISteamScreenshots.FAddScreenshotToLibrary>(Marshal.ReadIntPtr(this.VTable, 8));
			this._TriggerScreenshot = Marshal.GetDelegateForFunctionPointer<ISteamScreenshots.FTriggerScreenshot>(Marshal.ReadIntPtr(this.VTable, 16));
			this._HookScreenshots = Marshal.GetDelegateForFunctionPointer<ISteamScreenshots.FHookScreenshots>(Marshal.ReadIntPtr(this.VTable, 24));
			this._SetLocation = Marshal.GetDelegateForFunctionPointer<ISteamScreenshots.FSetLocation>(Marshal.ReadIntPtr(this.VTable, 32));
			this._TagUser = Marshal.GetDelegateForFunctionPointer<ISteamScreenshots.FTagUser>(Marshal.ReadIntPtr(this.VTable, 40));
			this._TagPublishedFile = Marshal.GetDelegateForFunctionPointer<ISteamScreenshots.FTagPublishedFile>(Marshal.ReadIntPtr(this.VTable, 48));
			this._IsScreenshotsHooked = Marshal.GetDelegateForFunctionPointer<ISteamScreenshots.FIsScreenshotsHooked>(Marshal.ReadIntPtr(this.VTable, 56));
			this._AddVRScreenshotToLibrary = Marshal.GetDelegateForFunctionPointer<ISteamScreenshots.FAddVRScreenshotToLibrary>(Marshal.ReadIntPtr(this.VTable, 64));
		}

		internal bool IsScreenshotsHooked()
		{
			return this._IsScreenshotsHooked(this.Self);
		}

		internal bool SetLocation(ScreenshotHandle hScreenshot, string pchLocation)
		{
			return this._SetLocation(this.Self, hScreenshot, pchLocation);
		}

		internal override void Shutdown()
		{
			base.Shutdown();
			this._WriteScreenshot = null;
			this._AddScreenshotToLibrary = null;
			this._TriggerScreenshot = null;
			this._HookScreenshots = null;
			this._SetLocation = null;
			this._TagUser = null;
			this._TagPublishedFile = null;
			this._IsScreenshotsHooked = null;
			this._AddVRScreenshotToLibrary = null;
		}

		internal bool TagPublishedFile(ScreenshotHandle hScreenshot, PublishedFileId unPublishedFileID)
		{
			return this._TagPublishedFile(this.Self, hScreenshot, unPublishedFileID);
		}

		internal bool TagUser(ScreenshotHandle hScreenshot, SteamId steamID)
		{
			return this._TagUser(this.Self, hScreenshot, steamID);
		}

		internal void TriggerScreenshot()
		{
			this._TriggerScreenshot(this.Self);
		}

		internal ScreenshotHandle WriteScreenshot(IntPtr pubRGB, uint cubRGB, int nWidth, int nHeight)
		{
			ScreenshotHandle self = this._WriteScreenshot(this.Self, pubRGB, cubRGB, nWidth, nHeight);
			return self;
		}

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate ScreenshotHandle FAddScreenshotToLibrary(IntPtr self, string pchFilename, string pchThumbnailFilename, int nWidth, int nHeight);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate ScreenshotHandle FAddVRScreenshotToLibrary(IntPtr self, VRScreenshotType eType, string pchFilename, string pchVRFilename);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FHookScreenshots(IntPtr self, bool bHook);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FIsScreenshotsHooked(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetLocation(IntPtr self, ScreenshotHandle hScreenshot, string pchLocation);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FTagPublishedFile(IntPtr self, ScreenshotHandle hScreenshot, PublishedFileId unPublishedFileID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FTagUser(IntPtr self, ScreenshotHandle hScreenshot, SteamId steamID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FTriggerScreenshot(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate ScreenshotHandle FWriteScreenshot(IntPtr self, IntPtr pubRGB, uint cubRGB, int nWidth, int nHeight);
	}
}