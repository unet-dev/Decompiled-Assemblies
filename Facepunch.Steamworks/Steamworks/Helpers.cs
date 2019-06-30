using System;
using System.Text;

namespace Steamworks
{
	internal static class Helpers
	{
		private static StringBuilder[] StringBuilderPool;

		private static int StringBuilderPoolIndex;

		private static byte[][] BufferPool;

		private static int BufferPoolIndex;

		public static byte[] TakeBuffer(int minSize)
		{
			if (Helpers.BufferPool == null)
			{
				Helpers.BufferPool = new byte[8][];
				for (int i = 0; i < (int)Helpers.BufferPool.Length; i++)
				{
					Helpers.BufferPool[i] = new Byte[131072];
				}
			}
			Helpers.BufferPoolIndex++;
			if (Helpers.BufferPoolIndex >= (int)Helpers.BufferPool.Length)
			{
				Helpers.BufferPoolIndex = 0;
			}
			if ((int)Helpers.BufferPool[Helpers.BufferPoolIndex].Length < minSize)
			{
				Helpers.BufferPool[Helpers.BufferPoolIndex] = new Byte[minSize + 1024];
			}
			return Helpers.BufferPool[Helpers.BufferPoolIndex];
		}

		public static StringBuilder TakeStringBuilder()
		{
			if (Helpers.StringBuilderPool == null)
			{
				Helpers.StringBuilderPool = new StringBuilder[4];
				for (int i = 0; i < (int)Helpers.StringBuilderPool.Length; i++)
				{
					Helpers.StringBuilderPool[i] = new StringBuilder(32768);
				}
			}
			Helpers.StringBuilderPoolIndex++;
			if (Helpers.StringBuilderPoolIndex >= (int)Helpers.StringBuilderPool.Length)
			{
				Helpers.StringBuilderPoolIndex = 0;
			}
			Helpers.StringBuilderPool[Helpers.StringBuilderPoolIndex].Length = 0;
			return Helpers.StringBuilderPool[Helpers.StringBuilderPoolIndex];
		}
	}
}