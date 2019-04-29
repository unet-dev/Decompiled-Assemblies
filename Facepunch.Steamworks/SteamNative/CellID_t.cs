using System;

namespace SteamNative
{
	internal struct CellID_t
	{
		public uint Value;

		public static implicit operator CellID_t(uint value)
		{
			return new CellID_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt32(CellID_t value)
		{
			return value.Value;
		}
	}
}