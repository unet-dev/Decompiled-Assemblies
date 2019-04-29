using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public class HasFactBoolean : BaseScorer
	{
		[ApexSerialization]
		public BaseNpc.Facts fact;

		[ApexSerialization(defaultValue=false)]
		public bool @value;

		public HasFactBoolean()
		{
		}

		public override float GetScore(BaseContext c)
		{
			object obj;
			if (this.@value)
			{
				obj = 1;
			}
			else
			{
				obj = null;
			}
			byte num = (byte)obj;
			if (c.GetFact(this.fact) != num)
			{
				return 0f;
			}
			return 1f;
		}
	}
}