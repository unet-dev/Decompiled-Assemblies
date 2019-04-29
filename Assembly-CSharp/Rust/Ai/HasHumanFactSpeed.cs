using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public class HasHumanFactSpeed : BaseScorer
	{
		[ApexSerialization(defaultValue=NPCPlayerApex.SpeedEnum.StandStill)]
		public NPCPlayerApex.SpeedEnum @value;

		public HasHumanFactSpeed()
		{
		}

		public override float GetScore(BaseContext c)
		{
			if (c.GetFact(NPCPlayerApex.Facts.Speed) != (byte)this.@value)
			{
				return 0f;
			}
			return 1f;
		}
	}
}