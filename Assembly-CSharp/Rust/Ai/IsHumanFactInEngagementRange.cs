using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public class IsHumanFactInEngagementRange : BaseScorer
	{
		[ApexSerialization(defaultValue=NPCPlayerApex.EnemyEngagementRangeEnum.AggroRange)]
		public NPCPlayerApex.EnemyEngagementRangeEnum @value;

		public IsHumanFactInEngagementRange()
		{
		}

		public override float GetScore(BaseContext c)
		{
			if (c.GetFact(NPCPlayerApex.Facts.EnemyEngagementRange) != (byte)this.@value)
			{
				return 0f;
			}
			return 1f;
		}
	}
}