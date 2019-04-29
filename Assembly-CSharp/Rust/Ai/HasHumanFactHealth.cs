using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public class HasHumanFactHealth : BaseScorer
	{
		[ApexSerialization(defaultValue=NPCPlayerApex.HealthEnum.Fine)]
		public NPCPlayerApex.HealthEnum @value;

		public HasHumanFactHealth()
		{
		}

		public override float GetScore(BaseContext c)
		{
			if (c.GetFact(NPCPlayerApex.Facts.Health) != (byte)this.@value)
			{
				return 0f;
			}
			return 1f;
		}
	}
}