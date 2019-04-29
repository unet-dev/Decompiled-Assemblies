using Apex.AI;
using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public class BestHostility : OptionScorerBase<BasePlayer>
	{
		[ApexSerialization]
		private float score = 10f;

		public BestHostility()
		{
		}

		public override float Score(IAIContext context, BasePlayer option)
		{
			PlayerTargetContext playerTargetContext = context as PlayerTargetContext;
			if (playerTargetContext != null)
			{
				NPCPlayerApex self = playerTargetContext.Self as NPCPlayerApex;
				if (self)
				{
					if (!self.HostilityConsideration(option))
					{
						return 0f;
					}
					return this.score;
				}
			}
			return 0f;
		}
	}
}