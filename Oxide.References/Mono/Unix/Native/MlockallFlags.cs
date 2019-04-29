using System;

namespace Mono.Unix.Native
{
	[CLSCompliant(false)]
	[Flags]
	[Map]
	public enum MlockallFlags
	{
		MCL_CURRENT = 1,
		MCL_FUTURE = 2
	}
}