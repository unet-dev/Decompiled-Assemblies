using System;

namespace Mono.Unix.Native
{
	[CLSCompliant(false)]
	[Flags]
	[Map]
	public enum AccessModes
	{
		R_OK = 1,
		W_OK = 2,
		X_OK = 4,
		F_OK = 8
	}
}