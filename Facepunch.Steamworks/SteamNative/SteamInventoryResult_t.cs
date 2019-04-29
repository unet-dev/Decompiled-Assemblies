using System;

namespace SteamNative
{
	internal struct SteamInventoryResult_t
	{
		public int Value;

		public static implicit operator SteamInventoryResult_t(int value)
		{
			return new SteamInventoryResult_t()
			{
				Value = value
			};
		}

		public static implicit operator Int32(SteamInventoryResult_t value)
		{
			return value.Value;
		}
	}
}