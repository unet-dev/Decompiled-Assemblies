using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public class HasFactValue : BaseScorer
	{
		[ApexSerialization]
		public BaseNpc.Facts fact;

		[ApexSerialization(defaultValue=0f)]
		public byte @value;

		public HasFactValue()
		{
		}

		public override float GetScore(BaseContext c)
		{
			if (c.GetFact(this.fact) != this.@value)
			{
				return 0f;
			}
			return 1f;
		}
	}
}