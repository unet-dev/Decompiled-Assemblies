using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public class HasHumanFactBodyState : BaseScorer
	{
		[ApexSerialization(defaultValue=NPCPlayerApex.BodyState.StandingTall)]
		public NPCPlayerApex.BodyState @value;

		public HasHumanFactBodyState()
		{
		}

		public override float GetScore(BaseContext c)
		{
			if (c.GetFact(NPCPlayerApex.Facts.BodyState) != (byte)this.@value)
			{
				return 0f;
			}
			return 1f;
		}
	}
}