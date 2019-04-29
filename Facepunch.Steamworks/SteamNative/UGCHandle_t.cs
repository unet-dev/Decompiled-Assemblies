using System;

namespace SteamNative
{
	internal struct UGCHandle_t
	{
		public ulong Value;

		public static implicit operator UGCHandle_t(ulong value)
		{
			return new UGCHandle_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(UGCHandle_t value)
		{
			return value.Value;
		}
	}
}