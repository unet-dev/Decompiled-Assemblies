using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public class LastSetDestinationTimeGreaterThan : BaseScorer
	{
		[ApexSerialization]
		private float Timeout = 5f;

		public LastSetDestinationTimeGreaterThan()
		{
		}

		public override float GetScore(BaseContext c)
		{
			BaseNpc aIAgent = c.AIAgent as BaseNpc;
			if (aIAgent != null && aIAgent.SecondsSinceLastSetDestination > this.Timeout)
			{
				return 1f;
			}
			return 0f;
		}
	}
}