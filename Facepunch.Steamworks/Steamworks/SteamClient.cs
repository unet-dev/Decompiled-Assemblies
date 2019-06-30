using Steamworks.ServerList;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Steamworks
{
	public static class SteamClient
	{
		private static bool initialized;

		private static List<SteamInterface> openIterfaces;

		public static Action<Exception> OnCallbackException;

		public static Steamworks.AppId AppId
		{
			get;
			internal set;
		}

		public static bool IsLoggedOn
		{
			get
			{
				return SteamUser.Internal.BLoggedOn();
			}
		}

		public static bool IsValid
		{
			get
			{
				return SteamClient.initialized;
			}
		}

		public static string Name
		{
			get
			{
				return SteamFriends.Internal.GetPersonaName();
			}
		}

		public static FriendState State
		{
			get
			{
				return SteamFriends.Internal.GetPersonaState();
			}
		}

		public static Steamworks.SteamId SteamId
		{
			get
			{
				return SteamUser.Internal.GetSteamID();
			}
		}

		static SteamClient()
		{
			SteamClient.openIterfaces = new List<SteamInterface>();
		}

		public static void Init(uint appid)
		{
			if (IntPtr.Size != 8)
			{
				throw new Exception("Only 64bit processes are currently supported");
			}
			Environment.SetEnvironmentVariable("SteamAppId", appid.ToString());
			Environment.SetEnvironmentVariable("SteamGameId", appid.ToString());
			if (!SteamAPI.Init())
			{
				throw new Exception("SteamApi_Init returned false. Steam isn't running, couldn't find Steam, AppId is ureleased, Don't own AppId.");
			}
			SteamClient.AppId = appid;
			SteamClient.initialized = true;
			SteamApps.InstallEvents();
			SteamUtils.InstallEvents();
			SteamParental.InstallEvents();
			SteamMusic.InstallEvents();
			SteamVideo.InstallEvents();
			SteamUser.InstallEvents();
			SteamFriends.InstallEvents();
			SteamScreenshots.InstallEvents();
			SteamUserStats.InstallEvents();
			SteamInventory.InstallEvents();
			SteamNetworking.InstallEvents();
			SteamMatchmaking.InstallEvents();
			SteamParties.InstallEvents();
			SteamNetworkingSockets.InstallEvents();
			SteamClient.RunCallbacksAsync();
		}

		internal static void RegisterCallback(IntPtr intPtr, int callbackId)
		{
			SteamAPI.RegisterCallback(intPtr, callbackId);
		}

		public static void RunCallbacks()
		{
			try
			{
				SteamAPI.RunCallbacks();
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Action<Exception> onCallbackException = SteamClient.OnCallbackException;
				if (onCallbackException != null)
				{
					onCallbackException(exception);
				}
				else
				{
				}
			}
		}

		internal static async void RunCallbacksAsync()
		{
			while (SteamClient.IsValid)
			{
				await Task.Delay(16);
				SteamClient.RunCallbacks();
			}
		}

		public static void Shutdown()
		{
			Event.DisposeAllClient();
			SteamClient.initialized = false;
			SteamClient.ShutdownInterfaces();
			SteamApps.Shutdown();
			SteamUtils.Shutdown();
			SteamParental.Shutdown();
			SteamMusic.Shutdown();
			SteamVideo.Shutdown();
			SteamUser.Shutdown();
			SteamFriends.Shutdown();
			SteamScreenshots.Shutdown();
			SteamUserStats.Shutdown();
			SteamInventory.Shutdown();
			SteamNetworking.Shutdown();
			SteamMatchmaking.Shutdown();
			SteamParties.Shutdown();
			SteamNetworkingUtils.Shutdown();
			SteamNetworkingSockets.Shutdown();
			Base.Shutdown();
			SteamAPI.Shutdown();
		}

		internal static void ShutdownInterfaces()
		{
			foreach (SteamInterface openIterface in SteamClient.openIterfaces)
			{
				openIterface.Shutdown();
			}
			SteamClient.openIterfaces.Clear();
		}

		internal static void UnregisterCallback(IntPtr intPtr)
		{
			SteamAPI.UnregisterCallback(intPtr);
		}

		internal static void WatchInterface(SteamInterface steamInterface)
		{
			if (SteamClient.openIterfaces.Contains(steamInterface))
			{
				throw new Exception("openIterfaces already contains interface!");
			}
			SteamClient.openIterfaces.Add(steamInterface);
		}
	}
}