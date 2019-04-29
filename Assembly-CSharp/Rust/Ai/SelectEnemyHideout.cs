using Apex.AI;
using Apex.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
	public class SelectEnemyHideout : ActionWithOptions<CoverPoint>
	{
		[ApexSerialization]
		private bool allScorersMustScoreAboveZero = true;

		public SelectEnemyHideout()
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
				if (single > context.HideoutValue)
				{
					context.HideoutCP = options[i];
				}
			}
			if (context.HideoutCP == null)
			{
				return false;
			}
			NPCPlayerApex entity = context.Self.Entity as NPCPlayerApex;
			if (entity != null)
			{
				List<NPCHumanContext.HideoutPoint> checkedHideoutPoints = entity.AiContext.CheckedHideoutPoints;
				NPCHumanContext.HideoutPoint hideoutPoint = new NPCHumanContext.HideoutPoint()
				{
					Hideout = context.HideoutCP,
					Time = Time.time
				};
				checkedHideoutPoints.Add(hideoutPoint);
			}
			return true;
		}

		public override void Execute(IAIContext context)
		{
			CoverContext coverContext = context as CoverContext;
			if (coverContext != null)
			{
				SelectEnemyHideout.Evaluate(coverContext, base.scorers, coverContext.SampledCoverPoints, coverContext.SampledCoverPoints.Count, this.allScorersMustScoreAboveZero);
			}
		}
	}
}