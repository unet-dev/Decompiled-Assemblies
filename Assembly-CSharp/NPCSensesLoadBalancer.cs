using Apex.LoadBalancing;
using System;

public sealed class NPCSensesLoadBalancer : Apex.LoadBalancing.LoadBalancer
{
	public readonly static ILoadBalancer NpcSensesLoadBalancer;

	static NPCSensesLoadBalancer()
	{
		NPCSensesLoadBalancer.NpcSensesLoadBalancer = new LoadBalancedQueue(50, 0.1f, 50, 4);
	}

	private NPCSensesLoadBalancer()
	{
	}
}