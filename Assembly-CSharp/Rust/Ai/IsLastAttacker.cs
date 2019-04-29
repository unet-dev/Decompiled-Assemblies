using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public class IsLastAttacker : WeightedScorerBase<BaseEntity>
	{
		[ApexSerialization]
		public float MinScore = 0.1f;

		public IsLastAttacker()
		{
		}

		public override float GetScore(BaseContext context, BaseEntity option)
		{
			NPCHumanContext nPCHumanContext = context as NPCHumanContext;
			if (nPCHumanContext == null)
			{
				return 0f;
			}
			if (nPCHumanContext.LastAttacker == option)
			{
				return 1f;
			}
			return this.MinScore;
		}
	}
}