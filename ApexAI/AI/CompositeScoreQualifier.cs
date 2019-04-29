using System;
using System.Collections.Generic;

namespace Apex.AI
{
	[AICategory("Composite Qualifiers")]
	[FriendlyName("Sum of Children", "Scores by summing the score of all child scorers.")]
	public class CompositeScoreQualifier : CompositeQualifier
	{
		public CompositeScoreQualifier()
		{
		}

		public sealed override float Score(IAIContext context, IList<IContextualScorer> scorers)
		{
			float single = 0f;
			int count = scorers.Count;
			for (int i = 0; i < count; i++)
			{
				IContextualScorer item = scorers[i];
				if (!item.isDisabled)
				{
					single += item.Score(context);
				}
			}
			return single;
		}
	}
}