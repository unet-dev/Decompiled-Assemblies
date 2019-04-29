using Apex.LoadBalancing;
using System;

namespace Apex.AI
{
	public sealed class AILoadBalancer : LoadBalancer
	{
		public readonly static ILoadBalancer aiLoadBalancer;

		static AILoadBalancer()
		{
			AILoadBalancer.aiLoadBalancer = new LoadBalancedQueue(20, 1f, 200, 4);
		}

		private AILoadBalancer()
		{
		}
	}
}