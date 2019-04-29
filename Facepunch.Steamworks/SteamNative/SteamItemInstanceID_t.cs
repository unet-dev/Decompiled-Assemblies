using System;

namespace SteamNative
{
	internal struct SteamItemInstanceID_t
	{
		public ulong Value;

		public static implicit operator SteamItemInstanceID_t(ulong value)
		{
			return new SteamItemInstanceID_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(SteamItemInstanceID_t value)
		{
			return value.Value;
		}
	}
}