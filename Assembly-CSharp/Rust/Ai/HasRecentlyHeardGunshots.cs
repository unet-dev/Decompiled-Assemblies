using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public sealed class HasRecentlyHeardGunshots : BaseScorer
	{
		[ApexSerialization]
		public float WithinSeconds = 10f;

		public HasRecentlyHeardGunshots()
		{
		}

		public override float GetScore(BaseContext c)
		{
			BaseNpc aIAgent = c.AIAgent as BaseNpc;
			if (aIAgent == null)
			{
				return 0f;
			}
			if (float.IsInfinity(aIAgent.SecondsSinceLastHeardGunshot) || float.IsNaN(aIAgent.SecondsSinceLastHeardGunshot))
			{
				return 0f;
			}
			return (this.WithinSeconds - aIAgent.SecondsSinceLastHeardGunshot) / this.WithinSeconds;
		}
	}
}