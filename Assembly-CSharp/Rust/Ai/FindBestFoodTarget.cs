using Apex.AI;
using System;

namespace Rust.Ai
{
	public class FindBestFoodTarget : BaseActionWithOptions<BaseEntity>
	{
		public FindBestFoodTarget()
		{
		}

		public override void DoExecute(BaseContext c)
		{
			BaseEntity best = base.GetBest(c, c.Memory.Visible);
			if (best == null || !c.AIAgent.WantsToEat(best))
			{
				best = null;
			}
			c.AIAgent.FoodTarget = best;
		}
	}
}