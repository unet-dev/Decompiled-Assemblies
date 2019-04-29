using Apex.AI;
using System;

namespace Rust.Ai
{
	public class HasAttackTarget : BaseScorer
	{
		public HasAttackTarget()
		{
		}

		public override float GetScore(BaseContext c)
		{
			if (c.AIAgent.AttackTarget == null)
			{
				return 0f;
			}
			return this.score;
		}
	}
}