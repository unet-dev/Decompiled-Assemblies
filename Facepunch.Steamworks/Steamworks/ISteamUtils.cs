using Steamworks.Data;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Steamworks
{
	internal class ISteamUtils : SteamInterface
	{
		private ISteamUtils.FGetSecondsSinceAppActive _GetSecondsSinceAppActive;

		private ISteamUtils.FGetSecondsSinceComputerActive _GetSecondsSinceComputerActive;

		private ISteamUtils.FGetConnectedUniverse _GetConnectedUniverse;

		private ISteamUtils.FGetServerRealTime _GetServerRealTime;

		private ISteamUtils.FGetIPCountry _GetIPCountry;

		private ISteamUtils.FGetImageSize _GetImageSize;

		private ISteamUtils.FGetImageRGBA _GetImageRGBA;

		private ISteamUtils.FGetCSERIPPort _GetCSERIPPort;

		private ISteamUtils.FGetCurrentBatteryPower _GetCurrentBatteryPower;

		private ISteamUtils.FGetAppID _GetAppID;

		private ISteamUtils.FSetOverlayNotificationPosition _SetOverlayNotificationPosition;

		private ISteamUtils.FIsAPICallCompleted _IsAPICallCompleted;

		private ISteamUtils.FGetAPICallFailureReason _GetAPICallFailureReason;

		private ISteamUtils.FGetAPICallResult _GetAPICallResult;

		private ISteamUtils.FRunFrame _RunFrame;

		private ISteamUtils.FGetIPCCallCount _GetIPCCallCount;

		private ISteamUtils.FSetWarningMessageHook _SetWarningMessageHook;

		private ISteamUtils.FIsOverlayEnabled _IsOverlayEnabled;

		private ISteamUtils.FBOverlayNeedsPresent _BOverlayNeedsPresent;

		private ISteamUtils.FCheckFileSignature _CheckFileSignature;

		private ISteamUtils.FShowGamepadTextInput _ShowGamepadTextInput;

		private ISteamUtils.FGetEnteredGamepadTextLength _GetEnteredGamepadTextLength;

		private ISteamUtils.FGetEnteredGamepadTextInput _GetEnteredGamepadTextInput;

		private ISteamUtils.FGetSteamUILanguage _GetSteamUILanguage;

		private ISteamUtils.FIsSteamRunningInVR _IsSteamRunningInVR;

		private ISteamUtils.FSetOverlayNotificationInset _SetOverlayNotificationInset;

		private ISteamUtils.FIsSteamInBigPictureMode _IsSteamInBigPictureMode;

		private ISteamUtils.FStartVRDashboard _StartVRDashboard;

		private ISteamUtils.FIsVRHeadsetStreamingEnabled _IsVRHeadsetStreamingEnabled;

		private ISteamUtils.FSetVRHeadsetStreamingEnabled _SetVRHeadsetStreamingEnabled;

		public override string InterfaceName
		{
			get
			{
				return "SteamUtils009";
			}
		}

		public ISteamUtils()
		{
		}

		internal bool BOverlayNeedsPresent()
		{
			return this._BOverlayNeedsPresent(this.Self);
		}

		internal async Task<CheckFileSignature_t?> CheckFileSignature(string szFileName)
		{
			CheckFileSignature_t? resultAsync = await CheckFileSignature_t.GetResultAsync(this._CheckFileSignature(this.Self, szFileName));
			return resultAsync;
		}

		internal SteamAPICallFailure GetAPICallFailureReason(SteamAPICall_t hSteamAPICall)
		{
			return this._GetAPICallFailureReason(this.Self, hSteamAPICall);
		}

		internal bool GetAPICallResult(SteamAPICall_t hSteamAPICall, IntPtr pCallback, int cubCallback, int iCallbackExpected, ref bool pbFailed)
		{
			bool self = this._GetAPICallResult(this.Self, hSteamAPICall, pCallback, cubCallback, iCallbackExpected, ref pbFailed);
			return self;
		}

		internal uint GetAppID()
		{
			return this._GetAppID(this.Self);
		}

		internal Universe GetConnectedUniverse()
		{
			return this._GetConnectedUniverse(this.Self);
		}

		internal bool GetCSERIPPort(ref uint unIP, ref ushort usPort)
		{
			return this._GetCSERIPPort(this.Self, ref unIP, ref usPort);
		}

		internal byte GetCurrentBatteryPower()
		{
			return this._GetCurrentBatteryPower(this.Self);
		}

		internal bool GetEnteredGamepadTextInput(StringBuilder pchText, uint cchText)
		{
			return this._GetEnteredGamepadTextInput(this.Self, pchText, cchText);
		}

		internal uint GetEnteredGamepadTextLength()
		{
			return this._GetEnteredGamepadTextLength(this.Self);
		}

		internal bool GetImageRGBA(int iImage, [In][Out] byte[] pubDest, int nDestBufferSize)
		{
			return this._GetImageRGBA(this.Self, iImage, pubDest, nDestBufferSize);
		}

		internal bool GetImageSize(int iImage, ref uint pnWidth, ref uint pnHeight)
		{
			return this._GetImageSize(this.Self, iImage, ref pnWidth, ref pnHeight);
		}

		internal uint GetIPCCallCount()
		{
			return this._GetIPCCallCount(this.Self);
		}

		internal string GetIPCountry()
		{
			return base.GetString(this._GetIPCountry(this.Self));
		}

		internal uint GetSecondsSinceAppActive()
		{
			return this._GetSecondsSinceAppActive(this.Self);
		}

		internal uint GetSecondsSinceComputerActive()
		{
			return this._GetSecondsSinceComputerActive(this.Self);
		}

		internal uint GetServerRealTime()
		{
			return this._GetServerRealTime(this.Self);
		}

		internal string GetSteamUILanguage()
		{
			return base.GetString(this._GetSteamUILanguage(this.Self));
		}

		public override void InitInternals()
		{
			this._GetSecondsSinceAppActive = Marshal.GetDelegateForFunctionPointer<ISteamUtils.FGetSecondsSinceAppActive>(Marshal.ReadIntPtr(this.VTable, 0));
			this._GetSecondsSinceComputerActive = Marshal.GetDelegateForFunctionPointer<ISteamUtils.FGetSecondsSinceComputerActive>(Marshal.ReadIntPtr(this.VTable, 8));
			this._GetConnectedUniverse = Marshal.GetDelegateForFunctionPointer<ISteamUtils.FGetConnectedUniverse>(Marshal.ReadIntPtr(this.VTable, 16));
			this._GetServerRealTime = Marshal.GetDelegateForFunctionPointer<ISteamUtils.FGetServerRealTime>(Marshal.ReadIntPtr(this.VTable, 24));
			this._GetIPCountry = Marshal.GetDelegateForFunctionPointer<ISteamUtils.FGetIPCountry>(Marshal.ReadIntPtr(this.VTable, 32));
			this._GetImageSize = Marshal.GetDelegateForFunctionPointer<ISteamUtils.FGetImageSize>(Marshal.ReadIntPtr(this.VTable, 40));
			this._GetImageRGBA = Marshal.GetDelegateForFunctionPointer<ISteamUtils.FGetImageRGBA>(Marshal.ReadIntPtr(this.VTable, 48));
			this._GetCSERIPPort = Marshal.GetDelegateForFunctionPointer<ISteamUtils.FGetCSERIPPort>(Marshal.ReadIntPtr(this.VTable, 56));
			this._GetCurrentBatteryPower = Marshal.GetDelegateForFunctionPointer<ISteamUtils.FGetCurrentBatteryPower>(Marshal.ReadIntPtr(this.VTable, 64));
			this._GetAppID = Marshal.GetDelegateForFunctionPointer<ISteamUtils.FGetAppID>(Marshal.ReadIntPtr(this.VTable, 72));
			this._SetOverlayNotificationPosition = Marshal.GetDelegateForFunctionPointer<ISteamUtils.FSetOverlayNotificationPosition>(Marshal.ReadIntPtr(this.VTable, 80));
			this._IsAPICallCompleted = Marshal.GetDelegateForFunctionPointer<ISteamUtils.FIsAPICallCompleted>(Marshal.ReadIntPtr(this.VTable, 88));
			this._GetAPICallFailureReason = Marshal.GetDelegateForFunctionPointer<ISteamUtils.FGetAPICallFailureReason>(Marshal.ReadIntPtr(this.VTable, 96));
			this._GetAPICallResult = Marshal.GetDelegateForFunctionPointer<ISteamUtils.FGetAPICallResult>(Marshal.ReadIntPtr(this.VTable, 104));
			this._RunFrame = Marshal.GetDelegateForFunctionPointer<ISteamUtils.FRunFrame>(Marshal.ReadIntPtr(this.VTable, 112));
			this._GetIPCCallCount = Marshal.GetDelegateForFunctionPointer<ISteamUtils.FGetIPCCallCount>(Marshal.ReadIntPtr(this.VTable, 120));
			this._SetWarningMessageHook = Marshal.GetDelegateForFunctionPointer<ISteamUtils.FSetWarningMessageHook>(Marshal.ReadIntPtr(this.VTable, 128));
			this._IsOverlayEnabled = Marshal.GetDelegateForFunctionPointer<ISteamUtils.FIsOverlayEnabled>(Marshal.ReadIntPtr(this.VTable, 136));
			this._BOverlayNeedsPresent = Marshal.GetDelegateForFunctionPointer<ISteamUtils.FBOverlayNeedsPresent>(Marshal.ReadIntPtr(this.VTable, 144));
			this._CheckFileSignature = Marshal.GetDelegateForFunctionPointer<ISteamUtils.FCheckFileSignature>(Marshal.ReadIntPtr(this.VTable, 152));
			this._ShowGamepadTextInput = Marshal.GetDelegateForFunctionPointer<ISteamUtils.FShowGamepadTextInput>(Marshal.ReadIntPtr(this.VTable, 160));
			this._GetEnteredGamepadTextLength = Marshal.GetDelegateForFunctionPointer<ISteamUtils.FGetEnteredGamepadTextLength>(Marshal.ReadIntPtr(this.VTable, 168));
			this._GetEnteredGamepadTextInput = Marshal.GetDelegateForFunctionPointer<ISteamUtils.FGetEnteredGamepadTextInput>(Marshal.ReadIntPtr(this.VTable, 176));
			this._GetSteamUILanguage = Marshal.GetDelegateForFunctionPointer<ISteamUtils.FGetSteamUILanguage>(Marshal.ReadIntPtr(this.VTable, 184));
			this._IsSteamRunningInVR = Marshal.GetDelegateForFunctionPointer<ISteamUtils.FIsSteamRunningInVR>(Marshal.ReadIntPtr(this.VTable, 192));
			this._SetOverlayNotificationInset = Marshal.GetDelegateForFunctionPointer<ISteamUtils.FSetOverlayNotificationInset>(Marshal.ReadIntPtr(this.VTable, 200));
			this._IsSteamInBigPictureMode = Marshal.GetDelegateForFunctionPointer<ISteamUtils.FIsSteamInBigPictureMode>(Marshal.ReadIntPtr(this.VTable, 208));
			this._StartVRDashboard = Marshal.GetDelegateForFunctionPointer<ISteamUtils.FStartVRDashboard>(Marshal.ReadIntPtr(this.VTable, 216));
			this._IsVRHeadsetStreamingEnabled = Marshal.GetDelegateForFunctionPointer<ISteamUtils.FIsVRHeadsetStreamingEnabled>(Marshal.ReadIntPtr(this.VTable, 224));
			this._SetVRHeadsetStreamingEnabled = Marshal.GetDelegateForFunctionPointer<ISteamUtils.FSetVRHeadsetStreamingEnabled>(Marshal.ReadIntPtr(this.VTable, 232));
		}

		internal bool IsAPICallCompleted(SteamAPICall_t hSteamAPICall, ref bool pbFailed)
		{
			return this._IsAPICallCompleted(this.Self, hSteamAPICall, ref pbFailed);
		}

		internal bool IsOverlayEnabled()
		{
			return this._IsOverlayEnabled(this.Self);
		}

		internal bool IsSteamInBigPictureMode()
		{
			return this._IsSteamInBigPictureMode(this.Self);
		}

		internal bool IsSteamRunningInVR()
		{
			return this._IsSteamRunningInVR(this.Self);
		}

		internal bool IsVRHeadsetStreamingEnabled()
		{
			return this._IsVRHeadsetStreamingEnabled(this.Self);
		}

		internal void RunFrame()
		{
			this._RunFrame(this.Self);
		}

		internal void SetOverlayNotificationInset(int nHorizontalInset, int nVerticalInset)
		{
			this._SetOverlayNotificationInset(this.Self, nHorizontalInset, nVerticalInset);
		}

		internal void SetOverlayNotificationPosition(NotificationPosition eNotificationPosition)
		{
			this._SetOverlayNotificationPosition(this.Self, eNotificationPosition);
		}

		internal void SetVRHeadsetStreamingEnabled(bool bEnabled)
		{
			this._SetVRHeadsetStreamingEnabled(this.Self, bEnabled);
		}

		internal void SetWarningMessageHook(IntPtr pFunction)
		{
			this._SetWarningMessageHook(this.Self, pFunction);
		}

		internal bool ShowGamepadTextInput(GamepadTextInputMode eInputMode, GamepadTextInputLineMode eLineInputMode, string pchDescription, uint unCharMax, string pchExistingText)
		{
			bool self = this._ShowGamepadTextInput(this.Self, eInputMode, eLineInputMode, pchDescription, unCharMax, pchExistingText);
			return self;
		}

		internal override void Shutdown()
		{
			base.Shutdown();
			this._GetSecondsSinceAppActive = null;
			this._GetSecondsSinceComputerActive = null;
			this._GetConnectedUniverse = null;
			this._GetServerRealTime = null;
			this._GetIPCountry = null;
			this._GetImageSize = null;
			this._GetImageRGBA = null;
			this._GetCSERIPPort = null;
			this._GetCurrentBatteryPower = null;
			this._GetAppID = null;
			this._SetOverlayNotificationPosition = null;
			this._IsAPICallCompleted = null;
			this._GetAPICallFailureReason = null;
			this._GetAPICallResult = null;
			this._RunFrame = null;
			this._GetIPCCallCount = null;
			this._SetWarningMessageHook = null;
			this._IsOverlayEnabled = null;
			this._BOverlayNeedsPresent = null;
			this._CheckFileSignature = null;
			this._ShowGamepadTextInput = null;
			this._GetEnteredGamepadTextLength = null;
			this._GetEnteredGamepadTextInput = null;
			this._GetSteamUILanguage = null;
			this._IsSteamRunningInVR = null;
			this._SetOverlayNotificationInset = null;
			this._IsSteamInBigPictureMode = null;
			this._StartVRDashboard = null;
			this._IsVRHeadsetStreamingEnabled = null;
			this._SetVRHeadsetStreamingEnabled = null;
		}

		internal void StartVRDashboard()
		{
			this._StartVRDashboard(this.Self);
		}

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FBOverlayNeedsPresent(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FCheckFileSignature(IntPtr self, string szFileName);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICallFailure FGetAPICallFailureReason(IntPtr self, SteamAPICall_t hSteamAPICall);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetAPICallResult(IntPtr self, SteamAPICall_t hSteamAPICall, IntPtr pCallback, int cubCallback, int iCallbackExpected, ref bool pbFailed);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate uint FGetAppID(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate Universe FGetConnectedUniverse(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetCSERIPPort(IntPtr self, ref uint unIP, ref ushort usPort);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate byte FGetCurrentBatteryPower(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetEnteredGamepadTextInput(IntPtr self, StringBuilder pchText, uint cchText);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate uint FGetEnteredGamepadTextLength(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetImageRGBA(IntPtr self, int iImage, [In][Out] byte[] pubDest, int nDestBufferSize);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetImageSize(IntPtr self, int iImage, ref uint pnWidth, ref uint pnHeight);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate uint FGetIPCCallCount(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate IntPtr FGetIPCountry(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate uint FGetSecondsSinceAppActive(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate uint FGetSecondsSinceComputerActive(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate uint FGetServerRealTime(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate IntPtr FGetSteamUILanguage(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FIsAPICallCompleted(IntPtr self, SteamAPICall_t hSteamAPICall, ref bool pbFailed);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FIsOverlayEnabled(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FIsSteamInBigPictureMode(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FIsSteamRunningInVR(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FIsVRHeadsetStreamingEnabled(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FRunFrame(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FSetOverlayNotificationInset(IntPtr self, int nHorizontalInset, int nVerticalInset);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FSetOverlayNotificationPosition(IntPtr self, NotificationPosition eNotificationPosition);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FSetVRHeadsetStreamingEnabled(IntPtr self, bool bEnabled);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FSetWarningMessageHook(IntPtr self, IntPtr pFunction);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FShowGamepadTextInput(IntPtr self, GamepadTextInputMode eInputMode, GamepadTextInputLineMode eLineInputMode, string pchDescription, uint unCharMax, string pchExistingText);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FStartVRDashboard(IntPtr self);
	}
}