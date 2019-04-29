using System;

namespace SteamNative
{
	internal struct HSteamUser
	{
		public int Value;

		public static implicit operator HSteamUser(int value)
		{
			return new HSteamUser()
			{
				Value = value
			};
		}

		public static implicit operator Int32(HSteamUser value)
		{
			return value.Value;
		}
	}
}