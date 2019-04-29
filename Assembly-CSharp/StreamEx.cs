using System;
using System.IO;
using System.Runtime.CompilerServices;

public static class StreamEx
{
	private readonly static byte[] StaticBuffer;

	static StreamEx()
	{
		StreamEx.StaticBuffer = new byte[16384];
	}

	public static void WriteToOtherStream(this Stream self, Stream target)
	{
		while (true)
		{
			int num = self.Read(StreamEx.StaticBuffer, 0, (int)StreamEx.StaticBuffer.Length);
			int num1 = num;
			if (num <= 0)
			{
				break;
			}
			target.Write(StreamEx.StaticBuffer, 0, num1);
		}
	}
}