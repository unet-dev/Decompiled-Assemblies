using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	internal static class SteamInternal
	{
		internal static IntPtr CreateInterface(string version)
		{
			IntPtr intPtr;
			if (Config.Os != OsType.Windows)
			{
				if (Config.Os != OsType.Posix)
				{
					throw new Exception("this platform isn't supported");
				}
				intPtr = SteamInternal.Posix.SteamInternal_CreateInterface(version);
			}
			else
			{
				intPtr = SteamInternal.Win64.SteamInternal_CreateInterface(version);
			}
			return intPtr;
		}

		internal static IntPtr FindOrCreateGameServerInterface(int steamuser, string versionname)
		{
			IntPtr intPtr;
			if (Config.Os != OsType.Windows)
			{
				if (Config.Os != OsType.Posix)
				{
					throw new Exception("this platform isn't supported");
				}
				intPtr = SteamInternal.Posix.SteamInternal_FindOrCreateGameServerInterface(steamuser, versionname);
			}
			else
			{
				intPtr = SteamInternal.Win64.SteamInternal_FindOrCreateGameServerInterface(steamuser, versionname);
			}
			return intPtr;
		}

		internal static IntPtr FindOrCreateUserInterface(int steamuser, string versionname)
		{
			IntPtr intPtr;
			if (Config.Os != OsType.Windows)
			{
				if (Config.Os != OsType.Posix)
				{
					throw new Exception("this platform isn't supported");
				}
				intPtr = SteamInternal.Posix.SteamInternal_FindOrCreateUserInterface(steamuser, versionname);
			}
			else
			{
				intPtr = SteamInternal.Win64.SteamInternal_FindOrCreateUserInterface(steamuser, versionname);
			}
			return intPtr;
		}

		internal static bool GameServer_Init(uint unIP, ushort usPort, ushort usGamePort, ushort usQueryPort, int eServerMode, string pchVersionString)
		{
			bool flag;
			if (Config.Os != OsType.Windows)
			{
				if (Config.Os != OsType.Posix)
				{
					throw new Exception("this platform isn't supported");
				}
				flag = SteamInternal.Posix.SteamInternal_GameServer_Init(unIP, usPort, usGamePort, usQueryPort, eServerMode, pchVersionString);
			}
			else
			{
				flag = SteamInternal.Win64.SteamInternal_GameServer_Init(unIP, usPort, usGamePort, usQueryPort, eServerMode, pchVersionString);
			}
			return flag;
		}

		internal static class Posix
		{
			[DllImport("libsteam_api", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
			public static extern IntPtr SteamInternal_CreateInterface(string version);

			[DllImport("libsteam_api", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
			public static extern IntPtr SteamInternal_FindOrCreateGameServerInterface(int steamuser, string versionname);

			[DllImport("libsteam_api", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
			public static extern IntPtr SteamInternal_FindOrCreateUserInterface(int steamuser, string versionname);

			[DllImport("libsteam_api", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
			public static extern bool SteamInternal_GameServer_Init(uint unIP, ushort usPort, ushort usGamePort, ushort usQueryPort, int eServerMode, string pchVersionString);
		}

		internal static class Win64
		{
			[DllImport("steam_api64", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
			public static extern IntPtr SteamInternal_CreateInterface(string version);

			[DllImport("steam_api64", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
			public static extern IntPtr SteamInternal_FindOrCreateGameServerInterface(int steamuser, string versionname);

			[DllImport("steam_api64", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
			public static extern IntPtr SteamInternal_FindOrCreateUserInterface(int steamuser, string versionname);

			[DllImport("steam_api64", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
			public static extern bool SteamInternal_GameServer_Init(uint unIP, ushort usPort, ushort usGamePort, ushort usQueryPort, int eServerMode, string pchVersionString);
		}
	}
}