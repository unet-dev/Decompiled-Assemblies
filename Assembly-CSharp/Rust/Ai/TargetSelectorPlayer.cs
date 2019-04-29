using Apex.AI;
using Apex.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
	public class TargetSelectorPlayer : ActionWithOptions<BasePlayer>
	{
		[ApexSerialization]
		private bool allScorersMustScoreAboveZero = true;

		public TargetSelectorPlayer()
		{
		}

		public static bool Evaluate(PlayerTargetContext context, IList<IOptionScorer<BasePlayer>> scorers, BasePlayer[] options, int numOptions, bool allScorersMustScoreAboveZero, out BasePlayer best, out float bestScore, out int bestIndex, out Vector3 bestLastKnownPosition)
		{
			Memory.ExtendedInfo extendedInfo;
			bestScore = Single.MinValue;
			best = null;
			bestIndex = -1;
			bestLastKnownPosition = Vector3.zero;
			for (int i = 0; i < numOptions; i++)
			{
				context.CurrentOptionsIndex = i;
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
				if (flag)
				{
					Vector3 position = Vector3.zero;
					BaseContext baseContext = context.Self.GetContext(Guid.Empty) as BaseContext;
					if (baseContext != null)
					{
						NPCPlayerApex self = context.Self as NPCPlayerApex;
						position = baseContext.Memory.Update(options[i], single, context.Direction[i], context.Dot[i], context.DistanceSqr[i], context.LineOfSight[i], (self == null ? false : self.lastAttacker == options[i]), (self != null ? self.lastAttackedTime : 0f), out extendedInfo).Position;
					}
					if (single > bestScore)
					{
						bestScore = single;
						best = options[i];
						bestIndex = i;
						bestLastKnownPosition = position;
					}
				}
			}
			return best != null;
		}

		public override void Execute(IAIContext context)
		{
			PlayerTargetContext playerTargetContext = context as PlayerTargetContext;
			if (playerTargetContext != null)
			{
				TargetSelectorPlayer.Evaluate(playerTargetContext, base.scorers, playerTargetContext.Players, playerTargetContext.PlayerCount, this.allScorersMustScoreAboveZero, out playerTargetContext.Target, out playerTargetContext.Score, out playerTargetContext.Index, out playerTargetContext.LastKnownPosition);
			}
		}
	}
}