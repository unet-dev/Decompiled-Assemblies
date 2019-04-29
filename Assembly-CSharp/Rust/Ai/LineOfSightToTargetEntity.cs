using Apex.Serialization;
using System;
using UnityEngine;

namespace Rust.Ai
{
	public sealed class LineOfSightToTargetEntity : BaseScorer
	{
		[ApexSerialization]
		private CoverPoint.CoverType Cover;

		public LineOfSightToTargetEntity()
		{
		}

		public override float GetScore(BaseContext c)
		{
			if (c.AIAgent.AttackTarget == null)
			{
				return 0f;
			}
			BasePlayer attackTarget = c.AIAgent.AttackTarget as BasePlayer;
			if (!attackTarget)
			{
				if (this.Cover == CoverPoint.CoverType.Full)
				{
					if (!c.AIAgent.AttackTarget.IsVisible(c.AIAgent.AttackPosition, Single.PositiveInfinity))
					{
						return 0f;
					}
					return 1f;
				}
				if (!c.AIAgent.AttackTarget.IsVisible(c.AIAgent.CrouchedAttackPosition, Single.PositiveInfinity))
				{
					return 0f;
				}
				return 1f;
			}
			Vector3 attackPosition = c.AIAgent.AttackPosition;
			if ((attackTarget.IsVisible(attackPosition, attackTarget.CenterPoint(), Single.PositiveInfinity) || attackTarget.IsVisible(attackPosition, attackTarget.eyes.position, Single.PositiveInfinity) ? false : !attackTarget.IsVisible(attackPosition, attackTarget.transform.position, Single.PositiveInfinity)))
			{
				return 0f;
			}
			return 1f;
		}
	}
}