using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public class HasFactEnemyRange : BaseScorer
	{
		[ApexSerialization(defaultValue=BaseNpc.EnemyRangeEnum.AttackRange)]
		public BaseNpc.EnemyRangeEnum @value;

		public HasFactEnemyRange()
		{
		}

		public override float GetScore(BaseContext c)
		{
			if (c.GetFact(BaseNpc.Facts.EnemyRange) != (byte)this.@value)
			{
				return 0f;
			}
			return 1f;
		}
	}
}