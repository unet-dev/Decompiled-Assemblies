using System;

namespace Apex.AI
{
	public interface IContextualScorer : ICanBeDisabled
	{
		bool CanInvalidatePlan
		{
			get;
			set;
		}

		float Score(IAIContext context);
	}
}