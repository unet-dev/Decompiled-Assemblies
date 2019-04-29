using System;

namespace Apex.AI
{
	public interface IRequireTermination
	{
		void Terminate(IAIContext context);
	}
}