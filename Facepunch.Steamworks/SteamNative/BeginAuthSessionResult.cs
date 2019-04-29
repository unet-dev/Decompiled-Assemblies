using System;

namespace SteamNative
{
	internal enum BeginAuthSessionResult
	{
		OK,
		InvalidTicket,
		DuplicateRequest,
		InvalidVersion,
		GameMismatch,
		ExpiredTicket
	}
}