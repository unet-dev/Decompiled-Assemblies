using System;

namespace SteamNative
{
	internal struct HServerListRequest
	{
		public IntPtr Value;

		public static implicit operator HServerListRequest(IntPtr value)
		{
			return new HServerListRequest()
			{
				Value = value
			};
		}

		public static implicit operator IntPtr(HServerListRequest value)
		{
			return value.Value;
		}
	}
}