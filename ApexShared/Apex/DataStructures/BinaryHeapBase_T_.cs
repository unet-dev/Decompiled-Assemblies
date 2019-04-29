using System;
using System.Collections.Generic;

namespace Apex.DataStructures
{
	public abstract class BinaryHeapBase<T>
	{
		private int _used;

		private T[] _heap;

		private IComparer<T> _comparer;

		public int capacity
		{
			get
			{
				return (int)this._heap.Length;
			}
		}

		public int count
		{
			get
			{
				return this._used;
			}
		}

		public bool hasNext
		{
			get
			{
				return this._used > 0;
			}
		}

		protected BinaryHeapBase(int capacity, IComparer<T> comparer)
		{
			if (capacity < 4)
			{
				capacity = 4;
			}
			this._heap = new T[capacity];
			this._comparer = comparer;
		}

		protected void AddInternal(T item)
		{
			if (this._used == (int)this._heap.Length)
			{
				this.Resize();
			}
			T[] tArray = this._heap;
			int num = this._used;
			this._used = num + 1;
			tArray[num] = item;
			this.ReheapifyUp(this._used - 1);
		}

		public void Clear()
		{
			Array.Clear(this._heap, 0, this._used);
			this._used = 0;
		}

		public T Peek()
		{
			if (this._used == 0)
			{
				throw new InvalidOperationException("The Heap is empty, Peek cannot be called on an empty heap");
			}
			return this._heap[0];
		}

		private void ReheapifyDown(int currentIdx)
		{
			int num;
			T t = this._heap[currentIdx];
			while (true)
			{
				int num1 = currentIdx * 2 + 1;
				if (num1 >= this._used)
				{
					break;
				}
				int num2 = currentIdx * 2 + 2;
				if (num2 < this._used)
				{
					num = (this._comparer.Compare(this._heap[num1], this._heap[num2]) > 0 ? num1 : num2);
				}
				else
				{
					num = num1;
				}
				if (this._comparer.Compare(t, this._heap[num]) >= 0)
				{
					break;
				}
				this._heap[currentIdx] = this._heap[num];
				currentIdx = num;
			}
			this._heap[currentIdx] = t;
		}

		public void ReheapifyDownFrom(T item)
		{
			int num = Array.IndexOf<T>(this._heap, item, 0);
			if (num < 0)
			{
				return;
			}
			this.ReheapifyDown(num);
		}

		public void ReheapifyDownFrom(int childIdx)
		{
			if (childIdx < 0 || childIdx >= (int)this._heap.Length)
			{
				throw new ArgumentOutOfRangeException("childIdx", "Specified index is outside the valid range.");
			}
			this.ReheapifyDown(childIdx);
		}

		private void ReheapifyUp(int childIdx)
		{
			int num = (childIdx - 1) / 2;
			T t = this._heap[childIdx];
			while (childIdx > 0 && this._comparer.Compare(t, this._heap[num]) > 0)
			{
				this._heap[childIdx] = this._heap[num];
				childIdx = num;
				num = (childIdx - 1) / 2;
			}
			this._heap[childIdx] = t;
		}

		public void ReheapifyUpFrom(T item)
		{
			int num = Array.IndexOf<T>(this._heap, item, 1);
			if (num < 1)
			{
				return;
			}
			this.ReheapifyUp(num);
		}

		public T Remove(T item)
		{
			int num = Array.IndexOf<T>(this._heap, item, 0);
			if (num < 0)
			{
				return default(T);
			}
			return this.Remove(num);
		}

		public T Remove(Func<T, bool> predicate)
		{
			int num = -1;
			int num1 = 0;
			while (num1 < this._used)
			{
				if (!predicate(this._heap[num1]))
				{
					num1++;
				}
				else
				{
					num = num1;
					break;
				}
			}
			if (num < 0)
			{
				return default(T);
			}
			return this.Remove(num);
		}

		private T Remove(int idx)
		{
			if (this._used == 0)
			{
				throw new InvalidOperationException("The Heap is empty, Remove cannot be called on an empty heap");
			}
			T t = this._heap[idx];
			this._used--;
			this._heap[idx] = this._heap[this._used];
			this._heap[this._used] = default(T);
			this.ReheapifyDown(idx);
			return t;
		}

		protected T RemoveInternal()
		{
			return this.Remove(0);
		}

		public void Resize()
		{
			T[] tArray = new T[(int)this._heap.Length * 2];
			if (this._used > 0)
			{
				Array.Copy(this._heap, tArray, this._used);
			}
			this._heap = tArray;
		}
	}
}