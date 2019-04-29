using System;

namespace SteamNative
{
	internal struct RTime32
	{
		public uint Value;

		public static implicit operator RTime32(uint value)
		{
			return new RTime32()
			{
				Value = value
			};
		}

		public static implicit operator UInt32(RTime32 value)
		{
			return value.Value;
		}
	}
}