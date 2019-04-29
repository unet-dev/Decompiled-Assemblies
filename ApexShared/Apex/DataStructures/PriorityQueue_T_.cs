using System;
using System.Collections.Generic;

namespace Apex.DataStructures
{
	public class PriorityQueue<T> : BinaryHeapBase<PriorityQueue<T>.QueueItem>
	{
		public PriorityQueue(int capacity, QueueType type) : base(capacity, (type == QueueType.Max ? PriorityQueue<T>.ItemComparerMax.instance : PriorityQueue<T>.ItemComparerMin.instance))
		{
		}

		public T Dequeue()
		{
			return base.RemoveInternal().item;
		}

		public void Enqueue(T item, int priority)
		{
			base.AddInternal(new PriorityQueue<T>.QueueItem(item, priority));
		}

		private class ItemComparerMax : IComparer<PriorityQueue<T>.QueueItem>
		{
			public readonly static IComparer<PriorityQueue<T>.QueueItem> instance;

			static ItemComparerMax()
			{
				PriorityQueue<T>.ItemComparerMax.instance = new PriorityQueue<T>.ItemComparerMax();
			}

			public ItemComparerMax()
			{
			}

			public int Compare(PriorityQueue<T>.QueueItem x, PriorityQueue<T>.QueueItem y)
			{
				return x.priority.CompareTo(y.priority);
			}
		}

		private class ItemComparerMin : IComparer<PriorityQueue<T>.QueueItem>
		{
			public readonly static IComparer<PriorityQueue<T>.QueueItem> instance;

			static ItemComparerMin()
			{
				PriorityQueue<T>.ItemComparerMin.instance = new PriorityQueue<T>.ItemComparerMin();
			}

			public ItemComparerMin()
			{
			}

			public int Compare(PriorityQueue<T>.QueueItem x, PriorityQueue<T>.QueueItem y)
			{
				return y.priority.CompareTo(x.priority);
			}
		}

		public struct QueueItem
		{
			private T _item;

			private int _priority;

			public T item
			{
				get
				{
					return this._item;
				}
			}

			public int priority
			{
				get
				{
					return this._priority;
				}
			}

			public QueueItem(T item, int priority)
			{
				this._item = item;
				this._priority = priority;
			}
		}
	}
}