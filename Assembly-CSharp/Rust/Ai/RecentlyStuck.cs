using System;

namespace Rust.Ai
{
	public class RecentlyStuck : BaseScorer
	{
		public RecentlyStuck()
		{
		}

		public override float GetScore(BaseContext c)
		{
			if (c.AIAgent.GetLastStuckTime == 0f)
			{
				return 0f;
			}
			return 1f;
		}
	}
}