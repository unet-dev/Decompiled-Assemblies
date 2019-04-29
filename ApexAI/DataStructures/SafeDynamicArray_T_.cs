using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Apex.DataStructures
{
	public sealed class SafeDynamicArray<T> : IDynamicArray<T>, IIterable<T>, IEnumerable<T>, IEnumerable, IIndexable<T>
	{
		private IDynamicArray<T> _array;

		public int count
		{
			get
			{
				for (int i = this._array.count - 1; i >= 0; i--)
				{
					if (this._array[i] == null || this._array[i].Equals(null))
					{
						this._array.RemoveAt(i);
					}
				}
				return this._array.count;
			}
		}

		public T this[int idx]
		{
			get
			{
				return this._array[idx];
			}
		}

		public SafeDynamicArray(IDynamicArray<T> array)
		{
			this._array = array;
		}

		public void Add(T item)
		{
			this._array.Add(item);
		}

		public void Clear()
		{
			this._array.Clear();
		}

		public void EnsureCapacity(int capacity)
		{
			this._array.EnsureCapacity(capacity);
		}

		public bool Remove(T item)
		{
			return this._array.Remove(item);
		}

		public void RemoveAt(int index)
		{
			this._array.RemoveAt(index);
		}

		IEnumerator<T> System.Collections.Generic.IEnumerable<T>.GetEnumerator()
		{
			SafeDynamicArray<T> ts = null;
			int num = ts._array.count;
			for (int i = 0; i < num; i++)
			{
				if (ts._array[i] != null && !ts._array[i].Equals(null))
				{
					yield return ts._array[i];
				}
			}
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			SafeDynamicArray<T> ts = null;
			int num = ts._array.count;
			for (int i = 0; i < num; i++)
			{
				if (ts._array[i] != null && !ts._array[i].Equals(null))
				{
					yield return ts._array[i];
				}
			}
		}
	}
}