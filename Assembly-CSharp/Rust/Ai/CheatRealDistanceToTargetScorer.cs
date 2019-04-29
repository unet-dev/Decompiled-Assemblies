using Apex.AI;
using System;
using UnityEngine;

namespace Rust.Ai
{
	public class CheatRealDistanceToTargetScorer : OptionScorerBase<CoverPoint>
	{
		public CheatRealDistanceToTargetScorer()
		{
		}

		public static float Evaluate(CoverContext c, CoverPoint option)
		{
			if (c == null)
			{
				return 0f;
			}
			Vector3 serverPosition = c.Self.Entity.ServerPosition;
			if (Mathf.Abs((c.DangerPoint - serverPosition).magnitude - (option.Position - serverPosition).magnitude) > 8f)
			{
				return 0f;
			}
			return 1f;
		}

		public override float Score(IAIContext context, CoverPoint option)
		{
			return CheatRealDistanceToTargetScorer.Evaluate(context as CoverContext, option);
		}
	}
}