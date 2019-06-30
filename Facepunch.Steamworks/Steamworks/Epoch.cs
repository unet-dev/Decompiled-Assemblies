using System;

namespace Steamworks
{
	internal static class Epoch
	{
		private readonly static DateTime epoch;

		public static int Current
		{
			get
			{
				return (int)DateTime.UtcNow.Subtract(Epoch.epoch).TotalSeconds;
			}
		}

		static Epoch()
		{
			Epoch.epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		}

		public static uint FromDateTime(DateTime dt)
		{
			return (uint)dt.Subtract(Epoch.epoch).TotalSeconds;
		}

		public static DateTime ToDateTime(Decimal unixTime)
		{
			return Epoch.epoch.AddSeconds((double)((long)unixTime));
		}
	}
}