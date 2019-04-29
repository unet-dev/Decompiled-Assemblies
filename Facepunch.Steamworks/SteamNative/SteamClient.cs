using Facepunch.Steamworks;
using System;

namespace SteamNative
{
	internal class SteamClient : IDisposable
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

		internal SteamClient(BaseSteamworks steamworks, IntPtr pointer)
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

		public bool BReleaseSteamPipe(HSteamPipe hSteamPipe)
		{
			return this.platform.ISteamClient_BReleaseSteamPipe(hSteamPipe.Value);
		}

		public bool BShutdownIfAllPipesClosed()
		{
			return this.platform.ISteamClient_BShutdownIfAllPipesClosed();
		}

		public HSteamUser ConnectToGlobalUser(HSteamPipe hSteamPipe)
		{
			return this.platform.ISteamClient_ConnectToGlobalUser(hSteamPipe.Value);
		}

		public HSteamUser CreateLocalUser(out HSteamPipe phSteamPipe, AccountType eAccountType)
		{
			phSteamPipe = new HSteamPipe();
			return this.platform.ISteamClient_CreateLocalUser(out phSteamPipe.Value, eAccountType);
		}

		public HSteamPipe CreateSteamPipe()
		{
			return this.platform.ISteamClient_CreateSteamPipe();
		}

		public virtual void Dispose()
		{
			if (this.platform != null)
			{
				this.platform.Dispose();
				this.platform = null;
			}
		}

		public uint GetIPCCallCount()
		{
			return this.platform.ISteamClient_GetIPCCallCount();
		}

		public SteamAppList GetISteamAppList(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			IntPtr intPtr = this.platform.ISteamClient_GetISteamAppList(hSteamUser.Value, hSteamPipe.Value, pchVersion);
			return new SteamAppList(this.steamworks, intPtr);
		}

		public SteamApps GetISteamApps(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			IntPtr intPtr = this.platform.ISteamClient_GetISteamApps(hSteamUser.Value, hSteamPipe.Value, pchVersion);
			return new SteamApps(this.steamworks, intPtr);
		}

		public SteamController GetISteamController(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			IntPtr intPtr = this.platform.ISteamClient_GetISteamController(hSteamUser.Value, hSteamPipe.Value, pchVersion);
			return new SteamController(this.steamworks, intPtr);
		}

		public SteamFriends GetISteamFriends(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			IntPtr intPtr = this.platform.ISteamClient_GetISteamFriends(hSteamUser.Value, hSteamPipe.Value, pchVersion);
			return new SteamFriends(this.steamworks, intPtr);
		}

		public SteamGameServer GetISteamGameServer(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			IntPtr intPtr = this.platform.ISteamClient_GetISteamGameServer(hSteamUser.Value, hSteamPipe.Value, pchVersion);
			return new SteamGameServer(this.steamworks, intPtr);
		}

