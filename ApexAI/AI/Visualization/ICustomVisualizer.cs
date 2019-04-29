using Apex.AI;
using System;

namespace Apex.AI.Visualization
{
	public interface ICustomVisualizer
	{
		void EntityUpdate(object aiEntity, IAIContext context, Guid aiId);
	}
}