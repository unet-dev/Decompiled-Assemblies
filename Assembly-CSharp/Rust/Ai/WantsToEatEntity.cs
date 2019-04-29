using System;

namespace Rust.Ai
{
	public sealed class WantsToEatEntity : WeightedScorerBase<BaseEntity>
	{
		public WantsToEatEntity()
		{
		}

		public override float GetScore(BaseContext c, BaseEntity target)
		{
			object obj;
			if (c.AIAgent.WantsToEat(target))
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