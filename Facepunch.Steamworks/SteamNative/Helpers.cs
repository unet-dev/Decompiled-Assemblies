using System;
using System.Text;

namespace SteamNative
{
	internal static class Helpers
	{
		private static StringBuilder[] StringBuilderPool;

		private static int StringBuilderPoolIndex;

		public static StringBuilder TakeStringBuilder()
		{
			if (Helpers.StringBuilderPool == null)
			{
				Helpers.StringBuilderPool = new StringBuilder[8];
				for (int i = 0; i < (int)Helpers.StringBuilderPool.Length; i++)
				{
					Helpers.StringBuilderPool[i] = new StringBuilder(4096);
				}
			}
			Helpers.StringBuilderPoolIndex++;
			if (Helpers.StringBuilderPoolIndex >= (int)Helpers.StringBuilderPool.Length)
			{
				Helpers.StringBuilderPoolIndex = 0;
			}
			Helpers.StringBuilderPool[Helpers.StringBuilderPoolIndex].Capacity = 4096;
			Helpers.StringBuilderPool[Helpers.StringBuilderPoolIndex].Length = 0;
			return Helpers.StringBuilderPool[Helpers.StringBuilderPoolIndex];
		}
	}
}