using System;
using System.Collections.Generic;

namespace Apex.DataStructures
{
	public class PriorityQueueFifo<T> : BinaryHeapBase<PriorityQueueFifo<T>.QueueItemFifo>
	{
		private int _entryIdx;

		public PriorityQueueFifo(int capacity, QueueType type) : base(capacity, (type == QueueType.Max ? PriorityQueueFifo<T>.ItemComparerMax.instance : PriorityQueueFifo<T>.ItemComparerMin.instance))
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
			base.AddInternal(new PriorityQueueFifo<T>.QueueItemFifo(item, priority, num));
		}

		private class ItemComparerMax : IComparer<PriorityQueueFifo<T>.QueueItemFifo>
		{
			public readonly static IComparer<PriorityQueueFifo<T>.QueueItemFifo> instance;

			static ItemComparerMax()
			{
				PriorityQueueFifo<T>.ItemComparerMax.instance = new PriorityQueueFifo<T>.ItemComparerMax();
			}

			public ItemComparerMax()
			{
			}

			public int Compare(PriorityQueueFifo<T>.QueueItemFifo x, PriorityQueueFifo<T>.QueueItemFifo y)
			{
				int num = x.priority;
				int num1 = num.CompareTo(y.priority);
				if (num1 != 0)
				{
					return num1;
				}
				num = y.entryOrder;
				return num.CompareTo(x.entryOrder);
			}
		}

		private class ItemComparerMin : IComparer<PriorityQueueFifo<T>.QueueItemFifo>
		{
			public readonly static IComparer<PriorityQueueFifo<T>.QueueItemFifo> instance;

			static ItemComparerMin()
			{
				PriorityQueueFifo<T>.ItemComparerMin.instance = new PriorityQueueFifo<T>.ItemComparerMin();
			}

			public ItemComparerMin()
			{
			}

			public int Compare(PriorityQueueFifo<T>.QueueItemFifo x, PriorityQueueFifo<T>.QueueItemFifo y)
			{
				int num = y.priority;
				int num1 = num.CompareTo(x.priority);
				if (num1 != 0)
				{
					return num1;
				}
				num = y.entryOrder;
				return num.CompareTo(x.entryOrder);
			}
		}

		public struct QueueItemFifo
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

			public QueueItemFifo(T item, int priority, int entryOrder)
			{
				this._item = item;
				this._priority = priority;
				this._entryOrder = entryOrder;
			}
		}
	}
}