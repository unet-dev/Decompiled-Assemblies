using System;

namespace Mono.Unix.Native
{
	[CLSCompliant(false)]
	[Flags]
	[Map]
	public enum MsyncFlags
	{
		MS_ASYNC = 1,
		MS_INVALIDATE = 2,
		MS_SYNC = 4
	}
}