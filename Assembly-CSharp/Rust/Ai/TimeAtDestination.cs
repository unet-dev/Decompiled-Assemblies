using System;

namespace Rust.Ai
{
	public class TimeAtDestination : BaseScorer
	{
		public TimeAtDestination()
		{
		}

		public override float GetScore(BaseContext c)
		{
			return c.AIAgent.TimeAtDestination;
		}
	}
}