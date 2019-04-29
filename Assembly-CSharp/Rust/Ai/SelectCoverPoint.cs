using Apex.AI;
using Apex.Serialization;
using System;
using System.Collections.Generic;

namespace Rust.Ai
{
	public class SelectCoverPoint : ActionWithOptions<CoverPoint>
	{
		[ApexSerialization]
		private bool allScorersMustScoreAboveZero = true;

		public SelectCoverPoint()
		{
		}

		public static bool Evaluate(CoverContext context, IList<IOptionScorer<CoverPoint>> scorers, List<CoverPoint> options, int numOptions, bool allScorersMustScoreAboveZero)
		{
			for (int i = 0; i < numOptions; i++)
			{
				float single = 0f;
				for (int j = 0; j < scorers.Count; j++)
				{
					if (!scorers[j].isDisabled)
					{
						float single1 = scorers[j].Score(context, options[i]);
						if (allScorersMustScoreAboveZero && single1 <= 0f)
						{
							break;
						}
						single += single1;
					}
				}
			}
			if (context.BestAdvanceCP != null || context.BestFlankCP != null)
			{
				return true;
			}
			return context.BestRetreatCP != null;
		}

		public override void Execute(IAIContext context)
		{
			CoverContext coverContext = context as CoverContext;
			if (coverContext != null)
			{
				SelectCoverPoint.Evaluate(coverContext, base.scorers, coverContext.SampledCoverPoints, coverContext.SampledCoverPoints.Count, this.allScorersMustScoreAboveZero);
			}
		}
	}
}