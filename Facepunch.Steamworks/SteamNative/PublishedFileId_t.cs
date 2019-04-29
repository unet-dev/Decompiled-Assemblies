using System;

namespace SteamNative
{
	internal struct PublishedFileId_t
	{
		public ulong Value;

		public static implicit operator PublishedFileId_t(ulong value)
		{
			return new PublishedFileId_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(PublishedFileId_t value)
		{
			return value.Value;
		}
	}
}