using System;

namespace SteamNative
{
	internal struct PublishedFileUpdateHandle_t
	{
		public ulong Value;

		public static implicit operator PublishedFileUpdateHandle_t(ulong value)
		{
			return new PublishedFileUpdateHandle_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(PublishedFileUpdateHandle_t value)
		{
			return value.Value;
		}
	}
}