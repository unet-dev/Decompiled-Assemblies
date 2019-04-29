using Apex.Serialization;
using System;
using System.Collections.Generic;

namespace Apex.AI
{
	[AICategory("Composite Qualifiers")]
	[FriendlyName("Sum all above threshold", "Scores by summing child scores that score above the threshold")]
	public class CompositeAllAboveThresholdQualifier : CompositeQualifier
	{
		[ApexSerialization(defaultValue=0f)]
		public float threshold;

		public CompositeAllAboveThresholdQualifier()
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
					float single1 = item.Score(context);
					if (single1 > this.threshold)
					{
						single += single1;
					}
				}
			}
			return single;
		}
	}
}