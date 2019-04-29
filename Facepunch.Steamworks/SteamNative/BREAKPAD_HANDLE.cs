using System;

namespace SteamNative
{
	internal struct BREAKPAD_HANDLE
	{
		public IntPtr Value;

		public static implicit operator BREAKPAD_HANDLE(IntPtr value)
		{
			return new BREAKPAD_HANDLE()
			{
				Value = value
			};
		}

		public static implicit operator IntPtr(BREAKPAD_HANDLE value)
		{
			return value.Value;
		}
	}
}