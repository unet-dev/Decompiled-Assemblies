using Apex.AI;
using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public class HasPlayerTarget : ContextualScorerBase<PlayerTargetContext>
	{
		[ApexSerialization]
		private bool Not;

		public HasPlayerTarget()
		{
		}

		public override float Score(PlayerTargetContext c)
		{
			if (this.Not)
			{
				return (c.Target != null ? 0f : 1f) * this.score;
			}
			return (c.Target != null ? 1f : 0f) * this.score;
		}
	}
}