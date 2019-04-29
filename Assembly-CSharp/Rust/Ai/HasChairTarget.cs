using System;

namespace Rust.Ai
{
	public class HasChairTarget : BaseScorer
	{
		public HasChairTarget()
		{
		}

		public override float GetScore(BaseContext context)
		{
			return HasChairTarget.Test(context as NPCHumanContext);
		}

		public static float Test(NPCHumanContext c)
		{
			if (c.ChairTarget == null)
			{
				return 0f;
			}
			return 1f;
		}
	}
}