using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public class AtDestinationFor : BaseScorer
	{
		[ApexSerialization]
		public float Duration = 5f;

		public AtDestinationFor()
		{
		}

		public override float GetScore(BaseContext c)
		{
			if (c.AIAgent.TimeAtDestination < this.Duration)
			{
				return 0f;
			}
			return 1f;
		}
	}
}