using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Steamworks
{
	internal class ISteamVideo : SteamInterface
	{
		private ISteamVideo.FGetVideoURL _GetVideoURL;

		private ISteamVideo.FIsBroadcasting _IsBroadcasting;

		private ISteamVideo.FGetOPFSettings _GetOPFSettings;

		private ISteamVideo.FGetOPFStringForApp _GetOPFStringForApp;

		public override string InterfaceName
		{
			get
			{
				return "STEAMVIDEO_INTERFACE_V002";
			}
		}

		public ISteamVideo()
		{
		}

		internal void GetOPFSettings(AppId unVideoAppID)
		{
			this._GetOPFSettings(this.Self, unVideoAppID);
		}

		internal bool GetOPFStringForApp(AppId unVideoAppID, StringBuilder pchBuffer, ref int pnBufferSize)
		{
			return this._GetOPFStringForApp(this.Self, unVideoAppID, pchBuffer, ref pnBufferSize);
		}

		internal void GetVideoURL(AppId unVideoAppID)
		{
			this._GetVideoURL(this.Self, unVideoAppID);
		}

		public override void InitInternals()
		{
			this._GetVideoURL = Marshal.GetDelegateForFunctionPointer<ISteamVideo.FGetVideoURL>(Marshal.ReadIntPtr(this.VTable, 0));
			this._IsBroadcasting = Marshal.GetDelegateForFunctionPointer<ISteamVideo.FIsBroadcasting>(Marshal.ReadIntPtr(this.VTable, 8));
			this._GetOPFSettings = Marshal.GetDelegateForFunctionPointer<ISteamVideo.FGetOPFSettings>(Marshal.ReadIntPtr(this.VTable, 16));
			this._GetOPFStringForApp = Marshal.GetDelegateForFunctionPointer<ISteamVideo.FGetOPFStringForApp>(Marshal.ReadIntPtr(this.VTable, 24));
		}

		internal bool IsBroadcasting(ref int pnNumViewers)
		{
			return this._IsBroadcasting(this.Self, ref pnNumViewers);
		}

		internal override void Shutdown()
		{
			base.Shutdown();
			this._GetVideoURL = null;
			this._IsBroadcasting = null;
			this._GetOPFSettings = null;
			this._GetOPFStringForApp = null;
		}

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FGetOPFSettings(IntPtr self, AppId unVideoAppID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetOPFStringForApp(IntPtr self, AppId unVideoAppID, StringBuilder pchBuffer, ref int pnBufferSize);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FGetVideoURL(IntPtr self, AppId unVideoAppID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FIsBroadcasting(IntPtr self, ref int pnNumViewers);
	}
}