using System;
using System.Collections.Generic;

namespace Apex.DataStructures
{
	public class PriorityQueueLifo<T> : BinaryHeapBase<PriorityQueueLifo<T>.QueueItemLifo>
	{
		private int _entryIdx;

		public PriorityQueueLifo(int capacity, QueueType type) : base(capacity, (type == QueueType.Max ? PriorityQueueLifo<T>.ItemComparerMax.instance : PriorityQueueLifo<T>.ItemComparerMin.instance))
		{
		}

		public T Dequeue()
		{
			return base.RemoveInternal().item;
		}

		public void Enqueue(T item, int priority)
		{
			int num = this._entryIdx;
			this._entryIdx = num + 1;
			base.AddInternal(new PriorityQueueLifo<T>.QueueItemLifo(item, priority, num));
		}

		private class ItemComparerMax : IComparer<PriorityQueueLifo<T>.QueueItemLifo>
		{
			public readonly static IComparer<PriorityQueueLifo<T>.QueueItemLifo> instance;

			static ItemComparerMax()
			{
				PriorityQueueLifo<T>.ItemComparerMax.instance = new PriorityQueueLifo<T>.ItemComparerMax();
			}

			public ItemComparerMax()
			{
			}

			public int Compare(PriorityQueueLifo<T>.QueueItemLifo x, PriorityQueueLifo<T>.QueueItemLifo y)
			{
				int num = x.priority;
				int num1 = num.CompareTo(y.priority);
				if (num1 != 0)
				{
					return num1;
				}
				num = x.entryOrder;
				return num.CompareTo(y.entryOrder);
			}
		}

		private class ItemComparerMin : IComparer<PriorityQueueLifo<T>.QueueItemLifo>
		{
			public readonly static IComparer<PriorityQueueLifo<T>.QueueItemLifo> instance;

			static ItemComparerMin()
			{
				PriorityQueueLifo<T>.ItemComparerMin.instance = new PriorityQueueLifo<T>.ItemComparerMin();
			}

			public ItemComparerMin()
			{
			}

			public int Compare(PriorityQueueLifo<T>.QueueItemLifo x, PriorityQueueLifo<T>.QueueItemLifo y)
			{
				int num = y.priority;
				int num1 = num.CompareTo(x.priority);
				if (num1 != 0)
				{
					return num1;
				}
				num = x.entryOrder;
				return num.CompareTo(y.entryOrder);
			}
		}

		public struct QueueItemLifo
		{
			private T _item;

			private int _priority;

			private int _entryOrder;

			public int entryOrder
			{
				get
				{
					return this._entryOrder;
				}
			}

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

			public QueueItemLifo(T item, int priority, int entryOrder)
			{
				this._item = item;
				this._priority = priority;
				this._entryOrder = entryOrder;
			}
		}
	}
}