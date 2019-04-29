using Apex.Serialization;
using System;
using System.Collections.Generic;

namespace Apex.AI
{
	[AICategory("Composite Qualifiers")]
	[FriendlyName("Minimum or Nothing", "Scores by summing child scores. Only returns a score if the sum exceeds the threshold, otherwise 0 is returned.")]
	public class CompositeScoreAboveThresholdQualifier : CompositeQualifier
	{
		[ApexSerialization(defaultValue=0f)]
		public float threshold;

		public CompositeScoreAboveThresholdQualifier()
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
			if (single <= this.threshold)
			{
				return 0f;
			}
			return single;
		}
	}
}