using System;

namespace Rust.Ai
{
	public sealed class IsCurrentAttackEntity : WeightedScorerBase<BaseEntity>
	{
		public IsCurrentAttackEntity()
		{
		}

		public override float GetScore(BaseContext c, BaseEntity target)
		{
			object obj;
			if (c.AIAgent.AttackTarget == target)
			{
				obj = 1;
			}
			else
			{
				obj = null;
			}
			return (float)obj;
		}
	}
}