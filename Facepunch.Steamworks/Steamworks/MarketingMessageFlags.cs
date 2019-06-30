using System;

namespace Steamworks
{
	internal enum MarketingMessageFlags
	{
		None = 0,
		HighPriority = 1,
		PlatformWindows = 2,
		PlatformMac = 4,
		PlatformLinux = 8,
		PlatformRestrictions = 14
	}
}