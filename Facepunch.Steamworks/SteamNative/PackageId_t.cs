using System;

namespace SteamNative
{
	internal struct PackageId_t
	{
		public uint Value;

		public static implicit operator PackageId_t(uint value)
		{
			return new PackageId_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt32(PackageId_t value)
		{
			return value.Value;
		}
	}
}