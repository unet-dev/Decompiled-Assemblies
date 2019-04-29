using System;

namespace SteamNative
{
	internal struct CGameID
	{
		public ulong Value;

		public static implicit operator CGameID(ulong value)
		{
			return new CGameID()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(CGameID value)
		{
			return value.Value;
		}
	}
}