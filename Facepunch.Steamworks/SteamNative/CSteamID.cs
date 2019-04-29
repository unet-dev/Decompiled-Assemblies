using System;

namespace SteamNative
{
	internal struct CSteamID
	{
		public ulong Value;

		public static implicit operator CSteamID(ulong value)
		{
			return new CSteamID()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(CSteamID value)
		{
			return value.Value;
		}
	}
}