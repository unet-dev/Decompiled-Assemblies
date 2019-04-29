using System;

namespace Apex.AI
{
	public interface IAction
	{
		void Execute(IAIContext context);
	}
}