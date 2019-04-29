using System;

namespace SteamNative
{
	internal struct ControllerHandle_t
	{
		public ulong Value;

		public static implicit operator ControllerHandle_t(ulong value)
		{
			return new ControllerHandle_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(ControllerHandle_t value)
		{
			return value.Value;
		}
	}
}