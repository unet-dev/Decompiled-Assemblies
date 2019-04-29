using System;

namespace Mono.Unix.Native
{
	[CLSCompliant(false)]
	[Flags]
	[Map]
	public enum XattrFlags
	{
		XATTR_AUTO,
		XATTR_CREATE,
		XATTR_REPLACE
	}
}