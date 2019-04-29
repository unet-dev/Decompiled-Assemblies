using System;

namespace Mono.Unix.Native
{
	[CLSCompliant(false)]
	[Flags]
	[Map]
	public enum OpenFlags
	{
		O_RDONLY = 0,
		O_WRONLY = 1,
		O_RDWR = 2,
		O_CREAT = 64,
		O_EXCL = 128,
		O_NOCTTY = 256,
		O_TRUNC = 512,
		O_APPEND = 1024,
		O_NONBLOCK = 2048,
		O_SYNC = 4096,
		O_ASYNC = 8192,
		O_DIRECT = 16384,
		O_LARGEFILE = 32768,
		O_DIRECTORY = 65536,
		O_NOFOLLOW = 131072
	}
}