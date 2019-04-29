using System;

namespace SteamNative
{
	internal struct SiteId_t
	{
		public ulong Value;

		public static implicit operator SiteId_t(ulong value)
		{
			return new SiteId_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(SiteId_t value)
		{
			return value.Value;
		}
	}
}