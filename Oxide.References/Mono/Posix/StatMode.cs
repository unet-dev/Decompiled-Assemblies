using System;

namespace Mono.Posix
{
	[Flags]
	[Obsolete("Use Mono.Unix.Native.FilePermissions")]
	public enum StatMode
	{
		[Obsolete("Use Mono.Unix.Native.FilePermissions.S_IXOTH")]
		OthersExecute = 1,
		[Obsolete("Use Mono.Unix.Native.FilePermissions.S_IWOTH")]
		OthersWrite = 2,
		[Obsolete("Use Mono.Unix.Native.FilePermissions.S_IROTH")]
		OthersRead = 4,
		[Obsolete("Use Mono.Unix.Native.FilePermissions.S_IXGRP")]
		GroupExecute = 8,
		[Obsolete("Use Mono.Unix.Native.FilePermissions.S_IWGRP")]
		GroupWrite = 16,
		[Obsolete("Use Mono.Unix.Native.FilePermissions.S_IRGRP")]
		GroupRead = 32,
		[Obsolete("Use Mono.Unix.Native.FilePermissions.S_IXUSR")]
		OwnerExecute = 64,
		[Obsolete("Use Mono.Unix.Native.FilePermissions.S_IWUSR")]
		OwnerWrite = 128,
		[Obsolete("Use Mono.Unix.Native.FilePermissions.S_IRUSR")]
		OwnerRead = 256,
		[Obsolete("Use Mono.Unix.Native.FilePermissions.S_ISVTX")]
		Sticky = 512,
		[Obsolete("Use Mono.Unix.Native.FilePermissions.S_ISGID")]
		SGid = 1024,
		[Obsolete("Use Mono.Unix.Native.FilePermissions.S_ISUID")]
		SUid = 2048,
		[Obsolete("Use Mono.Unix.Native.FilePermissions.S_IFIFO")]
		FIFO = 4096,
		[Obsolete("Use Mono.Unix.Native.FilePermissions.S_IFCHR")]
		CharDevice = 8192,
		[Obsolete("Use Mono.Unix.Native.FilePermissions.S_IFDIR")]
		Directory = 16384,
		[Obsolete("Use Mono.Unix.Native.FilePermissions.S_IFBLK")]
		BlockDevice = 24576,
		[Obsolete("Use Mono.Unix.Native.FilePermissions.S_IFREG")]
		Regular = 32768,
		[Obsolete("Use Mono.Unix.Native.FilePermissions.S_IFLNK")]
		SymLink = 40960,
		[Obsolete("Use Mono.Unix.Native.FilePermissions.S_IFSOCK")]
		Socket = 49152
	}
}