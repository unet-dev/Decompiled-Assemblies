using System;

namespace SteamNative
{
	internal struct PhysicalItemId_t
	{
		public uint Value;

		public static implicit operator PhysicalItemId_t(uint value)
		{
			return new PhysicalItemId_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt32(PhysicalItemId_t value)
		{
			return value.Value;
		}
	}
}