using System;

namespace Apex.AI
{
	public interface ISelect
	{
		Guid id
		{
			get;
		}

		IAction Select(IAIContext context);
	}
}