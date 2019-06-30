using Steamworks.Data;
using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	internal static class SteamGameServer
	{
		internal static HSteamPipe GetHSteamPipe()
		{
			HSteamPipe hSteamPipe;
			if (Config.Os != OsType.Windows)
			{
				if (Config.Os != OsType.Posix)
				{
					throw new Exception("this platform isn't supported");
				}
				hSteamPipe = SteamGameServer.Posix.SteamGameServer_GetHSteamPipe();
			}
			else
			{
				hSteamPipe = SteamGameServer.Win64.SteamGameServer_GetHSteamPipe();
			}
			return hSteamPipe;
		}

		internal static HSteamUser GetHSteamUser()
		{
			HSteamUser hSteamUser;
			if (Config.Os != OsType.Windows)
			{
				if (Config.Os != OsType.Posix)
				{
					throw new Exception("this platform isn't supported");
				}
				hSteamUser = SteamGameServer.Posix.SteamGameServer_GetHSteamUser();
			}
			else
			{
				hSteamUser = SteamGameServer.Win64.SteamGameServer_GetHSteamUser();
			}
			return hSteamUser;
		}

		internal static void RunCallbacks()
		{
			if (Config.Os != OsType.Windows)
			{
				if (Config.Os != OsType.Posix)
				{
					throw new Exception("this platform isn't supported");
				}
				SteamGameServer.Posix.SteamGameServer_RunCallbacks();
			}
			else
			{
				SteamGameServer.Win64.SteamGameServer_RunCallbacks();
			}
		}

		internal static void Shutdown()
		{
			if (Config.Os != OsType.Windows)
			{
				if (Config.Os != OsType.Posix)
				{
					throw new Exception("this platform isn't supported");
				}
				SteamGameServer.Posix.SteamGameServer_Shutdown();
			}
			else
			{
				SteamGameServer.Win64.SteamGameServer_Shutdown();
			}
		}

		internal static class Posix
		{
			[DllImport("libsteam_api", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
			public static extern HSteamPipe SteamGameServer_GetHSteamPipe();

			[DllImport("libsteam_api", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
			public static extern HSteamUser SteamGameServer_GetHSteamUser();

			[DllImport("libsteam_api", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
			public static extern void SteamGameServer_RunCallbacks();

			[DllImport("libsteam_api", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
			public static extern void SteamGameServer_Shutdown();
		}

		internal static class Win64
		{
			[DllImport("steam_api64", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
			public static extern HSteamPipe SteamGameServer_GetHSteamPipe();

			[DllImport("steam_api64", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
			public static extern HSteamUser SteamGameServer_GetHSteamUser();

			[DllImport("steam_api64", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
			public static extern void SteamGameServer_RunCallbacks();

			[DllImport("steam_api64", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
			public static extern void SteamGameServer_Shutdown();
		}
	}
}