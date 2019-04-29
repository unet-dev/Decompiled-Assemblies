using System;

namespace SteamNative
{
	internal struct JobID_t
	{
		public ulong Value;

		public static implicit operator JobID_t(ulong value)
		{
			return new JobID_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(JobID_t value)
		{
			return value.Value;
		}
	}
}