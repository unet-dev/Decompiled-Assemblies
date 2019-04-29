using System;

namespace Rust.Ai
{
	public class HasHideout : BaseScorer
	{
		public HasHideout()
		{
		}

		public override float GetScore(BaseContext context)
		{
			NPCHumanContext nPCHumanContext = context as NPCHumanContext;
			if (nPCHumanContext == null)
			{
				return 0f;
			}
			if (nPCHumanContext.EnemyHideoutGuess == null)
			{
				return 0f;
			}
			return 1f;
		}
	}
}