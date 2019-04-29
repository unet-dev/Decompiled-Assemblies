using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct FriendGameInfo_t
	{
		internal ulong GameID;

		internal uint GameIP;

		internal ushort GamePort;

		internal ushort QueryPort;

		internal ulong SteamIDLobby;

		internal static FriendGameInfo_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (FriendGameInfo_t)Marshal.PtrToStructure(p, typeof(FriendGameInfo_t));
			}
			return (FriendGameInfo_t.PackSmall)Marshal.PtrToStructure(p, typeof(FriendGameInfo_t.PackSmall));
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(FriendGameInfo_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(FriendGameInfo_t));
		}

		internal struct PackSmall
		{
			internal ulong GameID;

			internal uint GameIP;

			internal ushort GamePort;

			internal ushort QueryPort;

			internal ulong SteamIDLobby;

			public static implicit operator FriendGameInfo_t(FriendGameInfo_t.PackSmall d)
			{
				FriendGameInfo_t friendGameInfoT = new FriendGameInfo_t()
				{
					GameID = d.GameID,
					GameIP = d.GameIP,
					GamePort = d.GamePort,
					QueryPort = d.QueryPort,
					SteamIDLobby = d.SteamIDLobby
				};
				return friendGameInfoT;
			}
		}
	}
}