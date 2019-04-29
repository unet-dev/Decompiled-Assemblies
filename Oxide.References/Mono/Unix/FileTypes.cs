using System;

namespace Mono.Unix
{
	public enum FileTypes
	{
		Fifo = 4096,
		CharacterDevice = 8192,
		Directory = 16384,
		BlockDevice = 24576,
		RegularFile = 32768,
		SymbolicLink = 40960,
		Socket = 49152
	}
}