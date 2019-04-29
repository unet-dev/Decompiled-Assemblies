using Apex.Serialization;
using System;
using UnityEngine;

namespace Apex.AI
{
	public abstract class UtilityCurveEditorBaseScorer : ContextualScorerBase
	{
		[ApexSerialization]
		public AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		protected UtilityCurveEditorBaseScorer()
		{
		}

		public float GetCurveScore(float value)
		{
			return this.curve.Evaluate(value);
		}
	}
}