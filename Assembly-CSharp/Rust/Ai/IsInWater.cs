using Apex.Serialization;
using System;
using UnityEngine;

namespace Rust.Ai
{
	public class IsInWater : WeightedScorerBase<Vector3>
	{
		[ApexSerialization(defaultValue=3f)]
		public float MaxDepth = 3f;

		public IsInWater()
		{
		}

		public override float GetScore(BaseContext c, Vector3 position)
		{
			return WaterLevel.GetWaterDepth(position) / this.MaxDepth;
		}
	}
}