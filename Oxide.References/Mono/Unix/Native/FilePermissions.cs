using System;

namespace Mono.Unix.Native
{
	[CLSCompliant(false)]
	[Flags]
	[Map]
	public enum FilePermissions : uint
	{
		S_IXOTH = 1,
		S_IWOTH = 2,
		S_IROTH = 4,
		S_IRWXO = 7,
		S_IXGRP = 8,
		S_IWGRP = 16,
		S_IRGRP = 32,
		S_IRWXG = 56,
		S_IXUSR = 64,
		S_IWUSR = 128,
		S_IRUSR = 256,
		DEFFILEMODE = 438,
		S_IRWXU = 448,
		ACCESSPERMS = 511,
		S_ISVTX = 512,
		S_ISGID = 1024,
		S_ISUID = 2048,
		ALLPERMS = 4095,
		[Map(SuppressFlags="S_IFMT")]
		S_IFIFO = 4096,
		[Map(SuppressFlags="S_IFMT")]
		S_IFCHR = 8192,
		[Map(SuppressFlags="S_IFMT")]
		S_IFDIR = 16384,
		[Map(SuppressFlags="S_IFMT")]
		S_IFBLK = 24576,
		[Map(SuppressFlags="S_IFMT")]
		S_IFREG = 32768,
		[Map(SuppressFlags="S_IFMT")]
		S_IFLNK = 40960,
		[Map(SuppressFlags="S_IFMT")]
		S_IFSOCK = 49152,
		S_IFMT = 61440
	}
}