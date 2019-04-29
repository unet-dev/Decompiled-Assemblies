using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Apex.DataStructures
{
	public class SortedArray<T> : IDynamicArray<T>, IIterable<T>, IEnumerable<T>, IEnumerable, IIndexable<T>
	{
		private readonly static T[] _empty;

		private T[] _items;

		private int _capacity;

		private int _used;

		private IComparer<T> _sortComparer;

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
				return this._items[idx];
			}
		}

		static SortedArray()
		{
			SortedArray<T>._empty = new T[0];
		}

		public SortedArray(IComparer<T> sortComparer)
		{
			this._items = SortedArray<T>._empty;
			this._sortComparer = sortComparer;
		}

		public SortedArray(int capacity, IComparer<T> sortComparer)
		{
			this._items = new T[capacity];
			this._capacity = capacity;
			this._sortComparer = sortComparer;
		}

		public SortedArray(T[] source, IComparer<T> sortComparer)
		{
			int length = (int)source.Length;
			int num = length;
			this._capacity = length;
			this._used = num;
			this._items = new T[this._capacity];
			Array.Copy(source, this._items, this._capacity);
			this._sortComparer = sortComparer;
			Array.Sort<T>(this._items, 0, this._used, this._sortComparer);
		}

		public SortedArray(IIndexable<T> source, IComparer<T> sortComparer)
		{
			int num = source.count;
			int num1 = num;
			this._capacity = num;
			this._used = num1;
			this._items = new T[this._capacity];
			for (int i = 0; i < this._capacity; i++)
			{
				this._items[i] = source[i];
			}
			this._sortComparer = sortComparer;
			Array.Sort<T>(this._items, 0, this._used, this._sortComparer);
		}

		public SortedArray(IEnumerable<T> source, IComparer<T> sortComparer)
		{
			this._items = source.ToArray<T>();
			int length = (int)this._items.Length;
			int num = length;
			this._capacity = length;
			this._used = num;
			this._sortComparer = sortComparer;
			Array.Sort<T>(this._items, 0, this._used, this._sortComparer);
		}

		public void Add(T item)
		{
			if (this._used == this._capacity)
			{
				int num = Math.Max(this._capacity, 1);
				this.Resize(num * 2);
			}
			T[] tArray = this._items;
			int num1 = this._used;
			this._used = num1 + 1;
			tArray[num1] = item;
			Array.Sort<T>(this._items, 0, this._used, this._sortComparer);
		}

		public void AddRange(IIndexable<T> items)
		{
			int num = items.count + this._used;
			if (this._capacity < num)
			{
				this.Resize(num);
			}
			for (int i = 0; i < items.count; i++)
			{
				T[] item = this._items;
				int num1 = this._used;
				this._used = num1 + 1;
				item[num1] = items[i];
			}
			Array.Sort<T>(this._items, 0, this._used, this._sortComparer);
		}

		public void AddRange(IEnumerable<T> items)
		{
			int num = items.Count<T>() + this._used;
			if (this._capacity < num)
			{
				this.Resize(num);
			}
			foreach (T item in items)
			{
				T[] tArray = this._items;
				int num1 = this._used;
				this._used = num1 + 1;
				tArray[num1] = item;
			}
			Array.Sort<T>(this._items, 0, this._used, this._sortComparer);
		}

		public void Clear()
		{
			Array.Clear(this._items, 0, this._used);
			this._used = 0;
		}

		public void EnsureCapacity(int capacity)
		{
			if (this._capacity < capacity)
			{
				this.Resize(capacity);
			}
		}

		public bool Remove(T item)
		{
			for (int i = 0; i < this._used; i++)
			{
				if (this._items[i] != null && this._items[i].Equals(item))
				{
					this.RemoveAt(i);
					return true;
				}
			}
			return false;
		}

		public void RemoveAt(int index)
		{
			for (int i = index; i < this._used - 1; i++)
			{
				this._items[i] = this._items[i + 1];
			}
			this._items[this._used - 1] = default(T);
			this._used--;
		}

		private void Resize(int newCapacity)
		{
			this._capacity = newCapacity;
			T[] tArray = new T[this._capacity];
			Array.Copy(this._items, 0, tArray, 0, (int)this._items.Length);
			this._items = tArray;
		}

		IEnumerator<T> System.Collections.Generic.IEnumerable<T>.GetEnumerator()
		{
			SortedArray<T> ts = null;
			int num = ts.count;
			for (int i = 0; i < num; i++)
			{
				yield return ts._items[i];
			}
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			SortedArray<T> ts = null;
			int num = ts.count;
			for (int i = 0; i < num; i++)
			{
				yield return ts._items[i];
			}
		}
	}
}