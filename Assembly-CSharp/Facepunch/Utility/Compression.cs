using Ionic.Zlib;
using System;

namespace Facepunch.Utility
{
	public class Compression
	{
		public Compression()
		{
		}

		public static byte[] Compress(byte[] data)
		{
			byte[] numArray;
			try
			{
				numArray = GZipStream.CompressBuffer(data);
			}
			catch (Exception exception)
			{
				numArray = null;
			}
			return numArray;
		}

		public static byte[] Uncompress(byte[] data)
		{
			return GZipStream.UncompressBuffer(data);
		}
	}
}