using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Apex.DataStructures
{
	public class SimpleQueue<T> : IIterable<T>, IEnumerable<T>, IEnumerable, IIndexable<T>
	{
		private T[] _array;

		private int _used;

		private int _head;

		public int count
		{
			get
			{
				return this._used;
			}
		}

		public T this[int idx]
		{
			get
			{
				if (idx < 0 || idx >= this._used)
				{
					throw new ArgumentOutOfRangeException("idx", "The queue does not contain an item at that index.");
				}
				idx = (this._head + idx) % (int)this._array.Length;
				return this._array[idx];
			}
		}

		public SimpleQueue() : this(4)
		{
		}

		public SimpleQueue(int capacity)
		{
			capacity = Math.Max(capacity, 4);
			this._array = new T[capacity];
			this._used = 0;
		}

		public void Clear()
		{
			Array.Clear(this._array, 0, this._used);
			this._used = 0;
			this._head = 0;
		}

		public T Dequeue()
		{
			if (this._used == 0)
			{
				throw new InvalidOperationException("The queue is empty.");
			}
			T t = this._array[this._head];
			this._array[this._head] = default(T);
			this._used--;
			this._head = (this._head + 1) % (int)this._array.Length;
			return t;
		}

		public void Enqueue(T item)
		{
			if (this._used == (int)this._array.Length)
			{
				T[] tArray = new T[2 * this._used];
				if (this._head != 0)
				{
					Array.Copy(this._array, this._head, tArray, 0, this._used - this._head);
					Array.Copy(this._array, 0, tArray, this._used - this._head, this._head);
				}
				else
				{
					Array.Copy(this._array, 0, tArray, 0, this._used);
				}
				this._array = tArray;
				this._head = 0;
			}
			int length = (this._head + this._used) % (int)this._array.Length;
			this._used++;
			this._array[length] = item;
		}

		public T Last()
		{
			if (this._used == 0)
			{
				throw new InvalidOperationException("The queue is empty.");
			}
			int length = (this._head + this._used - 1) % (int)this._array.Length;
			return this._array[length];
		}

		public T Peek()
		{
			if (this._used == 0)
			{
				throw new InvalidOperationException("The queue is empty.");
			}
			return this._array[this._head];
		}

		IEnumerator<T> System.Collections.Generic.IEnumerable<T>.GetEnumerator()
		{
			SimpleQueue<T> ts = null;
			for (int i = 0; i < ts._used; i++)
			{
				int length = (ts._head + i) % (int)ts._array.Length;
				yield return ts._array[length];
			}
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			SimpleQueue<T> ts = null;
			for (int i = 0; i < ts._used; i++)
			{
				int length = (ts._head + i) % (int)ts._array.Length;
				yield return ts._array[length];
			}
		}

		public T[] ToArray()
		{
			T[] tArray = new T[this._used];
			if (this._head + this._used > (int)this._array.Length)
			{
				int length = (int)this._array.Length - this._head;
				Array.Copy(this._array, this._head, tArray, 0, length);
				Array.Copy(this._array, 0, tArray, length, this._used - length);
			}
			else
			{
				Array.Copy(this._array, this._head, tArray, 0, this._used);
			}
			return tArray;
		}
	}
}