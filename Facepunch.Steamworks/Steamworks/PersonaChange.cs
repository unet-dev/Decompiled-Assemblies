using System;

namespace Steamworks
{
	internal enum PersonaChange
	{
		Name = 1,
		Status = 2,
		ComeOnline = 4,
		GoneOffline = 8,
		GamePlayed = 16,
		GameServer = 32,
		Avatar = 64,
		JoinedSource = 128,
		LeftSource = 256,
		RelationshipChanged = 512,
		NameFirstSet = 1024,
		Broadcast = 2048,
		Nickname = 4096,
		SteamLevel = 8192,
		RichPresence = 16384
	}
}