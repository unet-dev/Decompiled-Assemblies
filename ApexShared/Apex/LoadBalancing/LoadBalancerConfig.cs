using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Apex.LoadBalancing
{
	[Serializable]
	public class LoadBalancerConfig
	{
		public float updateInterval = 0.1f;

		public int maxUpdatesPerFrame = 20;

		public int maxUpdateTimeInMillisecondsPerUpdate = 4;

		public bool autoAdjust;

		[HideInInspector]
		[SerializeField]
		public string targetLoadBalancer;

		public LoadBalancedQueue associatedLoadBalancer
		{
			get;
			private set;
		}

		public LoadBalancerConfig()
		{
		}

		internal void ApplyTo(LoadBalancedQueue q)
		{
			q.defaultUpdateInterval = this.updateInterval;
			q.maxUpdatesPerInterval = this.maxUpdatesPerFrame;
			q.maxUpdateTimeInMillisecondsPerUpdate = this.maxUpdateTimeInMillisecondsPerUpdate;
			q.autoAdjust = this.autoAdjust;
			this.associatedLoadBalancer = q;
		}

		internal static LoadBalancerConfig From(string name, LoadBalancedQueue q)
		{
			return new LoadBalancerConfig()
			{
				associatedLoadBalancer = q,
				autoAdjust = q.autoAdjust,
				maxUpdatesPerFrame = q.maxUpdatesPerInterval,
				maxUpdateTimeInMillisecondsPerUpdate = q.maxUpdateTimeInMillisecondsPerUpdate,
				updateInterval = q.defaultUpdateInterval,
				targetLoadBalancer = name
			};
		}
	}
}