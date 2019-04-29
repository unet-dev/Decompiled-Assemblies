using System;

namespace Mono.Cecil
{
	[Flags]
	public enum FieldAttributes : ushort
	{
		CompilerControlled = 0,
		Private = 1,
		FamANDAssem = 2,
		Assembly = 3,
		Family = 4,
		FamORAssem = 5,
		Public = 6,
		FieldAccessMask = 7,
		Static = 16,
		InitOnly = 32,
		Literal = 64,
		NotSerialized = 128,
		HasFieldRVA = 256,
		SpecialName = 512,
		RTSpecialName = 1024,
		HasFieldMarshal = 4096,
		PInvokeImpl = 8192,
		HasDefault = 32768
	}
}