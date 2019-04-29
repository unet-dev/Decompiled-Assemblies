using System;
using System.Collections.Generic;

namespace Apex.DataStructures
{
	public class QueueOnce<T>
	{
		private HashSet<T> _set;

		private Queue<T> _q;

		public int count
		{
			get
			{
				return this._q.Count;
			}
		}

		public QueueOnce()
		{
		}

		public void Clear()
		{
			this._q.Clear();
		}

		public T Dequeue()
		{
			return this._q.Dequeue();
		}

		public bool Enqueue(T item)
		{
			if (!this._set.Add(item))
			{
				return false;
			}
			this._q.Enqueue(item);
			return true;
		}

		public void Enqueue(IEnumerable<T> items)
		{
			foreach (T item in items)
			{
				if (!this._set.Add(item))
				{
					continue;
				}
				this._q.Enqueue(item);
			}
		}

		public bool HasQueued(T item)
		{
			return this._set.Contains(item);
		}
	}
}