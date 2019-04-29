using System;
using System.Runtime.CompilerServices;

namespace Apex.LoadBalancing
{
	public class LoadBalancer
	{
		public readonly static ILoadBalancer defaultBalancer;

		public static IMarshaller marshaller
		{
			get;
			internal set;
		}

		static LoadBalancer()
		{
			LoadBalancer.defaultBalancer = new LoadBalancedQueue(4);
		}

		protected LoadBalancer()
		{
		}
	}
}