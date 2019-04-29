using System;

namespace SteamNative
{
	internal struct SteamInventoryUpdateHandle_t
	{
		public ulong Value;

		public static implicit operator SteamInventoryUpdateHandle_t(ulong value)
		{
			return new SteamInventoryUpdateHandle_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(SteamInventoryUpdateHandle_t value)
		{
			return value.Value;
		}
	}
}