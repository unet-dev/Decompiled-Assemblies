using System;

namespace Apex.LoadBalancing
{
	public interface ILoadBalancedHandle
	{
		bool isDisposed
		{
			get;
		}

		ILoadBalanced item
		{
			get;
		}

		void Pause();

		void Resume();

		void Stop();
	}
}