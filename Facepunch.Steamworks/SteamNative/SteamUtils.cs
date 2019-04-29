using Facepunch.Steamworks;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SteamNative
{
	internal class SteamUtils : IDisposable
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

		internal SteamUtils(BaseSteamworks steamworks, IntPtr pointer)
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

		public bool BOverlayNeedsPresent()
		{
			return this.platform.ISteamUtils_BOverlayNeedsPresent();
		}

		public CallbackHandle CheckFileSignature(string szFileName, Action<CheckFileSignature_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamUtils_CheckFileSignature(szFileName);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return CheckFileSignature_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public virtual void Dispose()
		{
			if (this.platform != null)
			{
				this.platform.Dispose();
				this.platform = null;
			}
		}

		public SteamAPICallFailure GetAPICallFailureReason(SteamAPICall_t hSteamAPICall)
		{
			return this.platform.ISteamUtils_GetAPICallFailureReason(hSteamAPICall.Value);
		}

		public bool GetAPICallResult(SteamAPICall_t hSteamAPICall, IntPtr pCallback, int cubCallback, int iCallbackExpected, ref bool pbFailed)
		{
			return this.platform.ISteamUtils_GetAPICallResult(hSteamAPICall.Value, pCallback, cubCallback, iCallbackExpected, ref pbFailed);
		}

		public uint GetAppID()
		{
			return this.platform.ISteamUtils_GetAppID();
		}

		public Universe GetConnectedUniverse()
		{
			return this.platform.ISteamUtils_GetConnectedUniverse();
		}

		public bool GetCSERIPPort(out uint unIP, out ushort usPort)
		{
			return this.platform.ISteamUtils_GetCSERIPPort(out unIP, out usPort);
		}

		public byte GetCurrentBatteryPower()
		{
			return this.platform.ISteamUtils_GetCurrentBatteryPower();
		}

		public string GetEnteredGamepadTextInput()
		{
			StringBuilder stringBuilder = Helpers.TakeStringBuilder();
			if (!this.platform.ISteamUtils_GetEnteredGamepadTextInput(stringBuilder, 4096))
			{
				return null;
			}
			return stringBuilder.ToString();
		}

		public uint GetEnteredGamepadTextLength()
		{
			return this.platform.ISteamUtils_GetEnteredGamepadTextLength();
		}

		public bool GetImageRGBA(int iImage, IntPtr pubDest, int nDestBufferSize)
		{
			return this.platform.ISteamUtils_GetImageRGBA(iImage, pubDest, nDestBufferSize);
		}

		public bool GetImageSize(int iImage, out uint pnWidth, out uint pnHeight)
		{
			return this.platform.ISteamUtils_GetImageSize(iImage, out pnWidth, out pnHeight);
		}

		public uint GetIPCCallCount()
		{
			return this.platform.ISteamUtils_GetIPCCallCount();
		}

		public string GetIPCountry()
		{
			return Marshal.PtrToStringAnsi(this.platform.ISteamUtils_GetIPCountry());
		}

		public uint GetSecondsSinceAppActive()
		{
			return this.platform.ISteamUtils_GetSecondsSinceAppActive();
		}

		public uint GetSecondsSinceComputerActive()
		{
			return this.platform.ISteamUtils_GetSecondsSinceComputerActive();
		}

		public uint GetServerRealTime()
		{
			return this.platform.ISteamUtils_GetServerRealTime();
		}

		public string GetSteamUILanguage()
		{
			return Marshal.PtrToStringAnsi(this.platform.ISteamUtils_GetSteamUILanguage());
		}

		public bool IsAPICallCompleted(SteamAPICall_t hSteamAPICall, ref bool pbFailed)
		{
			return this.platform.ISteamUtils_IsAPICallCompleted(hSteamAPICall.Value, ref pbFailed);
		}

		public bool IsOverlayEnabled()
		{
			return this.platform.ISteamUtils_IsOverlayEnabled();
		}

		public bool IsSteamInBigPictureMode()
		{
			return this.platform.ISteamUtils_IsSteamInBigPictureMode();
		}

		public bool IsSteamRunningInVR()
		{
			return this.platform.ISteamUtils_IsSteamRunningInVR();
		}

		public bool IsVRHeadsetStreamingEnabled()
		{
			return this.platform.ISteamUtils_IsVRHeadsetStreamingEnabled();
		}

		public void SetOverlayNotificationInset(int nHorizontalInset, int nVerticalInset)
		{
			this.platform.ISteamUtils_SetOverlayNotificationInset(nHorizontalInset, nVerticalInset);
		}

		public void SetOverlayNotificationPosition(NotificationPosition eNotificationPosition)
		{
			this.platform.ISteamUtils_SetOverlayNotificationPosition(eNotificationPosition);
		}

		public void SetVRHeadsetStreamingEnabled(bool bEnabled)
		{
			this.platform.ISteamUtils_SetVRHeadsetStreamingEnabled(bEnabled);
		}

		public void SetWarningMessageHook(IntPtr pFunction)
		{
			this.platform.ISteamUtils_SetWarningMessageHook(pFunction);
		}

		public bool ShowGamepadTextInput(GamepadTextInputMode eInputMode, GamepadTextInputLineMode eLineInputMode, string pchDescription, uint unCharMax, string pchExistingText)
		{
			return this.platform.ISteamUtils_ShowGamepadTextInput(eInputMode, eLineInputMode, pchDescription, unCharMax, pchExistingText);
		}

		public void StartVRDashboard()
		{
			this.platform.ISteamUtils_StartVRDashboard();
		}
	}
}