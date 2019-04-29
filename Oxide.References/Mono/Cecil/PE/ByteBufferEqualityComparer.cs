using System;
using System.Collections.Generic;

namespace Mono.Cecil.PE
{
	internal sealed class ByteBufferEqualityComparer : IEqualityComparer<ByteBuffer>
	{
		public ByteBufferEqualityComparer()
		{
		}

		public bool Equals(ByteBuffer x, ByteBuffer y)
		{
			if (x.length != y.length)
			{
				return false;
			}
			byte[] numArray = x.buffer;
			byte[] numArray1 = y.buffer;
			for (int i = 0; i < x.length; i++)
			{
				if (numArray[i] != numArray1[i])
				{
					return false;
				}
			}
			return true;
		}

		public int GetHashCode(ByteBuffer buffer)
		{
			int num = 0;
			byte[] numArray = buffer.buffer;
			for (int i = 0; i < buffer.length; i++)
			{
				num = num * 37 ^ numArray[i];
			}
			return num;
		}
	}
}