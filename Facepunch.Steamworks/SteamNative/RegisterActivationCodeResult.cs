using System;

namespace SteamNative
{
	internal enum RegisterActivationCodeResult
	{
		ResultOK,
		ResultFail,
		ResultAlreadyRegistered,
		ResultTimeout,
		AlreadyOwned
	}
}