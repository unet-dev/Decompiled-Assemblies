using Apex.LoadBalancing;
using System;

public sealed class AiManagerLoadBalancer : Apex.LoadBalancing.LoadBalancer
{
	public readonly static ILoadBalancer aiManagerLoadBalancer;

	static AiManagerLoadBalancer()
	{
		AiManagerLoadBalancer.aiManagerLoadBalancer = new LoadBalancedQueue(1, 2.5f, 1, 4);
	}

	private AiManagerLoadBalancer()
	{
	}
}