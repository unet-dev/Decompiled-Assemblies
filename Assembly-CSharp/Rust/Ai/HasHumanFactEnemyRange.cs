using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public class HasHumanFactEnemyRange : BaseScorer
	{
		[ApexSerialization(defaultValue=NPCPlayerApex.EnemyRangeEnum.CloseAttackRange)]
		public NPCPlayerApex.EnemyRangeEnum @value;

		public HasHumanFactEnemyRange()
		{
		}

		public override float GetScore(BaseContext c)
		{
			if (c.GetFact(NPCPlayerApex.Facts.EnemyRange) != (byte)this.@value)
			{
				return 0f;
			}
			return 1f;
		}
	}
}