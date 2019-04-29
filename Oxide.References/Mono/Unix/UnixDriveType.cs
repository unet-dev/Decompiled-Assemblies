using System;

namespace Mono.Unix
{
	public enum UnixDriveType
	{
		Unknown,
		NoRootDirectory,
		Removable,
		Fixed,
		Network,
		CDRom,
		Ram
	}
}