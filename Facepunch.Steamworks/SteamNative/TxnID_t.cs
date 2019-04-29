using System;

namespace SteamNative
{
	internal struct TxnID_t
	{
		public GID_t Value;

		public static implicit operator TxnID_t(GID_t value)
		{
			return new TxnID_t()
			{
				Value = value
			};
		}

		public static implicit operator GID_t(TxnID_t value)
		{
			return value.Value;
		}
	}
}