using Apex.LoadBalancing;
using System;

public sealed class HTNPlayerLoadBalancer : Apex.LoadBalancing.LoadBalancer
{
	public readonly static ILoadBalancer HTNPlayerBalancer;

	static HTNPlayerLoadBalancer()
	{
		HTNPlayerLoadBalancer.HTNPlayerBalancer = new LoadBalancedQueue(50, 0.1f, 50, 4);
	}

	private HTNPlayerLoadBalancer()
	{
	}
}