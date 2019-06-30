using System;

namespace Steamworks.Data
{
	internal enum DebugOutputType
	{
		None = 0,
		Bug = 1,
		Error = 2,
		Important = 3,
		Warning = 4,
		Msg = 5,
		Verbose = 6,
		Debug = 7,
		Everything = 8,
		Force32Bit = 2147483647
	}
}