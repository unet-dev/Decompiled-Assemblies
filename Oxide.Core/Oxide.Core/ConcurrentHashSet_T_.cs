using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Oxide.Core
{
	public class ConcurrentHashSet<T> : ICollection<T>, IEnumerable<T>, IEnumerable
	{
		private readonly HashSet<T> collection;

		private readonly object syncRoot;

		public int Count
		{
			get
			{
				int count;
				lock (this.syncRoot)
				{
					count = this.collection.Count;
				}
				return count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public ConcurrentHashSet()
		{
			this.collection = new HashSet<T>();
		}

		public ConcurrentHashSet(ICollection<T> values)
		{
			this.collection = new HashSet<T>(values);
		}

		public bool Add(T value)
		{
			bool flag;
			lock (this.syncRoot)
			{
				flag = this.collection.Add(value);
			}
			return flag;
		}

		public bool Any(Func<T, bool> callback)
		{
			bool flag;
			lock (this.syncRoot)
			{
				flag = this.collection.Any<T>(callback);
			}
			return flag;
		}

		public void Clear()
		{
			lock (this.syncRoot)
			{
				this.collection.Clear();
			}
		}

		public bool Contains(T value)
		{
			bool flag;
			lock (this.syncRoot)
			{
				flag = this.collection.Contains(value);
			}
			return flag;
		}

		public void CopyTo(T[] array, int index)
		{
			lock (this.syncRoot)
			{
				this.collection.CopyTo(array, index);
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this.collection.GetEnumerator();
		}

		public bool Remove(T value)
		{
			bool flag;
			lock (this.syncRoot)
			{
				flag = this.collection.Remove(value);
			}
			return flag;
		}

		void System.Collections.Generic.ICollection<T>.Add(T value)
		{
			this.Add(value);
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public T[] ToArray()
		{
			T[] array;
			lock (this.syncRoot)
			{
				array = this.collection.ToArray<T>();
			}
			return array;
		}

		public bool TryDequeue(out T value)
		{
			bool flag;
			lock (this.syncRoot)
			{
				value = this.collection.ElementAtOrDefault<T>(0);
				if (value != null)
				{
					this.collection.Remove(value);
				}
				flag = value != null;
			}
			return flag;
		}
	}
}