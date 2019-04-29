using Apex.Serialization;
using System;
using UnityEngine;

namespace Rust.Ai
{
	public class TimeSinceLastMoveToCoverThreshold : BaseScorer
	{
		[ApexSerialization]
		public float minThreshold;

		[ApexSerialization]
		public float maxThreshold = 1f;

		public TimeSinceLastMoveToCoverThreshold()
		{
		}

		public static bool Evaluate(NPCHumanContext c, float minThreshold, float maxThreshold)
		{
			if (Mathf.Approximately(c.Human.TimeLastMovedToCover, 0f))
			{
				return true;
			}
			float timeLastMovedToCover = Time.realtimeSinceStartup - c.Human.TimeLastMovedToCover;
			if (c.GetFact(NPCPlayerApex.Facts.IsMovingToCover) > 0 || timeLastMovedToCover < minThreshold)
			{
				return false;
			}
			if (timeLastMovedToCover >= maxThreshold)
			{
				return true;
			}
			float single = maxThreshold - minThreshold;
			return UnityEngine.Random.@value < (maxThreshold - timeLastMovedToCover) / single;
		}

		public override float GetScore(BaseContext c)
		{
			object obj;
			if (TimeSinceLastMoveToCoverThreshold.Evaluate(c as NPCHumanContext, this.minThreshold, this.maxThreshold))
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