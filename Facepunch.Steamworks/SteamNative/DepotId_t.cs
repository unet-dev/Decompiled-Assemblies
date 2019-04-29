using System;

namespace SteamNative
{
	internal struct DepotId_t
	{
		public uint Value;

		public static implicit operator DepotId_t(uint value)
		{
			return new DepotId_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt32(DepotId_t value)
		{
			return value.Value;
		}
	}
}