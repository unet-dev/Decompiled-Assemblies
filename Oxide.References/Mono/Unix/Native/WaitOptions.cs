using System;

namespace Mono.Unix.Native
{
	[Flags]
	[Map]
	public enum WaitOptions
	{
		WNOHANG = 1,
		WUNTRACED = 2
	}
}