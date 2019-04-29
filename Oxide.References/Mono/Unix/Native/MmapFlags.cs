using System;

namespace Mono.Unix.Native
{
	[CLSCompliant(false)]
	[Flags]
	[Map]
	public enum MmapFlags
	{
		MAP_FILE = 0,
		MAP_SHARED = 1,
		MAP_PRIVATE = 2,
		MAP_TYPE = 15,
		MAP_FIXED = 16,
		MAP_ANON = 32,
		MAP_ANONYMOUS = 32,
		MAP_GROWSDOWN = 256,
		MAP_DENYWRITE = 2048,
		MAP_EXECUTABLE = 4096,
		MAP_LOCKED = 8192,
		MAP_NORESERVE = 16384,
		MAP_POPULATE = 32768,
		MAP_NONBLOCK = 65536
	}
}