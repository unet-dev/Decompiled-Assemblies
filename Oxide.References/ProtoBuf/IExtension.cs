using System;
using System.IO;

namespace ProtoBuf
{
	public interface IExtension
	{
		Stream BeginAppend();

		Stream BeginQuery();

		void EndAppend(Stream stream, bool commit);

		void EndQuery(Stream stream);

		int GetLength();
	}
}