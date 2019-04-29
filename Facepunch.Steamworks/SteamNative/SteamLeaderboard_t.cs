using System;

namespace SteamNative
{
	internal struct SteamLeaderboard_t
	{
		public ulong Value;

		public static implicit operator SteamLeaderboard_t(ulong value)
		{
			return new SteamLeaderboard_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(SteamLeaderboard_t value)
		{
			return value.Value;
		}
	}
}