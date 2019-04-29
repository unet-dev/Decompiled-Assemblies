using System;
using System.Collections.Generic;

namespace Apex.DataStructures
{
	public class BinaryHeap<T> : BinaryHeapBase<T>
	{
		public BinaryHeap(int capacity, IComparer<T> comparer) : base(capacity, comparer)
		{
		}

		public void Add(T item)
		{
			base.AddInternal(item);
		}

		public T Remove()
		{
			return base.RemoveInternal();
		}
	}
}