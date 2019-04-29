using System;

namespace Apex.LoadBalancing
{
	public interface IMarshaller
	{
		void ExecuteOnMainThread(Action a);
	}
}