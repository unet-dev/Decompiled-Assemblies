using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public sealed class HasRecentlyBeenAttacked : BaseScorer
	{
		[ApexSerialization]
		public float WithinSeconds = 10f;

		[ApexSerialization]
		public bool BooleanResult;

		public HasRecentlyBeenAttacked()
		{
		}

		public override float GetScore(BaseContext c)
		{
			if (float.IsNegativeInfinity(c.Entity.SecondsSinceAttacked) || float.IsNaN(c.Entity.SecondsSinceAttacked))
			{
				return 0f;
			}
			if (this.BooleanResult)
			{
				if (c.Entity.SecondsSinceAttacked > this.WithinSeconds)
				{
					return 0f;
				}
				return 1f;
			}
			return (this.WithinSeconds - c.Entity.SecondsSinceAttacked) / this.WithinSeconds;
		}
	}
}