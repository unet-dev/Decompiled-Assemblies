using System;

namespace Rust.Ai
{
	public sealed class Hostility : BaseScorer
	{
		public Hostility()
		{
		}

		public override float GetScore(BaseContext c)
		{
			return c.AIAgent.GetStats.Hostility;
		}
	}
}