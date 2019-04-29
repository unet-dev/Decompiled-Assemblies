using System;
using System.Collections.Generic;

namespace Oxide.Core
{
	public static class ArrayPool
	{
		private const int MaxArrayLength = 50;

		private const int InitialPoolAmount = 64;

		private const int MaxPoolAmount = 256;

		private static List<Queue<object[]>> _pooledArrays;

		static ArrayPool()
		{
			ArrayPool._pooledArrays = new List<Queue<object[]>>();
			for (int i = 0; i < 50; i++)
			{
				ArrayPool._pooledArrays.Add(new Queue<object[]>());
				ArrayPool.SetupArrays(i + 1);
			}
		}

		public static void Free(object[] array)
		{
			if (array == null || array.Length == 0 || (int)array.Length > 50)
			{
				return;
			}
			for (int i = 0; i < (int)array.Length; i++)
			{
				array[i] = null;
			}
			Queue<object[]> item = ArrayPool._pooledArrays[(int)array.Length - 1];
			lock (item)
			{
				if (item.Count <= 256)
				{
					item.Enqueue(array);
				}
				else
				{
					for (int j = 0; j < 64; j++)
					{
						item.Dequeue();
					}
				}
			}
		}

		public static object[] Get(int length)
		{
			object[] objArray;
			if (length == 0 || length > 50)
			{
				return new object[length];
			}
			Queue<object[]> item = ArrayPool._pooledArrays[length - 1];
			lock (item)
			{
				if (item.Count == 0)
				{
					ArrayPool.SetupArrays(length);
				}
				objArray = item.Dequeue();
			}
			return objArray;
		}

		private static void SetupArrays(int length)
		{
			Queue<object[]> item = ArrayPool._pooledArrays[length - 1];
			for (int i = 0; i < 64; i++)
			{
				item.Enqueue(new object[length]);
			}
		}
	}
}