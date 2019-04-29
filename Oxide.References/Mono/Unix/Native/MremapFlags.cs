using System;

namespace Mono.Unix.Native
{
	[CLSCompliant(false)]
	[Flags]
	[Map]
	public enum MremapFlags : ulong
	{
		MREMAP_MAYMOVE = 1
	}
}