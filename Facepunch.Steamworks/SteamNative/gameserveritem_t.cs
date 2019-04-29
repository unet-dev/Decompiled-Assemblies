using System;
using System.Runtime.InteropServices;

namespace SteamNative
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

		internal static gameserveritem_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (gameserveritem_t)Marshal.PtrToStructure(p, typeof(gameserveritem_t));
			}
			return (gameserveritem_t.PackSmall)Marshal.PtrToStructure(p, typeof(gameserveritem_t.PackSmall));
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(gameserveritem_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(gameserveritem_t));
		}

		internal struct PackSmall
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

			public static implicit operator gameserveritem_t(gameserveritem_t.PackSmall d)
			{
				gameserveritem_t gameserveritemT = new gameserveritem_t()
				{
					NetAdr = d.NetAdr,
					Ping = d.Ping,
					HadSuccessfulResponse = d.HadSuccessfulResponse,
					DoNotRefresh = d.DoNotRefresh,
					GameDir = d.GameDir,
					Map = d.Map,
					GameDescription = d.GameDescription,
					AppID = d.AppID,
					Players = d.Players,
					MaxPlayers = d.MaxPlayers,
					BotPlayers = d.BotPlayers,
					Password = d.Password,
					Secure = d.Secure,
					TimeLastPlayed = d.TimeLastPlayed,
					ServerVersion = d.ServerVersion,
					ServerName = d.ServerName,
					GameTags = d.GameTags,
					SteamID = d.SteamID
				};
				return gameserveritemT;
			}
		}
	}
}