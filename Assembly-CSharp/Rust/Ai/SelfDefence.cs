using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public sealed class SelfDefence : BaseScorer
	{
		[ApexSerialization]
		public float WithinSeconds = 10f;

		public SelfDefence()
		{
		}

		public override float GetScore(BaseContext c)
		{
			if (float.IsNegativeInfinity(c.Entity.SecondsSinceAttacked) || float.IsNaN(c.Entity.SecondsSinceAttacked))
			{
				return 0f;
			}
			return (this.WithinSeconds - c.Entity.SecondsSinceAttacked) / this.WithinSeconds * c.AIAgent.GetStats.Defensiveness;
		}
	}
}