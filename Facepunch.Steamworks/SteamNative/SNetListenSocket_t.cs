using System;

namespace SteamNative
{
	internal struct SNetListenSocket_t
	{
		public uint Value;

		public static implicit operator SNetListenSocket_t(uint value)
		{
			return new SNetListenSocket_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt32(SNetListenSocket_t value)
		{
			return value.Value;
		}
	}
}