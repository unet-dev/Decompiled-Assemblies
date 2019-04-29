using Apex.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Apex.AI
{
	[AICategory("Composite Qualifiers")]
	[FriendlyName("Sum must be above threshold", "Scores 0 if sum is below threshold.")]
	public class CompositeSumMustBeAboveThresholdQualifier : CompositeQualifier
	{
		[ApexSerialization(defaultValue=0f)]
		public float threshold;

		public CompositeSumMustBeAboveThresholdQualifier()
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
					if (single1 >= 0f)
					{
						single += single1;
						if (single > this.threshold)
						{
							break;
						}
					}
					else
					{
						Debug.LogWarning("SumMustBeAboveThreshold scorer does not support scores below 0!");
					}
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