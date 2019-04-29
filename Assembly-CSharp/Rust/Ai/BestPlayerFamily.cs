using Apex.AI;
using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public class BestPlayerFamily : OptionScorerBase<BasePlayer>
	{
		[ApexSerialization]
		private float score = 10f;

		public BestPlayerFamily()
		{
		}

		public override float Score(IAIContext context, BasePlayer option)
		{
			PlayerTargetContext playerTargetContext = context as PlayerTargetContext;
			if (playerTargetContext == null)
			{
				return 0f;
			}
			if (option.Family == playerTargetContext.Self.GetStats.Family)
			{
				return 0f;
			}
			return this.score;
		}
	}
}