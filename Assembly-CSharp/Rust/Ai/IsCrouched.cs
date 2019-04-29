using System;

namespace Rust.Ai
{
	public sealed class IsCrouched : BaseScorer
	{
		public IsCrouched()
		{
		}

		public override float GetScore(BaseContext c)
		{
			NPCPlayerApex aIAgent = c.AIAgent as NPCPlayerApex;
			if (aIAgent == null)
			{
				return 0f;
			}
			if (!aIAgent.modelState.ducked)
			{
				return 0f;
			}
			return 1f;
		}
	}
}