using System;

namespace ProtoBuf
{
	[Flags]
	public enum MemberSerializationOptions
	{
		None = 0,
		Packed = 1,
		Required = 2,
		AsReference = 4,
		DynamicType = 8,
		OverwriteList = 16,
		AsReferenceHasValue = 32
	}
}