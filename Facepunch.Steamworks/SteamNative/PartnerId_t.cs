using System;

namespace SteamNative
{
	internal struct PartnerId_t
	{
		public uint Value;

		public static implicit operator PartnerId_t(uint value)
		{
			return new PartnerId_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt32(PartnerId_t value)
		{
			return value.Value;
		}
	}
}