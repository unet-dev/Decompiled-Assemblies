using System;

namespace Steamworks
{
	internal enum MarketNotAllowedReasonFlags
	{
		None = 0,
		TemporaryFailure = 1,
		AccountDisabled = 2,
		AccountLockedDown = 4,
		AccountLimited = 8,
		TradeBanned = 16,
		AccountNotTrusted = 32,
		SteamGuardNotEnabled = 64,
		SteamGuardOnlyRecentlyEnabled = 128,
		RecentPasswordReset = 256,
		NewPaymentMethod = 512,
		InvalidCookie = 1024,
		UsingNewDevice = 2048,
		RecentSelfRefund = 4096,
		NewPaymentMethodCannotBeVerified = 8192,
		NoRecentPurchases = 16384,
		AcceptedWalletGift = 32768
	}
}