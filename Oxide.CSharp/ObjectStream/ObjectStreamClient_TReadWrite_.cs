using System;
using System.IO;

namespace ObjectStream
{
	public class ObjectStreamClient<TReadWrite> : ObjectStreamClient<TReadWrite, TReadWrite>
	where TReadWrite : class
	{
		public ObjectStreamClient(Stream inStream, Stream outStream) : base(inStream, outStream)
		{
		}
	}
}