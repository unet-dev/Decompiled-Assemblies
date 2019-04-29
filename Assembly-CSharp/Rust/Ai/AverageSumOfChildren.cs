using Apex.AI;
using Apex.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
	public class AverageSumOfChildren : CompositeQualifier
	{
		[ApexSerialization]
		private bool normalize = true;

		[ApexSerialization]
		private float postNormalizeMultiplier = 1f;

		[ApexSerialization]
		private float MaxAverageScore = 100f;

		[ApexSerialization]
		private bool FailIfAnyScoreZero = true;

		public AverageSumOfChildren()
		{
		}

		public override float Score(IAIContext context, IList<IContextualScorer> scorers)
		{
			if (scorers.Count == 0)
			{
				return 0f;
			}
			float count = 0f;
			for (int i = 0; i < scorers.Count; i++)
			{
				float single = scorers[i].Score(context);
				if (this.FailIfAnyScoreZero && (single < 0f || Mathf.Approximately(single, 0f)))
				{
					return 0f;
				}
				count += single;
			}
			count /= (float)scorers.Count;
			if (!this.normalize)
			{
				return count;
			}
			count /= this.MaxAverageScore;
			return count * this.postNormalizeMultiplier;
		}
	}
}