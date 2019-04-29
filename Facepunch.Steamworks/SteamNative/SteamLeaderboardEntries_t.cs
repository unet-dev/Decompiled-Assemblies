using System;

namespace SteamNative
{
	internal struct SteamLeaderboardEntries_t
	{
		public ulong Value;

		public static implicit operator SteamLeaderboardEntries_t(ulong value)
		{
			return new SteamLeaderboardEntries_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(SteamLeaderboardEntries_t value)
		{
			return value.Value;
		}
	}
}