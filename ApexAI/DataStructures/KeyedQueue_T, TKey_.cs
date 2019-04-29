using Apex.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Apex.DataStructures
{
	public class KeyedQueue<T, TKey> : IIterable<T>, IEnumerable<T>, IEnumerable, IIndexable<T>, IQueue<T>
	{
		private HashSet<TKey> _hashset;

		private SimpleQueue<T> _queue;

		private Func<T, TKey> _keyProvider;

		private bool _strictSet;

		public int count
		{
			get
			{
				return this._queue.count;
			}
		}

		public T this[int idx]
		{
			get
			{
				return this._queue[idx];
			}
		}

		public KeyedQueue(Func<T, TKey> keyProvider, bool strictSet) : this(0, keyProvider, strictSet)
		{
		}

		public KeyedQueue(int capacity, Func<T, TKey> keyProvider, bool strictSet)
		{
			Ensure.ArgumentInRange(() => capacity >= 0, "capacity", capacity, null);
			this._hashset = new HashSet<TKey>();
			this._queue = new SimpleQueue<T>(capacity);
			this._keyProvider = keyProvider;
			this._strictSet = strictSet;
		}

		public void Clear()
		{
			this._queue.Clear();
			this._hashset.Clear();
		}

		public bool Contains(T obj)
		{
			return this._hashset.Contains(this._keyProvider(obj));
		}

		public T Dequeue()
		{
			T t = this._queue.Dequeue();
			if (!this._strictSet)
			{
				this._hashset.Remove(this._keyProvider(t));
			}
			return t;
		}

		public void Enqueue(T obj)
		{
			if (this._hashset.Add(this._keyProvider(obj)))
			{
				this._queue.Enqueue(obj);
			}
		}

		IEnumerator<T> System.Collections.Generic.IEnumerable<T>.GetEnumerator()
		{
			KeyedQueue<T, TKey> ts = null;
			int num = ts.count;
			for (int i = 0; i < num; i++)
			{
				yield return ts._queue[i];
			}
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			KeyedQueue<T, TKey> ts = null;
			int num = ts.count;
			for (int i = 0; i < num; i++)
			{
				yield return ts._queue[i];
			}
		}
	}
}