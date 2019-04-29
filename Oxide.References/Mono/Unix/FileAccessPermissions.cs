using System;

namespace Mono.Unix
{
	[Flags]
	public enum FileAccessPermissions
	{
		OtherExecute = 1,
		OtherWrite = 2,
		OtherRead = 4,
		OtherReadWriteExecute = 7,
		GroupExecute = 8,
		GroupWrite = 16,
		GroupRead = 32,
		GroupReadWriteExecute = 56,
		UserExecute = 64,
		UserWrite = 128,
		UserRead = 256,
		DefaultPermissions = 438,
		UserReadWriteExecute = 448,
		AllPermissions = 511
	}
}