using System;

namespace Mono.Unix.Native
{
	[CLSCompliant(false)]
	[Map]
	public enum LockType : short
	{
		F_RDLCK,
		F_WRLCK,
		F_UNLCK
	}
}