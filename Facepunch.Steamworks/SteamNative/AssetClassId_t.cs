using System;

namespace SteamNative
{
	internal struct AssetClassId_t
	{
		public ulong Value;

		public static implicit operator AssetClassId_t(ulong value)
		{
			return new AssetClassId_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(AssetClassId_t value)
		{
			return value.Value;
		}
	}
}