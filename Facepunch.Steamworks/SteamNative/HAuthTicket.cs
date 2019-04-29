using System;

namespace SteamNative
{
	internal struct HAuthTicket
	{
		public uint Value;

		public static implicit operator HAuthTicket(uint value)
		{
			return new HAuthTicket()
			{
				Value = value
			};
		}

		public static implicit operator UInt32(HAuthTicket value)
		{
			return value.Value;
		}
	}
}