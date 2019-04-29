using System;

namespace Mono.Cecil
{
	[Flags]
	public enum PInvokeAttributes : ushort
	{
		CharSetNotSpec = 0,
		NoMangle = 1,
		CharSetAnsi = 2,
		CharSetUnicode = 4,
		CharSetAuto = 6,
		CharSetMask = 6,
		BestFitEnabled = 16,
		BestFitDisabled = 32,
		BestFitMask = 48,
		SupportsLastError = 64,
		CallConvWinapi = 256,
		CallConvCdecl = 512,
		CallConvStdCall = 768,
		CallConvThiscall = 1024,
		CallConvFastcall = 1280,
		CallConvMask = 1792,
		ThrowOnUnmappableCharEnabled = 4096,
		ThrowOnUnmappableCharDisabled = 8192,
		ThrowOnUnmappableCharMask = 12288
	}
}