using System;

namespace SteamNative
{
	internal struct GID_t
	{
		public ulong Value;

		public static implicit operator GID_t(ulong value)
		{
			return new GID_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(GID_t value)
		{
			return value.Value;
		}
	}
}