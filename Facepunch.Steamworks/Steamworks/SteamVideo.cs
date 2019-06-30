using Steamworks.Data;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Steamworks
{
	public static class SteamVideo
	{
		private static ISteamVideo _internal;

		internal static ISteamVideo Internal
		{
			get
			{
				if (SteamVideo._internal == null)
				{
					SteamVideo._internal = new ISteamVideo();
					SteamVideo._internal.Init();
				}
				return SteamVideo._internal;
			}
		}

		public static bool IsBroadcasting
		{
			get
			{
				int num = 0;
				return SteamVideo.Internal.IsBroadcasting(ref num);
			}
		}

		public static int NumViewers
		{
			get
			{
				int num;
				int num1 = 0;
				num = (SteamVideo.Internal.IsBroadcasting(ref num1) ? num1 : 0);
				return num;
			}
		}

		internal static void InstallEvents()
		{
			BroadcastUploadStart_t.Install((BroadcastUploadStart_t x) => {
				Action onBroadcastStarted = SteamVideo.OnBroadcastStarted;
				if (onBroadcastStarted != null)
				{
					onBroadcastStarted();
				}
				else
				{
				}
			}, false);
			BroadcastUploadStop_t.Install((BroadcastUploadStop_t x) => {
				Action<BroadcastUploadResult> onBroadcastStopped = SteamVideo.OnBroadcastStopped;
				if (onBroadcastStopped != null)
				{
					onBroadcastStopped(x.Result);
				}
				else
				{
				}
			}, false);
		}

		internal static void Shutdown()
		{
			SteamVideo._internal = null;
		}

		public static event Action OnBroadcastStarted;

		public static event Action<BroadcastUploadResult> OnBroadcastStopped;
	}
}