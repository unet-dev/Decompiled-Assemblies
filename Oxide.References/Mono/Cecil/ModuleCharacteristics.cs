using System;

namespace Mono.Cecil
{
	[Flags]
	public enum ModuleCharacteristics
	{
		HighEntropyVA = 32,
		DynamicBase = 64,
		NXCompat = 256,
		NoSEH = 1024,
		AppContainer = 4096,
		TerminalServerAware = 32768
	}
}