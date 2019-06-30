using Steamworks.Data;
using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	internal static class SteamAPI
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
				hSteamPipe = SteamAPI.Posix.SteamAPI_GetHSteamPipe();
			}
			else
			{
				hSteamPipe = SteamAPI.Win64.SteamAPI_GetHSteamPipe();
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
				hSteamUser = SteamAPI.Posix.SteamAPI_GetHSteamUser();
			}
			else
			{
				hSteamUser = SteamAPI.Win64.SteamAPI_GetHSteamUser();
			}
			return hSteamUser;
		}

		internal static bool Init()
		{
			bool flag;
			if (Config.Os != OsType.Windows)
			{
				if (Config.Os != OsType.Posix)
				{
					throw new Exception("this platform isn't supported");
				}
				flag = SteamAPI.Posix.SteamAPI_Init();
			}
			else
			{
				flag = SteamAPI.Win64.SteamAPI_Init();
			}
			return flag;
		}

		internal static void RegisterCallback(IntPtr pCallback, int callback)
		{
			if (Config.Os != OsType.Windows)
			{
				if (Config.Os != OsType.Posix)
				{
					throw new Exception("this platform isn't supported");
				}
				SteamAPI.Posix.SteamAPI_RegisterCallback(pCallback, callback);
			}
			else
			{
				SteamAPI.Win64.SteamAPI_RegisterCallback(pCallback, callback);
			}
		}

		internal static void RegisterCallResult(IntPtr pCallback, SteamAPICall_t callback)
		{
			if (Config.Os != OsType.Windows)
			{
				if (Config.Os != OsType.Posix)
				{
					throw new Exception("this platform isn't supported");
				}
				SteamAPI.Posix.SteamAPI_RegisterCallResult(pCallback, callback);
			}
			else
			{
				SteamAPI.Win64.SteamAPI_RegisterCallResult(pCallback, callback);
			}
		}

		internal static bool RestartAppIfNecessary(uint unOwnAppID)
		{
			bool flag;
			if (Config.Os != OsType.Windows)
			{
				if (Config.Os != OsType.Posix)
				{
					throw new Exception("this platform isn't supported");
				}
				flag = SteamAPI.Posix.SteamAPI_RestartAppIfNecessary(unOwnAppID);
			}
			else
			{
				flag = SteamAPI.Win64.SteamAPI_RestartAppIfNecessary(unOwnAppID);
			}
			return flag;
		}

		internal static void RunCallbacks()
		{
			if (Config.Os != OsType.Windows)
			{
				if (Config.Os != OsType.Posix)
				{
					throw new Exception("this platform isn't supported");
				}
				SteamAPI.Posix.SteamAPI_RunCallbacks();
			}
			else
			{
				SteamAPI.Win64.SteamAPI_RunCallbacks();
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
				SteamAPI.Posix.SteamAPI_Shutdown();
			}
			else
			{
				SteamAPI.Win64.SteamAPI_Shutdown();
			}
		}

		internal static void UnregisterCallback(IntPtr pCallback)
		{
			if (Config.Os != OsType.Windows)
			{
				if (Config.Os != OsType.Posix)
				{
					throw new Exception("this platform isn't supported");
				}
				SteamAPI.Posix.SteamAPI_UnregisterCallback(pCallback);
			}
			else
			{
				SteamAPI.Win64.SteamAPI_UnregisterCallback(pCallback);
			}
		}

		internal static void UnregisterCallResult(IntPtr pCallback, SteamAPICall_t callback)
		{
			if (Config.Os != OsType.Windows)
			{
				if (Config.Os != OsType.Posix)
				{
					throw new Exception("this platform isn't supported");
				}
				SteamAPI.Posix.SteamAPI_UnregisterCallResult(pCallback, callback);
			}
			else
			{
				SteamAPI.Win64.SteamAPI_UnregisterCallResult(pCallback, callback);
			}
		}

		internal static class Posix
		{
			[DllImport("libsteam_api", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
			public static extern HSteamPipe SteamAPI_GetHSteamPipe();

			[DllImport("libsteam_api", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
			public static extern HSteamUser SteamAPI_GetHSteamUser();

			[DllImport("libsteam_api", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
			public static extern bool SteamAPI_Init();

			[DllImport("libsteam_api", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
			public static extern void SteamAPI_RegisterCallback(IntPtr pCallback, int callback);

			[DllImport("libsteam_api", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
			public static extern void SteamAPI_RegisterCallResult(IntPtr pCallback, SteamAPICall_t callback);

			[DllImport("libsteam_api", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
			public static extern bool SteamAPI_RestartAppIfNecessary(uint unOwnAppID);

			[DllImport("libsteam_api", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
			public static extern void SteamAPI_RunCallbacks();

			[DllImport("libsteam_api", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
			public static extern void SteamAPI_Shutdown();

			[DllImport("libsteam_api", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
			public static extern void SteamAPI_UnregisterCallback(IntPtr pCallback);

			[DllImport("libsteam_api", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
			public static extern void SteamAPI_UnregisterCallResult(IntPtr pCallback, SteamAPICall_t callback);
		}

		internal static class Win64
		{
			[DllImport("steam_api64", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
			public static extern HSteamPipe SteamAPI_GetHSteamPipe();

			[DllImport("steam_api64", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
			public static extern HSteamUser SteamAPI_GetHSteamUser();

			[DllImport("steam_api64", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
			public static extern bool SteamAPI_Init();

			[DllImport("steam_api64", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
			public static extern void SteamAPI_RegisterCallback(IntPtr pCallback, int callback);

			[DllImport("steam_api64", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
			public static extern void SteamAPI_RegisterCallResult(IntPtr pCallback, SteamAPICall_t callback);

			[DllImport("steam_api64", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
			public static extern bool SteamAPI_RestartAppIfNecessary(uint unOwnAppID);

			[DllImport("steam_api64", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
			public static extern void SteamAPI_RunCallbacks();

			[DllImport("steam_api64", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
			public static extern void SteamAPI_Shutdown();

			[DllImport("steam_api64", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
			public static extern void SteamAPI_UnregisterCallback(IntPtr pCallback);

			[DllImport("steam_api64", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
			public static extern void SteamAPI_UnregisterCallResult(IntPtr pCallback, SteamAPICall_t callback);
		}
	}
}