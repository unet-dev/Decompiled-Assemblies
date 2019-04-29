using System;

namespace SteamNative
{
	internal struct UGCQueryHandle_t
	{
		public ulong Value;

		public static implicit operator UGCQueryHandle_t(ulong value)
		{
			return new UGCQueryHandle_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(UGCQueryHandle_t value)
		{
			return value.Value;
		}
	}
}