		public SteamGameServerStats GetISteamGameServerStats(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
		{
			IntPtr intPtr = this.platform.ISteamClient_GetISteamGameServerStats(hSteamuser.Value, hSteamPipe.Value, pchVersion);
			return new SteamGameServerStats(this.steamworks, intPtr);
		}

		public IntPtr GetISteamGenericInterface(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			return this.platform.ISteamClient_GetISteamGenericInterface(hSteamUser.Value, hSteamPipe.Value, pchVersion);
		}

		public SteamHTMLSurface GetISteamHTMLSurface(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
		{
			IntPtr intPtr = this.platform.ISteamClient_GetISteamHTMLSurface(hSteamuser.Value, hSteamPipe.Value, pchVersion);
			return new SteamHTMLSurface(this.steamworks, intPtr);
		}

		public SteamHTTP GetISteamHTTP(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
		{
			IntPtr intPtr = this.platform.ISteamClient_GetISteamHTTP(hSteamuser.Value, hSteamPipe.Value, pchVersion);
			return new SteamHTTP(this.steamworks, intPtr);
		}

		public SteamInventory GetISteamInventory(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
		{
			IntPtr intPtr = this.platform.ISteamClient_GetISteamInventory(hSteamuser.Value, hSteamPipe.Value, pchVersion);
			return new SteamInventory(this.steamworks, intPtr);
		}

		public SteamMatchmaking GetISteamMatchmaking(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			IntPtr intPtr = this.platform.ISteamClient_GetISteamMatchmaking(hSteamUser.Value, hSteamPipe.Value, pchVersion);
			return new SteamMatchmaking(this.steamworks, intPtr);
		}

		public SteamMatchmakingServers GetISteamMatchmakingServers(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			IntPtr intPtr = this.platform.ISteamClient_GetISteamMatchmakingServers(hSteamUser.Value, hSteamPipe.Value, pchVersion);
			return new SteamMatchmakingServers(this.steamworks, intPtr);
		}

		public SteamMusic GetISteamMusic(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
		{
			IntPtr intPtr = this.platform.ISteamClient_GetISteamMusic(hSteamuser.Value, hSteamPipe.Value, pchVersion);
			return new SteamMusic(this.steamworks, intPtr);
		}

		public SteamMusicRemote GetISteamMusicRemote(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
		{
			IntPtr intPtr = this.platform.ISteamClient_GetISteamMusicRemote(hSteamuser.Value, hSteamPipe.Value, pchVersion);
			return new SteamMusicRemote(this.steamworks, intPtr);
		}

		public SteamNetworking GetISteamNetworking(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			IntPtr intPtr = this.platform.ISteamClient_GetISteamNetworking(hSteamUser.Value, hSteamPipe.Value, pchVersion);
			return new SteamNetworking(this.steamworks, intPtr);
		}

		public SteamParentalSettings GetISteamParentalSettings(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
		{
			IntPtr intPtr = this.platform.ISteamClient_GetISteamParentalSettings(hSteamuser.Value, hSteamPipe.Value, pchVersion);
			return new SteamParentalSettings(this.steamworks, intPtr);
		}

		public SteamRemoteStorage GetISteamRemoteStorage(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
		{
			IntPtr intPtr = this.platform.ISteamClient_GetISteamRemoteStorage(hSteamuser.Value, hSteamPipe.Value, pchVersion);
			return new SteamRemoteStorage(this.steamworks, intPtr);
		}

		public SteamScreenshots GetISteamScreenshots(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
		{
			IntPtr intPtr = this.platform.ISteamClient_GetISteamScreenshots(hSteamuser.Value, hSteamPipe.Value, pchVersion);
			return new SteamScreenshots(this.steamworks, intPtr);
		}

		public SteamUGC GetISteamUGC(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			IntPtr intPtr = this.platform.ISteamClient_GetISteamUGC(hSteamUser.Value, hSteamPipe.Value, pchVersion);
			return new SteamUGC(this.steamworks, intPtr);
		}

		public SteamUser GetISteamUser(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			IntPtr intPtr = this.platform.ISteamClient_GetISteamUser(hSteamUser.Value, hSteamPipe.Value, pchVersion);
			return new SteamUser(this.steamworks, intPtr);
		}

		public SteamUserStats GetISteamUserStats(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			IntPtr intPtr = this.platform.ISteamClient_GetISteamUserStats(hSteamUser.Value, hSteamPipe.Value, pchVersion);
			return new SteamUserStats(this.steamworks, intPtr);
		}

		public SteamUtils GetISteamUtils(HSteamPipe hSteamPipe, string pchVersion)
		{
			IntPtr intPtr = this.platform.ISteamClient_GetISteamUtils(hSteamPipe.Value, pchVersion);
			return new SteamUtils(this.steamworks, intPtr);
		}

		public SteamVideo GetISteamVideo(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
		{
			IntPtr intPtr = this.platform.ISteamClient_GetISteamVideo(hSteamuser.Value, hSteamPipe.Value, pchVersion);
			return new SteamVideo(this.steamworks, intPtr);
		}

		public void ReleaseUser(HSteamPipe hSteamPipe, HSteamUser hUser)
		{
			this.platform.ISteamClient_ReleaseUser(hSteamPipe.Value, hUser.Value);
		}

		public void SetLocalIPBinding(uint unIP, ushort usPort)
		{
			this.platform.ISteamClient_SetLocalIPBinding(unIP, usPort);
		}

		public void SetWarningMessageHook(IntPtr pFunction)
		{
			this.platform.ISteamClient_SetWarningMessageHook(pFunction);
		}
	}
}