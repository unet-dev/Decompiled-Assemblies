using System;

namespace Mono.Unix.Native
{
	[CLSCompliant(false)]
	[Map]
	public enum SeekFlags : short
	{
		L_SET = 0,
		SEEK_SET = 0,
		L_INCR = 1,
		SEEK_CUR = 1,
		L_XTND = 2,
		SEEK_END = 2
	}
}