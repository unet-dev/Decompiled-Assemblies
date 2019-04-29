using System;

namespace SteamNative
{
	internal struct HSteamPipe
	{
		public int Value;

		public static implicit operator HSteamPipe(int value)
		{
			return new HSteamPipe()
			{
				Value = value
			};
		}

		public static implicit operator Int32(HSteamPipe value)
		{
			return value.Value;
		}
	}
}