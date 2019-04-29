using System;
using System.Runtime.CompilerServices;

namespace Facepunch.Extend
{
	public static class ByteExtensions
	{
		public static float ReadFloat(this byte[] buffer, int iOffset = 0)
		{
			unsafe
			{
				fixed (byte* numPointer = &buffer[iOffset])
				{
					return (float)(*numPointer);
				}
			}
		}

		public static unsafe void WriteFloat(this byte[] buffer, float f, int iOffset = 0)
		{
			byte* numPointer = (byte*)(&f);
			buffer[iOffset] = *numPointer;
			buffer[iOffset + 1] = *(numPointer + 1);
			buffer[iOffset + 2] = *(numPointer + 2);
			buffer[iOffset + 3] = *(numPointer + 3);
		}
	}
}