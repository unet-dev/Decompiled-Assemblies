using System;

namespace SteamNative
{
	internal struct SNetSocket_t
	{
		public uint Value;

		public static implicit operator SNetSocket_t(uint value)
		{
			return new SNetSocket_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt32(SNetSocket_t value)
		{
			return value.Value;
		}
	}
}