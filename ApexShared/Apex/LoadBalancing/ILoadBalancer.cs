using System;

namespace Apex.LoadBalancing
{
	public interface ILoadBalancer
	{
		float defaultUpdateInterval
		{
			get;
		}

		ILoadBalancedHandle Add(ILoadBalanced item);

		ILoadBalancedHandle Add(ILoadBalanced item, bool delayFirstUpdate);

		ILoadBalancedHandle Add(ILoadBalanced item, float interval);

		ILoadBalancedHandle Add(ILoadBalanced item, float interval, bool delayFirstUpdate);

		ILoadBalancedHandle Add(ILoadBalanced item, float interval, float delayFirstUpdateBy);

		void Remove(ILoadBalanced item);
	}
}