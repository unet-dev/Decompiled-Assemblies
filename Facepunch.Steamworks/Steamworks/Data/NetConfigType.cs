using System;

namespace Steamworks.Data
{
	internal enum NetConfigType
	{
		Int32 = 1,
		Int64 = 2,
		Float = 3,
		String = 4,
		FunctionPtr = 5,
		Force32Bit = 2147483647
	}
}