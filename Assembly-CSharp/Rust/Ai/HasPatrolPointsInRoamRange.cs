using System;
using UnityEngine;

namespace Rust.Ai
{
	public class HasPatrolPointsInRoamRange : BaseScorer
	{
		public HasPatrolPointsInRoamRange()
		{
		}

		public static bool Evaluate(NPCHumanContext c)
		{
			if (c.AiLocationManager == null)
			{
				return false;
			}
			PathInterestNode firstPatrolPointInRange = c.AiLocationManager.GetFirstPatrolPointInRange(c.Position, c.AIAgent.GetStats.MinRoamRange, c.AIAgent.GetStats.MaxRoamRange);
			if (firstPatrolPointInRange == null)
			{
				return false;
			}
			return Time.time >= firstPatrolPointInRange.NextVisitTime;
		}

		public override float GetScore(BaseContext c)
		{
			object obj;
			if (HasPatrolPointsInRoamRange.Evaluate(c as NPCHumanContext))
			{
				obj = 1;
			}
			else
			{
				obj = null;
			}
			return (float)obj;
		}
	}
}