using System;

namespace Mono.Posix
{
	[CLSCompliant(false)]
	[Flags]
	[Obsolete("Use Mono.Unix.Native.OpenFlags")]
	public enum OpenFlags
	{
		O_RDONLY = 0,
		O_WRONLY = 1,
		O_RDWR = 2,
		O_CREAT = 4,
		O_EXCL = 8,
		O_NOCTTY = 16,
		O_TRUNC = 32,
		O_APPEND = 64,
		O_NONBLOCK = 128,
		O_SYNC = 256
	}
}