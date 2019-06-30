using System;

namespace Steamworks
{
	internal enum SteamAPICallFailure
	{
		None = -1,
		SteamGone = 0,
		NetworkFailure = 1,
		InvalidHandle = 2,
		MismatchedCallback = 3
	}
}