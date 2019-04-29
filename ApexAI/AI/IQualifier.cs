using System;

namespace Apex.AI
{
	public interface IQualifier : ICanBeDisabled
	{
		IAction action
		{
			get;
			set;
		}

		float Score(IAIContext context);
	}
}