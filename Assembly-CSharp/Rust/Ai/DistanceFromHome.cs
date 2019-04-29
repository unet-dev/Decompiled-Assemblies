using Apex.Serialization;
using System;
using UnityEngine;

namespace Rust.Ai
{
	public class DistanceFromHome : BaseScorer
	{
		[ApexSerialization]
		public float Range = 50f;

		[ApexSerialization]
		public AnimationCurve ResponseCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		[ApexSerialization]
		public bool UseResponseCurve = true;

		public DistanceFromHome()
		{
		}

		public override float GetScore(BaseContext c)
		{
			float single = Mathf.Min(Vector3.Distance(c.Position, c.AIAgent.SpawnPosition), this.Range) / this.Range;
			if (!this.UseResponseCurve)
			{
				return single;
			}
			return this.ResponseCurve.Evaluate(single);
		}
	}
}