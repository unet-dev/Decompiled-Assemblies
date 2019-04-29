using System;

namespace Mono.Unix.Native
{
	[CLSCompliant(false)]
	[Flags]
	[Map]
	public enum MountFlags : ulong
	{
		ST_RDONLY = 1,
		ST_NOSUID = 2,
		ST_NODEV = 4,
		ST_NOEXEC = 8,
		ST_SYNCHRONOUS = 16,
		ST_REMOUNT = 32,
		ST_MANDLOCK = 64,
		ST_WRITE = 128,
		ST_APPEND = 256,
		ST_IMMUTABLE = 512,
		ST_NOATIME = 1024,
		ST_NODIRATIME = 2048,
		ST_BIND = 4096
	}
}