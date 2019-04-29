using System;

namespace Mono.Posix
{
	[CLSCompliant(false)]
	[Flags]
	[Obsolete("Use Mono.Unix.Native.WaitOptions")]
	public enum WaitOptions
	{
		WNOHANG,
		WUNTRACED
	}
}