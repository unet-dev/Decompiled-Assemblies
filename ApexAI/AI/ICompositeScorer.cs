using Apex.Ai.HTN;
using System;
using System.Collections.Generic;

namespace Apex.AI
{
	public interface ICompositeScorer
	{
		IList<IContextualScorer> scorers
		{
			get;
		}

		float Score(IAIContext context, IList<IContextualScorer> scorers);

		bool Validate(IHTNContext context, IList<IContextualScorer> scorers);
	}
}