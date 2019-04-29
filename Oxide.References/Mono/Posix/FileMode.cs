using System;

namespace Mono.Posix
{
	[CLSCompliant(false)]
	[Flags]
	[Obsolete("Use Mono.Unix.Native.FilePermissions")]
	public enum FileMode
	{
		S_IXOTH = 1,
		S_IWOTH = 2,
		S_IROTH = 4,
		S_IXGRP = 8,
		S_IWGRP = 16,
		S_IRGRP = 32,
		S_IXUSR = 64,
		S_IWUSR = 128,
		S_IRUSR = 256,
		S_ISVTX = 512,
		S_ISGID = 1024,
		S_ISUID = 2048
	}
}