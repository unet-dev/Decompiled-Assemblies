using Apex.DataStructures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Apex.LoadBalancing
{
	public sealed class LoadBalancedQueue : ILoadBalancer
	{
		private BinaryHeap<LoadBalancedQueue.LoadBalancerItem> _queue;

		private Stopwatch _watch;

		public bool autoAdjust
		{
			get;
			set;
		}

		public float defaultUpdateInterval
		{
			get
			{
				return JustDecompileGenerated_get_defaultUpdateInterval();
			}
			set
			{
				JustDecompileGenerated_set_defaultUpdateInterval(value);
			}
		}

		private float JustDecompileGenerated_defaultUpdateInterval_k__BackingField;

		public float JustDecompileGenerated_get_defaultUpdateInterval()
		{
			return this.JustDecompileGenerated_defaultUpdateInterval_k__BackingField;
		}

		public void JustDecompileGenerated_set_defaultUpdateInterval(float value)
		{
			this.JustDecompileGenerated_defaultUpdateInterval_k__BackingField = value;
		}

		public int itemCount
		{
			get
			{
				return this._queue.count;
			}
		}

		public int maxUpdatesPerInterval
		{
			get;
			set;
		}

		public int maxUpdateTimeInMillisecondsPerUpdate
		{
			get;
			set;
		}

		public int updatedItemsCount
		{
			get;
			private set;
		}

		public long updateMillisecondsUsed
		{
			get;
			private set;
		}

		public float updatesOverdueByTotal
		{
			get;
			private set;
		}

		public LoadBalancedQueue(int capacity) : this(capacity, 0.1f, 20, 4, false)
		{
		}

		public LoadBalancedQueue(int capacity, float defaultUpdateInterval, bool autoAdjust) : this(capacity, defaultUpdateInterval, 20, 4, autoAdjust)
		{
		}

		public LoadBalancedQueue(int capacity, float defaultUpdateInterval, int maxUpdatesPerInterval, int maxUpdateTimeInMillisecondsPerUpdate) : this(capacity, defaultUpdateInterval, maxUpdatesPerInterval, maxUpdateTimeInMillisecondsPerUpdate, false)
		{
		}

		private LoadBalancedQueue(int capacity, float defaultUpdateInterval, int maxUpdatesPerInterval, int maxUpdateTimeInMillisecondsPerUpdate, bool autoAdjust)
		{
			this.defaultUpdateInterval = defaultUpdateInterval;
			this.maxUpdatesPerInterval = maxUpdatesPerInterval;
			this.maxUpdateTimeInMillisecondsPerUpdate = maxUpdateTimeInMillisecondsPerUpdate;
			this.autoAdjust = autoAdjust;
			this._queue = new BinaryHeap<LoadBalancedQueue.LoadBalancerItem>(capacity, LoadBalancedQueue.LoadBalanceItemComparer.instance);
			this._watch = new Stopwatch();
		}

		public ILoadBalancedHandle Add(ILoadBalanced item)
		{
			return this.Add(item, this.defaultUpdateInterval, 0f);
		}

		public ILoadBalancedHandle Add(ILoadBalanced item, bool delayFirstUpdate)
		{
			return this.Add(item, this.defaultUpdateInterval, delayFirstUpdate);
		}

		public ILoadBalancedHandle Add(ILoadBalanced item, float interval)
		{
			return this.Add(item, interval, 0f);
		}

		public ILoadBalancedHandle Add(ILoadBalanced item, float interval, bool delayFirstUpdate)
		{
			return this.Add(item, interval, (delayFirstUpdate ? interval : 0f));
		}

		public ILoadBalancedHandle Add(ILoadBalanced item, float interval, float delayFirstUpdateBy)
		{
			float single = Time.time;
			LoadBalancedQueue.LoadBalancerItem loadBalancerItem = new LoadBalancedQueue.LoadBalancerItem()
			{
				parent = this,
				lastUpdate = single,
				nextUpdate = single + delayFirstUpdateBy,
				interval = interval,
				item = item
			};
			this._queue.Add(loadBalancerItem);
			return loadBalancerItem;
		}

		public void Remove(ILoadBalanced item)
		{
			LoadBalancedQueue.LoadBalancerItem loadBalancerItem = this._queue.Remove((LoadBalancedQueue.LoadBalancerItem o) => o.item == item);
			if (loadBalancerItem != null)
			{
				loadBalancerItem.Dispose();
			}
		}

		private void Remove(LoadBalancedQueue.LoadBalancerItem item)
		{
			this._queue.Remove(item);
		}

		public void Update()
		{
			if (!this._queue.hasNext)
			{
				return;
			}
			float single = Time.time;
			this._watch.Reset();
			this._watch.Start();
			int num = this.maxUpdatesPerInterval;
			int num1 = 0;
			float single1 = 0f;
			if (this.autoAdjust)
			{
				float single2 = this.defaultUpdateInterval / Time.deltaTime;
				num = Mathf.CeilToInt((float)this._queue.count / single2);
			}
			LoadBalancedQueue.LoadBalancerItem loadBalancerItem = this._queue.Peek();
			while (true)
			{
				int num2 = num1;
				num1 = num2 + 1;
				if (num2 >= num || loadBalancerItem.nextUpdate > single || !this.autoAdjust && this._watch.ElapsedMilliseconds >= (long)this.maxUpdateTimeInMillisecondsPerUpdate)
				{
					break;
				}
				float single3 = single - loadBalancerItem.lastUpdate;
				single1 = single1 + (single3 - loadBalancerItem.interval);
				float? nullable = loadBalancerItem.item.ExecuteUpdate(single3, loadBalancerItem.interval);
				float valueOrDefault = nullable.GetValueOrDefault(loadBalancerItem.interval);
				if (!loadBalancerItem.item.repeat)
				{
					this._queue.Remove().Dispose();
				}
				else
				{
					valueOrDefault = Mathf.Max(valueOrDefault, 0.01f);
					loadBalancerItem.lastUpdate = single;
					loadBalancerItem.nextUpdate = single + valueOrDefault;
					this._queue.ReheapifyDownFrom(0);
				}
				if (!this._queue.hasNext)
				{
					break;
				}
				loadBalancerItem = this._queue.Peek();
			}
			this.updatedItemsCount = num1 - 1;
			this.updatesOverdueByTotal = single1;
			this.updateMillisecondsUsed = this._watch.ElapsedMilliseconds;
		}

		private sealed class LoadBalanceItemComparer : IComparer<LoadBalancedQueue.LoadBalancerItem>
		{
			public readonly static IComparer<LoadBalancedQueue.LoadBalancerItem> instance;

			static LoadBalanceItemComparer()
			{
				LoadBalancedQueue.LoadBalanceItemComparer.instance = new LoadBalancedQueue.LoadBalanceItemComparer();
			}

			public LoadBalanceItemComparer()
			{
			}

			public int Compare(LoadBalancedQueue.LoadBalancerItem x, LoadBalancedQueue.LoadBalancerItem y)
			{
				return y.nextUpdate.CompareTo(x.nextUpdate);
			}
		}

		private class LoadBalancerItem : ILoadBalancedHandle
		{
			private bool _isDisposed;

			internal LoadBalancedQueue parent;

			internal float nextUpdate;

			internal float lastUpdate;

			internal float interval;

			public bool isDisposed
			{
				get
				{
					return this._isDisposed;
				}
			}

			public ILoadBalanced item
			{
				get
				{
					return get_item();
				}
				set
				{
					set_item(value);
				}
			}

			private ILoadBalanced <item>k__BackingField;

			public ILoadBalanced get_item()
			{
				return this.<item>k__BackingField;
			}

			public void set_item(ILoadBalanced value)
			{
				this.<item>k__BackingField = value;
			}

			public LoadBalancerItem()
			{
			}

			internal void Dispose()
			{
				if (!this._isDisposed)
				{
					this._isDisposed = true;
					this.parent = null;
				}
			}

			public void Pause()
			{
				if (this._isDisposed)
				{
					return;
				}
				float single = Time.time;
				this.lastUpdate = this.nextUpdate - single;
				this.nextUpdate = Single.MaxValue;
				this.parent._queue.ReheapifyDownFrom(this);
			}

			public void Resume()
			{
				if (this._isDisposed)
				{
					return;
				}
				this.nextUpdate = Time.time + this.lastUpdate;
				this.lastUpdate = this.nextUpdate - this.interval;
				this.parent._queue.ReheapifyUpFrom(this);
			}

			public void Stop()
			{
				if (!this._isDisposed)
				{
					this.parent.Remove(this);
					this.Dispose();
				}
			}
		}
	}
}