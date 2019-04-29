using System;

namespace SteamNative
{
	internal struct UGCFileWriteStreamHandle_t
	{
		public ulong Value;

		public static implicit operator UGCFileWriteStreamHandle_t(ulong value)
		{
			return new UGCFileWriteStreamHandle_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(UGCFileWriteStreamHandle_t value)
		{
			return value.Value;
		}
	}
}