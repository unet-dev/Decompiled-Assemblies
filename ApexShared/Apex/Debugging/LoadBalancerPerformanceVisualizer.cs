using Apex;
using Apex.LoadBalancing;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Apex.Debugging
{
	[AddComponentMenu("Apex/Game World/Debugging/LoadBalancer Performance Visualizer", 1014)]
	[ApexComponent("Debugging")]
	public class LoadBalancerPerformanceVisualizer : MonoBehaviour
	{
		private LoadBalancerPerformanceVisualizer.PerformanceData[] _data;

		public LoadBalancerPerformanceVisualizer.PerformanceData[] data
		{
			get
			{
				return this._data;
			}
		}

		public LoadBalancerPerformanceVisualizer()
		{
		}

		private void Awake()
		{
			List<LoadBalancerPerformanceVisualizer.PerformanceData> performanceDatas = new List<LoadBalancerPerformanceVisualizer.PerformanceData>();
			LoadBalancerComponent component = base.GetComponent<LoadBalancerComponent>();
			if (component != null)
			{
				LoadBalancerConfig[] loadBalancerConfigArray = component.configurations;
				for (int i = 0; i < (int)loadBalancerConfigArray.Length; i++)
				{
					LoadBalancerConfig loadBalancerConfig = loadBalancerConfigArray[i];
					performanceDatas.Add(new LoadBalancerPerformanceVisualizer.PerformanceData(loadBalancerConfig.associatedLoadBalancer, loadBalancerConfig.targetLoadBalancer));
				}
			}
			else
			{
				Debug.LogWarning("LoadBalancer Performance Visualizer must reside on the same GameObject as the Load Balancer.");
				base.enabled = false;
			}
			this._data = performanceDatas.ToArray();
		}

		private void Update()
		{
			for (int i = 0; i < (int)this._data.Length; i++)
			{
				this._data[i].Update();
			}
		}

		public class PerformanceData
		{
			private long _updateCount;

			private LoadBalancedQueue _source;

			private float _summedUpdatesOverdueAverage;

			private float _summedUpdateMillisecondsUsed;

			private float _summedUpdatedItemsCount;

			public float averageUpdatedItemsCount
			{
				get
				{
					return this._summedUpdatedItemsCount / (float)Math.Max(this._updateCount, (long)1);
				}
			}

			public float averageUpdateMillisecondsUsed
			{
				get
				{
					return this._summedUpdateMillisecondsUsed / (float)Math.Max(this._updateCount, (long)1);
				}
			}

			public float averageUpdatesOverdueAverage
			{
				get
				{
					return this._summedUpdatesOverdueAverage / (float)Math.Max(this._updateCount, (long)1);
				}
			}

			public int frameUpdatedItemsCount
			{
				get
				{
					return this._source.updatedItemsCount;
				}
			}

			public long frameUpdateMillisecondsUsed
			{
				get
				{
					return this._source.updateMillisecondsUsed;
				}
			}

			public float frameUpdatesOverdueAverage
			{
				get
				{
					if (this._source.updatedItemsCount == 0)
					{
						return 0f;
					}
					return Mathf.Clamp(this._source.updatesOverdueByTotal / (float)this._source.updatedItemsCount - 0.02f, 0f, Single.MaxValue);
				}
			}

			public int itemsCount
			{
				get
				{
					return this._source.itemCount;
				}
			}

			public string loadBalancerName
			{
				get;
				private set;
			}

			public PerformanceData(LoadBalancedQueue source, string loadBalancerName)
			{
				this._source = source;
				this.loadBalancerName = loadBalancerName;
			}

			internal void Update()
			{
				if (this.frameUpdatedItemsCount > 0)
				{
					this._summedUpdatesOverdueAverage += this.frameUpdatesOverdueAverage;
					this._summedUpdateMillisecondsUsed += (float)this.frameUpdateMillisecondsUsed;
					this._summedUpdatedItemsCount += (float)this.frameUpdatedItemsCount;
					this._updateCount = this._updateCount % 9223372036854775807L + (long)1;
				}
			}
		}
	}
}