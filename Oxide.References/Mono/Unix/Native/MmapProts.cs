using System;

namespace Mono.Unix.Native
{
	[CLSCompliant(false)]
	[Flags]
	[Map]
	public enum MmapProts
	{
		PROT_NONE = 0,
		PROT_READ = 1,
		PROT_WRITE = 2,
		PROT_EXEC = 4,
		PROT_GROWSDOWN = 16777216,
		PROT_GROWSUP = 33554432
	}
}