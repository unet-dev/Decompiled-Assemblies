using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public class HasFactFoodRange : BaseScorer
	{
		[ApexSerialization(defaultValue=BaseNpc.FoodRangeEnum.EatRange)]
		public BaseNpc.FoodRangeEnum @value;

		public HasFactFoodRange()
		{
		}

		public override float GetScore(BaseContext c)
		{
			if (c.GetFact(BaseNpc.Facts.FoodRange) != (byte)this.@value)
			{
				return 0f;
			}
			return 1f;
		}
	}
}