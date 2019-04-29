using System;

namespace ProtoBuf
{
	internal sealed class BufferPool
	{
		private const int PoolSize = 20;

		internal const int BufferLength = 1024;

		private readonly static object[] pool;

		static BufferPool()
		{
			BufferPool.pool = new object[20];
		}

		private BufferPool()
		{
		}

		internal static void Flush()
		{
			lock (BufferPool.pool)
			{
				for (int i = 0; i < (int)BufferPool.pool.Length; i++)
				{
					BufferPool.pool[i] = null;
				}
			}
		}

		internal static byte[] GetBuffer()
		{
			byte[] numArray;
			lock (BufferPool.pool)
			{
				int num = 0;
				while (num < (int)BufferPool.pool.Length)
				{
					object obj = BufferPool.pool[num];
					object obj1 = obj;
					if (obj == null)
					{
						num++;
					}
					else
					{
						BufferPool.pool[num] = null;
						numArray = (byte[])obj1;
						return numArray;
					}
				}
				return new byte[1024];
			}
			return numArray;
		}

		internal static void ReleaseBufferToPool(ref byte[] buffer)
		{
			if (buffer == null)
			{
				return;
			}
			if ((int)buffer.Length == 1024)
			{
				lock (BufferPool.pool)
				{
					int num = 0;
					while (num < (int)BufferPool.pool.Length)
					{
						if (BufferPool.pool[num] != null)
						{
							num++;
						}
						else
						{
							BufferPool.pool[num] = buffer;
							break;
						}
					}
				}
			}
			buffer = null;
		}

		internal static void ResizeAndFlushLeft(ref byte[] buffer, int toFitAtLeastBytes, int copyFromIndex, int copyBytes)
		{
			int length = (int)buffer.Length * 2;
			if (length < toFitAtLeastBytes)
			{
				length = toFitAtLeastBytes;
			}
			byte[] numArray = new byte[length];
			if (copyBytes > 0)
			{
				Helpers.BlockCopy(buffer, copyFromIndex, numArray, 0, copyBytes);
			}
			if ((int)buffer.Length == 1024)
			{
				BufferPool.ReleaseBufferToPool(ref buffer);
			}
			buffer = numArray;
		}
	}
}