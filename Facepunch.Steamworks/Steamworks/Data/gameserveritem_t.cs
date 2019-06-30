using System;
using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	internal struct gameserveritem_t
	{
		internal servernetadr_t NetAdr;

		internal int Ping;

		internal bool HadSuccessfulResponse;

		internal bool DoNotRefresh;

		internal string GameDir;

		internal string Map;

		internal string GameDescription;

		internal uint AppID;

		internal int Players;

		internal int MaxPlayers;

		internal int BotPlayers;

		internal bool Password;

		internal bool Secure;

		internal uint TimeLastPlayed;

		internal int ServerVersion;

		internal string ServerName;

		internal string GameTags;

		internal ulong SteamID;

		internal static gameserveritem_t Fill(IntPtr p)
		{
			return (gameserveritem_t)Marshal.PtrToStructure(p, typeof(gameserveritem_t));
		}
	}
}