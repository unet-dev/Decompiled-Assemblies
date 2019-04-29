using System;

namespace Apex.AI
{
	public interface IOptionScorer<T> : ICanBeDisabled
	{
		float Score(IAIContext context, T option);
	}
}