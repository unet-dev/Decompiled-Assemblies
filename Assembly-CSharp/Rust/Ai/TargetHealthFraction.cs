using System;

namespace Rust.Ai
{
	public sealed class TargetHealthFraction : BaseScorer
	{
		public TargetHealthFraction()
		{
		}

		public override float GetScore(BaseContext c)
		{
			BaseCombatEntity combatTarget = c.AIAgent.CombatTarget;
			if (combatTarget == null)
			{
				return 0f;
			}
			return combatTarget.healthFraction;
		}
	}
}