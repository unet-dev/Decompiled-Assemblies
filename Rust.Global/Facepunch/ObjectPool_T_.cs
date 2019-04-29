using System;
using System.Collections.Generic;

namespace Facepunch
{
	public class ObjectPool<T>
	{
		public List<T> list;

		public ObjectPool()
		{
		}

		public virtual void AddToPool(T t)
		{
			this.list.Add(t);
		}

		public T TakeFromPool()
		{
			if (this.list.Count == 0)
			{
				return default(T);
			}
			T item = this.list[0];
			this.list.RemoveAt(0);
			return item;
		}
	}
}