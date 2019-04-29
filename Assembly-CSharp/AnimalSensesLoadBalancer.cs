using Apex.LoadBalancing;
using System;

public sealed class AnimalSensesLoadBalancer : Apex.LoadBalancing.LoadBalancer
{
	public readonly static ILoadBalancer animalSensesLoadBalancer;

	static AnimalSensesLoadBalancer()
	{
		AnimalSensesLoadBalancer.animalSensesLoadBalancer = new LoadBalancedQueue(300, 0.1f, 50, 4);
	}

	private AnimalSensesLoadBalancer()
	{
	}
}