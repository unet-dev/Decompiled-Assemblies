using System;
using System.IO;

namespace ObjectStream.IO
{
	public class ObjectStreamWrapper<TReadWrite> : ObjectStreamWrapper<TReadWrite, TReadWrite>
	where TReadWrite : class
	{
		public ObjectStreamWrapper(Stream inStream, Stream outStream) : base(inStream, outStream)
		{
		}
	}
}