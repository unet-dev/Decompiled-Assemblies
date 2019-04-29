using System;

namespace Mono.Cecil
{
	[Flags]
	public enum MethodImplAttributes : ushort
	{
		IL = 0,
		Managed = 0,
		Native = 1,
		OPTIL = 2,
		CodeTypeMask = 3,
		Runtime = 3,
		ManagedMask = 4,
		Unmanaged = 4,
		NoInlining = 8,
		ForwardRef = 16,
		Synchronized = 32,
		NoOptimization = 64,
		PreserveSig = 128,
		InternalCall = 4096
	}
}