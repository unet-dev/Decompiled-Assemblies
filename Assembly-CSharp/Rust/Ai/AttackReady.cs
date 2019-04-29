using System;

namespace Rust.Ai
{
	public class AttackReady : BaseScorer
	{
		public AttackReady()
		{
		}

		public override float GetScore(BaseContext c)
		{
			if (!c.AIAgent.AttackReady())
			{
				return 0f;
			}
			return 1f;
		}
	}
}