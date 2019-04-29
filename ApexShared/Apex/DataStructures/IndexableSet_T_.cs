using Apex.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Apex.DataStructures
{
	public class IndexableSet<T> : IDynamicArray<T>, IIterable<T>, IEnumerable<T>, IEnumerable, IIndexable<T>, ISortable<T>
	{
		private HashSet<T> _hashset;

		private DynamicArray<T> _array;

		public int count
		{
			get
			{
				return this._hashset.Count;
			}
		}

		public T this[int idx]
		{
			get
			{
				return this._array[idx];
			}
		}

		public IndexableSet()
		{
			this._hashset = new HashSet<T>();
			this._array = new DynamicArray<T>();
		}

		public IndexableSet(int capacity)
		{
			Ensure.ArgumentInRange(() => capacity >= 0, "capacity", capacity, null);
			this._hashset = new HashSet<T>();
			this._array = new DynamicArray<T>(capacity);
		}

		public IndexableSet(params T[] items)
		{
			Ensure.ArgumentNotNull(items, "items");
			this._hashset = new HashSet<T>(items);
			this._array = new DynamicArray<T>(items);
		}

		public IndexableSet(IEnumerable<T> items)
		{
			Ensure.ArgumentNotNull(items, "items");
			this._hashset = new HashSet<T>(items);
			this._array = new DynamicArray<T>(items);
		}

		public void Add(T obj)
		{
			if (this._hashset.Add(obj))
			{
				this._array.Add(obj);
			}
		}

		public void AddRange(params T[] objects)
		{
			for (int i = 0; i < (int)objects.Length; i++)
			{
				this.Add(objects[i]);
			}
		}

		public void AddRange(IEnumerable<T> objects)
		{
			foreach (T @object in objects)
			{
				this.Add(@object);
			}
		}

		public void AddRange(IIndexable<T> objects)
		{
			for (int i = 0; i < objects.count; i++)
			{
				this.Add(objects[i]);
			}
		}

		public void Clear()
		{
			this._array.Clear();
			this._hashset.Clear();
		}

		public bool Contains(T obj)
		{
			return this._hashset.Contains(obj);
		}

		public void EnsureCapacity(int capacity)
		{
			this._array.EnsureCapacity(capacity);
		}

		public bool Remove(T obj)
		{
			if (!this._hashset.Remove(obj))
			{
				return false;
			}
			this._array.Remove(obj);
			return true;
		}

		public void RemoveAt(int index)
		{
			T item = this._array[index];
			this._array.RemoveAt(index);
			this._hashset.Remove(item);
		}

		public void Sort()
		{
			this._array.Sort();
		}

		public void Sort(IComparer<T> comparer)
		{
			this._array.Sort(comparer);
		}

		public void Sort(int index, int length)
		{
			this._array.Sort(index, length);
		}

		public void Sort(int index, int length, IComparer<T> comparer)
		{
			this._array.Sort(index, length, comparer);
		}

		IEnumerator<T> System.Collections.Generic.IEnumerable<T>.GetEnumerator()
		{
			IndexableSet<T> ts = null;
			int num = ts.count;
			for (int i = 0; i < num; i++)
			{
				yield return ts._array[i];
			}
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			IndexableSet<T> ts = null;
			int num = ts.count;
			for (int i = 0; i < num; i++)
			{
				yield return ts._array[i];
			}
		}
	}
}