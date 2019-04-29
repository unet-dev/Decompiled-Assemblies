using System;

namespace Mono.Unix.Native
{
	[CLSCompliant(false)]
	[Flags]
	[Map]
	public enum DirectoryNotifyFlags
	{
		DN_MULTISHOT = -2147483648,
		DN_ACCESS = 1,
		DN_MODIFY = 2,
		DN_CREATE = 4,
		DN_DELETE = 8,
		DN_RENAME = 16,
		DN_ATTRIB = 32
	}
}