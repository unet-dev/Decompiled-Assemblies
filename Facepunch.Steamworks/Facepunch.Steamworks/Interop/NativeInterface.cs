using Facepunch.Steamworks;
using SteamNative;
using System;
using System.IO;

namespace Facepunch.Steamworks.Interop
{
	internal class NativeInterface : IDisposable
	{
		internal SteamApi api;

		internal SteamClient client;

		internal SteamUser user;

		internal SteamApps apps;

		internal SteamAppList applist;

		internal SteamFriends friends;

		internal SteamMatchmakingServers servers;

		internal SteamMatchmaking matchmaking;

		internal SteamInventory inventory;

		internal SteamNetworking networking;

		internal SteamUserStats userstats;

		internal SteamUtils utils;

		internal SteamScreenshots screenshots;

		internal SteamHTTP http;

		internal SteamUGC ugc;

		internal SteamGameServer gameServer;

		internal SteamGameServerStats gameServerStats;

		internal SteamRemoteStorage remoteStorage;

		private bool isServer;

		public NativeInterface()
		{
		}

		public void Dispose()
		{
			if (this.user != null)
			{
				this.user.Dispose();
				this.user = null;
			}
			if (this.utils != null)
			{
				this.utils.Dispose();
				this.utils = null;
			}
			if (this.networking != null)
			{
				this.networking.Dispose();
				this.networking = null;
			}
			if (this.gameServerStats != null)
			{
				this.gameServerStats.Dispose();
				this.gameServerStats = null;
			}
			if (this.http != null)
			{
				this.http.Dispose();
				this.http = null;
			}
			if (this.inventory != null)
			{
				this.inventory.Dispose();
				this.inventory = null;
			}
			if (this.ugc != null)
			{
				this.ugc.Dispose();
				this.ugc = null;
			}
			if (this.apps != null)
			{
				this.apps.Dispose();
				this.apps = null;
			}
			if (this.gameServer != null)
			{
				this.gameServer.Dispose();
				this.gameServer = null;
			}
			if (this.friends != null)
			{
				this.friends.Dispose();
				this.friends = null;
			}
			if (this.servers != null)
			{
				this.servers.Dispose();
				this.servers = null;
			}
			if (this.userstats != null)
			{
				this.userstats.Dispose();
				this.userstats = null;
			}
			if (this.screenshots != null)
			{
				this.screenshots.Dispose();
				this.screenshots = null;
			}
			if (this.remoteStorage != null)
			{
				this.remoteStorage.Dispose();
				this.remoteStorage = null;
			}
			if (this.matchmaking != null)
			{
				this.matchmaking.Dispose();
				this.matchmaking = null;
			}
			if (this.applist != null)
			{
				this.applist.Dispose();
				this.applist = null;
			}
			if (this.client != null)
			{
				this.client.Dispose();
				this.client = null;
			}
			if (this.api != null)
			{
				if (!this.isServer)
				{
					this.api.SteamAPI_Shutdown();
				}
				else
				{
					this.api.SteamGameServer_Shutdown();
				}
				this.api.Dispose();
				this.api = null;
			}
		}

		public void FillInterfaces(BaseSteamworks steamworks, int hpipe, int huser)
		{
			IntPtr intPtr = this.api.SteamInternal_CreateInterface("SteamClient017");
			if (intPtr == IntPtr.Zero)
			{
				throw new Exception("Steam Server: Couldn't load SteamClient017");
			}
			this.client = new SteamClient(steamworks, intPtr);
			this.user = this.client.GetISteamUser(huser, hpipe, "SteamUser019");
			this.utils = this.client.GetISteamUtils(hpipe, "SteamUtils009");
			this.networking = this.client.GetISteamNetworking(huser, hpipe, "SteamNetworking005");
			this.gameServerStats = this.client.GetISteamGameServerStats(huser, hpipe, "SteamGameServerStats001");
			this.http = this.client.GetISteamHTTP(huser, hpipe, "STEAMHTTP_INTERFACE_VERSION002");
			this.inventory = this.client.GetISteamInventory(huser, hpipe, "STEAMINVENTORY_INTERFACE_V002");
			this.ugc = this.client.GetISteamUGC(huser, hpipe, "STEAMUGC_INTERFACE_VERSION010");
			this.apps = this.client.GetISteamApps(huser, hpipe, "STEAMAPPS_INTERFACE_VERSION008");
			this.gameServer = this.client.GetISteamGameServer(huser, hpipe, "SteamGameServer012");
			this.friends = this.client.GetISteamFriends(huser, hpipe, "SteamFriends015");
			this.servers = this.client.GetISteamMatchmakingServers(huser, hpipe, "SteamMatchMakingServers002");
			this.userstats = this.client.GetISteamUserStats(huser, hpipe, "STEAMUSERSTATS_INTERFACE_VERSION011");
			this.screenshots = this.client.GetISteamScreenshots(huser, hpipe, "STEAMSCREENSHOTS_INTERFACE_VERSION003");
			this.remoteStorage = this.client.GetISteamRemoteStorage(huser, hpipe, "STEAMREMOTESTORAGE_INTERFACE_VERSION014");
			this.matchmaking = this.client.GetISteamMatchmaking(huser, hpipe, "SteamMatchMaking009");
			this.applist = this.client.GetISteamAppList(huser, hpipe, "STEAMAPPLIST_INTERFACE_VERSION001");
		}

		internal bool InitClient(BaseSteamworks steamworks)
		{
			if (Server.Instance != null)
			{
				throw new Exception("Steam client should be initialized before steam server - or there's big trouble.");
			}
			this.isServer = false;
			this.api = new SteamApi();
			if (!this.api.SteamAPI_Init())
			{
				Console.Error.WriteLine("InitClient: SteamAPI_Init returned false");
				return false;
			}
			HSteamUser hSteamUser = this.api.SteamAPI_GetHSteamUser();
			HSteamPipe hSteamPipe = this.api.SteamAPI_GetHSteamPipe();
			if (hSteamPipe == 0)
			{
				Console.Error.WriteLine("InitClient: hPipe == 0");
				return false;
			}
			this.FillInterfaces(steamworks, hSteamUser, hSteamPipe);
			if (this.user.IsValid)
			{
				return true;
			}
			Console.Error.WriteLine("InitClient: ISteamUser is null");
			return false;
		}

		internal bool InitServer(BaseSteamworks steamworks, uint IpAddress, ushort usPort, ushort GamePort, ushort QueryPort, int eServerMode, string pchVersionString)
		{
			this.isServer = true;
			this.api = new SteamApi();
			if (!this.api.SteamInternal_GameServer_Init(IpAddress, usPort, GamePort, QueryPort, eServerMode, pchVersionString))
			{
				Console.Error.WriteLine("InitServer: GameServer_Init returned false");
				return false;
			}
			HSteamUser hSteamUser = this.api.SteamGameServer_GetHSteamUser();
			HSteamPipe hSteamPipe = this.api.SteamGameServer_GetHSteamPipe();
			if (hSteamPipe == 0)
			{
				Console.Error.WriteLine("InitServer: hPipe == 0");
				return false;
			}
			this.FillInterfaces(steamworks, hSteamPipe, hSteamUser);
			if (!this.gameServer.IsValid)
			{
				this.gameServer = null;
				throw new Exception("Steam Server: Couldn't load SteamGameServer012");
			}
			return true;
		}
	}
}