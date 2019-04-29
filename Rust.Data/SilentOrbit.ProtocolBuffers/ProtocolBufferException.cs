using System;

namespace SilentOrbit.ProtocolBuffers
{
	public class ProtocolBufferException : Exception
	{
		public ProtocolBufferException(string message) : base(message)
		{
		}
	}
}