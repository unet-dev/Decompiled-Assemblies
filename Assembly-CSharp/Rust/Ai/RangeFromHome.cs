using Apex.Serialization;
using System;
using UnityEngine;

namespace Rust.Ai
{
	public class RangeFromHome : WeightedScorerBase<Vector3>
	{
		[ApexSerialization]
		public float Range = 50f;

		[ApexSerialization]
		public AnimationCurve ResponseCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		[ApexSerialization]
		public bool UseResponseCurve = true;

		public RangeFromHome()
		{
		}

		public override float GetScore(BaseContext c, Vector3 position)
		{
			float single = Mathf.Min(Vector3.Distance(position, c.AIAgent.SpawnPosition), this.Range) / this.Range;
			if (!this.UseResponseCurve)
			{
				return single;
			}
			return this.ResponseCurve.Evaluate(single);
		}
	}
}