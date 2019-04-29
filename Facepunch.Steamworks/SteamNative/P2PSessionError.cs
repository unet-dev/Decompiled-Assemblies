using System;

namespace SteamNative
{
	internal enum P2PSessionError
	{
		None,
		NotRunningApp,
		NoRightsToApp,
		DestinationNotLoggedIn,
		Timeout,
		Max
	}
}