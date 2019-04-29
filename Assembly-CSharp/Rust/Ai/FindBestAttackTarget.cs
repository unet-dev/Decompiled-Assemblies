using Apex.Serialization;
using System;
using System.Collections.Generic;

namespace Rust.Ai
{
	public class FindBestAttackTarget : BaseActionWithOptions<BaseEntity>
	{
		[ApexSerialization]
		public float ScoreThreshold;

		[ApexSerialization]
		public bool AllScorersMustScoreAboveZero;

		public FindBestAttackTarget()
		{
		}

		public override void DoExecute(BaseContext c)
		{
			BaseEntity baseEntity;
			float single;
			if (!base.TryGetBest(c, c.Memory.Visible, this.AllScorersMustScoreAboveZero, out baseEntity, out single) || single < this.ScoreThreshold)
			{
				NPCHumanContext nPCHumanContext = c as NPCHumanContext;
				if (nPCHumanContext == null || c.AIAgent.GetWantsToAttack(nPCHumanContext.LastAttacker) <= 0f)
				{
					c.AIAgent.AttackTarget = null;
				}
				else
				{
					c.AIAgent.AttackTarget = nPCHumanContext.LastAttacker;
				}
			}
			else
			{
				if (c.AIAgent.GetWantsToAttack(baseEntity) < 0.1f)
				{
					baseEntity = null;
				}
				c.AIAgent.AttackTarget = baseEntity;
			}
			if (c.AIAgent.AttackTarget != null)
			{
				foreach (Memory.SeenInfo all in c.Memory.All)
				{
					if (all.Entity != baseEntity)
					{
						continue;
					}
					c.AIAgent.AttackTargetMemory = all;
					return;
				}
			}
		}
	}
}