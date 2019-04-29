using System;

namespace Mono.Cecil
{
	public enum NativeType
	{
		Boolean = 2,
		I1 = 3,
		U1 = 4,
		I2 = 5,
		U2 = 6,
		I4 = 7,
		U4 = 8,
		I8 = 9,
		U8 = 10,
		R4 = 11,
		R8 = 12,
		Currency = 15,
		BStr = 19,
		LPStr = 20,
		LPWStr = 21,
		LPTStr = 22,
		FixedSysString = 23,
		IUnknown = 25,
		IDispatch = 26,
		Struct = 27,
		IntF = 28,
		SafeArray = 29,
		FixedArray = 30,
		Int = 31,
		UInt = 32,
		ByValStr = 34,
		ANSIBStr = 35,
		TBStr = 36,
		VariantBool = 37,
		Func = 38,
		ASAny = 40,
		Array = 42,
		LPStruct = 43,
		CustomMarshaler = 44,
		Error = 45,
		Max = 80,
		None = 102
	}
}