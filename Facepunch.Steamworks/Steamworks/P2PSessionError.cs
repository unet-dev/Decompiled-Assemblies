using System;

namespace Steamworks
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