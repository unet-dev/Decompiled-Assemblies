using Apex.AI;
using Apex.Serialization;
using System;
using UnityEngine;

namespace Rust.Ai
{
	public class HideoutScorer : OptionScorerBase<CoverPoint>
	{
		[ApexSerialization]
		[Range(-1f, 1f)]
		public float coverFromPointArcThreshold = -0.8f;

		[ApexSerialization]
		public float maxRange = 5f;

		public HideoutScorer()
		{
		}

		public static float Evaluate(CoverContext c, CoverPoint option, float arcThreshold, float maxRange)
		{
			if (c == null || !option.ProvidesCoverFromPoint(c.Self.Entity.ServerPosition, arcThreshold))
			{
				return 0f;
			}
			float position = (option.Position - c.DangerPoint).sqrMagnitude;
			float single = maxRange * maxRange;
			return 1f - Mathf.Min(position, single) / single;
		}

		public override float Score(IAIContext context, CoverPoint option)
		{
			return HideoutScorer.Evaluate(context as CoverContext, option, this.coverFromPointArcThreshold, this.maxRange);
		}
	}
}