using System;
using System.IO;

namespace SilentOrbit.ProtocolBuffers
{
	public interface IProto
	{
		void ReadFromStream(Stream stream, int size, bool isDelta = false);

		void WriteToStream(Stream stream);
	}
}