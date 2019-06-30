using Steamworks.Data;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Steamworks
{
	public static class SteamUtils
	{
		private static ISteamUtils _internal;

		private static NotificationPosition overlayNotificationPosition;

		public static Universe ConnectedUniverse
		{
			get
			{
				return SteamUtils.Internal.GetConnectedUniverse();
			}
		}

		public static float CurrentBatteryPower
		{
			get
			{
				return Math.Min((float)(SteamUtils.Internal.GetCurrentBatteryPower() / 100), 1f);
			}
		}

		public static bool DoesOverlayNeedPresent
		{
			get
			{
				return SteamUtils.Internal.BOverlayNeedsPresent();
			}
		}

		internal static ISteamUtils Internal
		{
			get
			{
				if (SteamUtils._internal == null)
				{
					SteamUtils._internal = new ISteamUtils();
					SteamUtils._internal.Init();
				}
				return SteamUtils._internal;
			}
		}

		public static string IpCountry
		{
			get
			{
				return SteamUtils.Internal.GetIPCountry();
			}
		}

		public static bool IsOverlayEnabled
		{
			get
			{
				return SteamUtils.Internal.IsOverlayEnabled();
			}
		}

		public static bool IsSteamInBigPictureMode
		{
			get
			{
				return SteamUtils.Internal.IsSteamInBigPictureMode();
			}
		}

		public static bool IsSteamRunningInVR
		{
			get
			{
				return SteamUtils.Internal.IsSteamRunningInVR();
			}
		}

		public static NotificationPosition OverlayNotificationPosition
		{
			get
			{
				return SteamUtils.overlayNotificationPosition;
			}
			set
			{
				SteamUtils.overlayNotificationPosition = value;
				SteamUtils.Internal.SetOverlayNotificationPosition(value);
			}
		}

		public static uint SecondsSinceAppActive
		{
			get
			{
				return SteamUtils.Internal.GetSecondsSinceAppActive();
			}
		}

		public static uint SecondsSinceComputerActive
		{
			get
			{
				return SteamUtils.Internal.GetSecondsSinceComputerActive();
			}
		}

		public static DateTime SteamServerTime
		{
			get
			{
				return Epoch.ToDateTime(SteamUtils.Internal.GetServerRealTime());
			}
		}

		public static string SteamUILanguage
		{
			get
			{
				return SteamUtils.Internal.GetSteamUILanguage();
			}
		}

		public static bool UsingBatteryPower
		{
			get
			{
				return SteamUtils.Internal.GetCurrentBatteryPower() != 255;
			}
		}

		public static bool VrHeadsetStreaming
		{
			get
			{
				return SteamUtils.Internal.IsVRHeadsetStreamingEnabled();
			}
			set
			{
				SteamUtils.Internal.SetVRHeadsetStreamingEnabled(value);
			}
		}

		static SteamUtils()
		{
			SteamUtils.overlayNotificationPosition = NotificationPosition.BottomRight;
		}

		public static async Task<CheckFileSignature> CheckFileSignatureAsync(string filename)
		{
			CheckFileSignature_t? nullable = await SteamUtils.Internal.CheckFileSignature(filename);
			CheckFileSignature_t? nullable1 = nullable;
			nullable = null;
			if (!nullable1.HasValue)
			{
				throw new Exception("Something went wrong");
			}
			return nullable1.Value.CheckFileSignature;
		}

		public static string GetEnteredGamepadText()
		{
			string empty;
			uint enteredGamepadTextLength = SteamUtils.Internal.GetEnteredGamepadTextLength();
			if (enteredGamepadTextLength != 0)
			{
				StringBuilder stringBuilder = Helpers.TakeStringBuilder();
				empty = (SteamUtils.Internal.GetEnteredGamepadTextInput(stringBuilder, enteredGamepadTextLength) ? stringBuilder.ToString() : String.Empty);
			}
			else
			{
				empty = String.Empty;
			}
			return empty;
		}

		public static Image? GetImage(int image)
		{
			Image? nullable;
			Image? nullable1;
			if (image == -1)
			{
				nullable = null;
				nullable1 = nullable;
			}
			else if (image != 0)
			{
				Image image1 = new Image();
				if (SteamUtils.GetImageSize(image, out image1.Width, out image1.Height))
				{
					uint width = image1.Width * image1.Height * 4;
					byte[] numArray = Helpers.TakeBuffer((int)width);
					if (SteamUtils.Internal.GetImageRGBA(image, numArray, (int)width))
					{
						image1.Data = new Byte[width];
						Array.Copy(numArray, (long)0, image1.Data, (long)0, (long)width);
						nullable1 = new Image?(image1);
					}
					else
					{
						nullable = null;
						nullable1 = nullable;
					}
				}
				else
				{
					nullable = null;
					nullable1 = nullable;
				}
			}
			else
			{
				nullable = null;
				nullable1 = nullable;
			}
			return nullable1;
		}

		public static bool GetImageSize(int image, out uint width, out uint height)
		{
			width = 0;
			height = 0;
			return SteamUtils.Internal.GetImageSize(image, ref width, ref height);
		}

		internal static void InstallEvents()
		{
			IPCountry_t.Install((IPCountry_t x) => {
				Action onIpCountryChanged = SteamUtils.OnIpCountryChanged;
				if (onIpCountryChanged != null)
				{
					onIpCountryChanged();
				}
				else
				{
				}
			}, false);
			LowBatteryPower_t.Install((LowBatteryPower_t x) => {
				Action<int> onLowBatteryPower = SteamUtils.OnLowBatteryPower;
				if (onLowBatteryPower != null)
				{
					onLowBatteryPower(x.MinutesBatteryLeft);
				}
				else
				{
				}
			}, false);
			SteamShutdown_t.Install((SteamShutdown_t x) => {
				Action onSteamShutdown = SteamUtils.OnSteamShutdown;
				if (onSteamShutdown != null)
				{
					onSteamShutdown();
				}
				else
				{
				}
			}, false);
			GamepadTextInputDismissed_t.Install((GamepadTextInputDismissed_t x) => {
				Action<bool> onGamepadTextInputDismissed = SteamUtils.OnGamepadTextInputDismissed;
				if (onGamepadTextInputDismissed != null)
				{
					onGamepadTextInputDismissed(x.Submitted);
				}
				else
				{
				}
			}, false);
		}

		internal static bool IsCallComplete(SteamAPICall_t call, out bool failed)
		{
			failed = false;
			return SteamUtils.Internal.IsAPICallCompleted(call, ref failed);
		}

		public static void SetOverlayNotificationInset(int x, int y)
		{
			SteamUtils.Internal.SetOverlayNotificationInset(x, y);
		}

		public static bool ShowGamepadTextInput(GamepadTextInputMode inputMode, GamepadTextInputLineMode lineInputMode, string description, int maxChars, string existingText = "")
		{
			bool flag = SteamUtils.Internal.ShowGamepadTextInput(inputMode, lineInputMode, description, (uint)maxChars, existingText);
			return flag;
		}

		internal static void Shutdown()
		{
			SteamUtils._internal = null;
		}

		public static void StartVRDashboard()
		{
			SteamUtils.Internal.StartVRDashboard();
		}

		public static event Action<bool> OnGamepadTextInputDismissed;

		public static event Action OnIpCountryChanged;

		public static event Action<int> OnLowBatteryPower;

		public static event Action OnSteamShutdown;
	}
}