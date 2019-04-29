using Apex;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Apex.Utilities
{
	public static class ListBufferPool
	{
		private readonly static Dictionary<Type, Queue<IList>> _pool;

		static ListBufferPool()
		{
			ListBufferPool._pool = new Dictionary<Type, Queue<IList>>();
		}

		public static List<T> GetBuffer<T>(int capacityHint)
		{
			Queue<IList> lists;
			List<T> ts;
			lock (ListBufferPool._pool)
			{
				if (!ListBufferPool._pool.TryGetValue(typeof(T), out lists) || lists.Count == 0)
				{
					ts = new List<T>(capacityHint);
				}
				else
				{
					List<T> ts1 = (List<T>)lists.Dequeue();
					ts1.EnsureCapacity<T>(capacityHint);
					ts = ts1;
				}
			}
			return ts;
		}

		public static void PreAllocate<T>(int capacity, int number = 1)
		{
			Queue<IList> lists;
			lock (ListBufferPool._pool)
			{
				if (!ListBufferPool._pool.TryGetValue(typeof(T), out lists))
				{
					lists = new Queue<IList>(number);
					ListBufferPool._pool[typeof(T)] = lists;
				}
				for (int i = 0; i < number; i++)
				{
					lists.Enqueue(new List<T>(capacity));
				}
			}
		}

		public static void ReturnBuffer<T>(List<T> buffer)
		{
			Queue<IList> lists;
			buffer.Clear();
			lock (ListBufferPool._pool)
			{
				if (!ListBufferPool._pool.TryGetValue(typeof(T), out lists))
				{
					lists = new Queue<IList>(1);
					ListBufferPool._pool[typeof(T)] = lists;
				}
				lists.Enqueue(buffer);
			}
		}
	}
}