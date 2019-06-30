using System;

namespace Steamworks.Data
{
	internal enum NetConfigResult
	{
		BufferTooSmall = -3,
		BadScopeObj = -2,
		BadValue = -1,
		OK = 1,
		OKInherited = 2,
		Force32Bit = 2147483647
	}
}