using System;

namespace Mono.Cecil
{
	[Flags]
	public enum AssemblyAttributes : uint
	{
		SideBySideCompatible = 0,
		PublicKey = 1,
		Retargetable = 256,
		WindowsRuntime = 512,
		DisableJITCompileOptimizer = 16384,
		EnableJITCompileTracking = 32768
	}
}