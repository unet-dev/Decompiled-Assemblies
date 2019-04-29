using ObjectStream.IO;
using System;
using System.IO;

namespace ObjectStream
{
	internal static class ObjectStreamClientFactory
	{
		public static ObjectStreamWrapper<TRead, TWrite> Connect<TRead, TWrite>(Stream inStream, Stream outStream)
		where TRead : class
		where TWrite : class
		{
			return new ObjectStreamWrapper<TRead, TWrite>(inStream, outStream);
		}
	}
}