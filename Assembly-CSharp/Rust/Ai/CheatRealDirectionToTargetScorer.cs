using Apex.AI;
using System;
using UnityEngine;

namespace Rust.Ai
{
	public class CheatRealDirectionToTargetScorer : OptionScorerBase<CoverPoint>
	{
		public CheatRealDirectionToTargetScorer()
		{
		}

		public static float Evaluate(CoverContext c, CoverPoint option)
		{
			if (c != null)
			{
				Vector3 serverPosition = c.Self.Entity.ServerPosition;
				Vector3 dangerPoint = c.DangerPoint - serverPosition;
				Vector3 vector3 = dangerPoint.normalized;
				dangerPoint = option.Position - serverPosition;
				float single = Vector3.Dot(dangerPoint.normalized, vector3);
				if (single > 0.5f)
				{
					return single;
				}
			}
			return 0f;
		}

		public override float Score(IAIContext context, CoverPoint option)
		{
			return CheatRealDirectionToTargetScorer.Evaluate(context as CoverContext, option);
		}
	}
}