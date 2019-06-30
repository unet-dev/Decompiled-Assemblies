using System;

namespace Steamworks
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