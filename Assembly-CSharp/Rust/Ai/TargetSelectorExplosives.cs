using Apex.AI;
using Apex.Serialization;
using System;
using System.Collections.Generic;

namespace Rust.Ai
{
	public class TargetSelectorExplosives : ActionWithOptions<BaseEntity>
	{
		[ApexSerialization]
		private bool allScorersMustScoreAboveZero = true;

		public TargetSelectorExplosives()
		{
		}

		public override void Execute(IAIContext context)
		{
			EntityTargetContext entityTargetContext = context as EntityTargetContext;
			if (entityTargetContext != null)
			{
				TargetSelectorExplosives.TryGetBest(entityTargetContext, base.scorers, entityTargetContext.Entities, entityTargetContext.EntityCount, this.allScorersMustScoreAboveZero, out entityTargetContext.ExplosiveTarget, out entityTargetContext.ExplosiveScore);
			}
		}

		public static bool TryGetBest(EntityTargetContext context, IList<IOptionScorer<BaseEntity>> scorers, BaseEntity[] options, int numOptions, bool allScorersMustScoreAboveZero, out TimedExplosive best, out float bestScore)
		{
			bestScore = Single.MinValue;
			best = null;
			BaseEntity baseEntity = null;
			for (int i = 0; i < numOptions; i++)
			{
				float single = 0f;
				bool flag = true;
				for (int j = 0; j < scorers.Count; j++)
				{
					if (!scorers[j].isDisabled)
					{
						float single1 = scorers[j].Score(context, options[i]);
						if (!allScorersMustScoreAboveZero || single1 > 0f)
						{
							single += single1;
						}
						else
						{
							flag = false;
							break;
						}
					}
				}
				if (flag && single > bestScore)
				{
					bestScore = single;
					baseEntity = options[i];
				}
			}
			if (baseEntity != null)
			{
				best = baseEntity as TimedExplosive;
			}
			return best != null;
		}
	}
}