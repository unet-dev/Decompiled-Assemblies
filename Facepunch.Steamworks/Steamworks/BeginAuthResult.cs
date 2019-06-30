using System;

namespace Steamworks
{
	public enum BeginAuthResult
	{
		OK,
		InvalidTicket,
		DuplicateRequest,
		InvalidVersion,
		GameMismatch,
		ExpiredTicket
	}
}