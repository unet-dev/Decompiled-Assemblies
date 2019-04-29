using Apex.Serialization;
using System;
using UnityEngine;

namespace Rust.Ai
{
	public sealed class HealthFractionCurve : BaseScorer
	{
		[ApexSerialization]
		private AnimationCurve ResponseCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		public HealthFractionCurve()
		{
		}

		public override float GetScore(BaseContext c)
		{
			return this.ResponseCurve.Evaluate(c.Entity.healthFraction);
		}
	}
}