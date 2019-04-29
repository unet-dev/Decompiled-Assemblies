using System;

namespace SteamNative
{
	internal struct SteamAPICall_t
	{
		public ulong Value;

		public static implicit operator SteamAPICall_t(ulong value)
		{
			return new SteamAPICall_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(SteamAPICall_t value)
		{
			return value.Value;
		}
	}
}