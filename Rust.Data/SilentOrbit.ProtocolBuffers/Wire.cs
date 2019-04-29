using System;

namespace SilentOrbit.ProtocolBuffers
{
	public enum Wire
	{
		Varint = 0,
		Fixed64 = 1,
		LengthDelimited = 2,
		Fixed32 = 5
	}
}