using System;

namespace SteamNative
{
	internal struct AppId_t
	{
		public uint Value;

		public static implicit operator AppId_t(uint value)
		{
			return new AppId_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt32(AppId_t value)
		{
			return value.Value;
		}
	}
}