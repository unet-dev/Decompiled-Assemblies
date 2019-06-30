using System;
using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	internal struct FriendGameInfo_t
	{
		internal GameId GameID;

		internal uint GameIP;

		internal ushort GamePort;

		internal ushort QueryPort;

		internal ulong SteamIDLobby;

		internal static FriendGameInfo_t Fill(IntPtr p)
		{
			return (FriendGameInfo_t)Marshal.PtrToStructure(p, typeof(FriendGameInfo_t));
		}
	}
}