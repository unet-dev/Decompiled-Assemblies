using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public sealed class TargetEntityVisibleFor : BaseScorer
	{
		[ApexSerialization]
		public float duration;

		public TargetEntityVisibleFor()
		{
		}

		public override float GetScore(BaseContext c)
		{
			if (c.AIAgent.AttackTargetVisibleFor < this.duration)
			{
				return 0f;
			}
			return 1f;
		}
	}
}