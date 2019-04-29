using System;

namespace Rust.Ai
{
	public sealed class IsCurrentFoodTarget : WeightedScorerBase<BaseEntity>
	{
		public IsCurrentFoodTarget()
		{
		}

		public override float GetScore(BaseContext c, BaseEntity target)
		{
			object obj;
			if (c.AIAgent.FoodTarget == target)
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