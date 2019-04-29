using Apex.AI;
using System;
using System.Collections.Generic;

namespace Rust.Ai
{
	public class SharePlayerTargetComm : ActionBase<PlayerTargetContext>
	{
		public SharePlayerTargetComm()
		{
		}

		public override void Execute(PlayerTargetContext c)
		{
			List<AiAnswer_ShareEnemyTarget> aiAnswerShareEnemyTargets;
			Memory.ExtendedInfo extendedInfo;
			NPCPlayerApex self = c.Self as NPCPlayerApex;
			if (self != null)
			{
				if (self.AskQuestion(new AiQuestion_ShareEnemyTarget(), out aiAnswerShareEnemyTargets) > 0)
				{
					foreach (AiAnswer_ShareEnemyTarget aiAnswerShareEnemyTarget in aiAnswerShareEnemyTargets)
					{
						if (!(aiAnswerShareEnemyTarget.PlayerTarget != null) || !aiAnswerShareEnemyTarget.LastKnownPosition.HasValue || !self.HostilityConsideration(aiAnswerShareEnemyTarget.PlayerTarget))
						{
							continue;
						}
						c.Target = aiAnswerShareEnemyTarget.PlayerTarget;
						c.Score = 1f;
						c.LastKnownPosition = aiAnswerShareEnemyTarget.LastKnownPosition.Value;
						self.UpdateTargetMemory(c.Target, c.Score, c.LastKnownPosition, out extendedInfo);
						return;
					}
				}
			}
		}
	}
}