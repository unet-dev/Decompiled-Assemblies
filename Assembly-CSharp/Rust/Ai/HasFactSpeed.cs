using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public class HasFactSpeed : BaseScorer
	{
		[ApexSerialization(defaultValue=BaseNpc.SpeedEnum.StandStill)]
		public BaseNpc.SpeedEnum @value;

		public HasFactSpeed()
		{
		}

		public override float GetScore(BaseContext c)
		{
			if (c.GetFact(BaseNpc.Facts.Speed) != (byte)this.@value)
			{
				return 0f;
			}
			return 1f;
		}
	}
}