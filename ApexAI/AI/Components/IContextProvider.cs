using Apex.AI;
using System;

namespace Apex.AI.Components
{
	public interface IContextProvider
	{
		IAIContext GetContext(Guid aiId);
	}
}