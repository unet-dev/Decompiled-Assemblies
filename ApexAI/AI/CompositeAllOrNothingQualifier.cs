using Apex.Ai.HTN;
using Apex.Serialization;
using System;
using System.Collections.Generic;

namespace Apex.AI
{
	[AICategory("Composite Qualifiers")]
	[FriendlyName("All or Nothing", "Only scores if all child scorers score above the threshold.")]
	public class CompositeAllOrNothingQualifier : CompositeQualifier
	{
		[ApexSerialization(defaultValue=0f)]
		public float threshold;

		public CompositeAllOrNothingQualifier()
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
					if (single1 <= this.threshold)
					{
						return 0f;
					}
					single += single1;
				}
			}
			return single;
		}

		public sealed override bool Validate(IHTNContext context, IList<IContextualScorer> scorers)
		{
			int count = scorers.Count;
			for (int i = 0; i < count; i++)
			{
				IContextualScorer item = scorers[i];
				if (!item.isDisabled && item.CanInvalidatePlan && item.Score(context) <= this.threshold)
				{
					return false;
				}
			}
			return true;
		}
	}
}