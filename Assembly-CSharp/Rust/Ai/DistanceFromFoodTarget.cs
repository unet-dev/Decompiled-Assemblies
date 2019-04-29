using Apex.Serialization;
using System;
using UnityEngine;

namespace Rust.Ai
{
	public sealed class DistanceFromFoodTarget : BaseScorer
	{
		[ApexSerialization(defaultValue=10f)]
		public float MaxDistance = 10f;

		public DistanceFromFoodTarget()
		{
		}

		public override float GetScore(BaseContext c)
		{
			if (c.AIAgent.FoodTarget == null)
			{
				return 0f;
			}
			return Vector3.Distance(c.Position, c.AIAgent.FoodTarget.transform.localPosition) / this.MaxDistance;
		}
	}
}