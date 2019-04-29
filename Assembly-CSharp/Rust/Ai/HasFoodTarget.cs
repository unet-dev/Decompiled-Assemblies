using System;

namespace Rust.Ai
{
	public class HasFoodTarget : BaseScorer
	{
		public HasFoodTarget()
		{
		}

		public override float GetScore(BaseContext c)
		{
			if (c.AIAgent.FoodTarget == null)
			{
				return 0f;
			}
			return 1f;
		}
	}
}