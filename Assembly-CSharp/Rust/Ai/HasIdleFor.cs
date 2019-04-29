using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public class HasIdleFor : BaseScorer
	{
		[ApexSerialization]
		public float StuckSeconds = 5f;

		public HasIdleFor()
		{
		}

		public override float GetScore(BaseContext c)
		{
			if (c.AIAgent.GetStuckDuration < this.StuckSeconds)
			{
				return 0f;
			}
			return 1f;
		}
	}
}