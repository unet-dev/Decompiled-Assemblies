using System;

namespace Apex.LoadBalancing
{
	public interface ILoadBalanced
	{
		bool repeat
		{
			get;
		}

		float? ExecuteUpdate(float deltaTime, float nextInterval);
	}
}