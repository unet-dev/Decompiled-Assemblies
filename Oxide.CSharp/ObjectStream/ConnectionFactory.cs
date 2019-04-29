using System;
using System.IO;

namespace ObjectStream
{
	internal static class ConnectionFactory
	{
		public static ObjectStreamConnection<TRead, TWrite> CreateConnection<TRead, TWrite>(Stream inStream, Stream outStream)
		where TRead : class
		where TWrite : class
		{
			return new ObjectStreamConnection<TRead, TWrite>(inStream, outStream);
		}
	}
}