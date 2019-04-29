using Apex.Serialization;
using System;
using UnityEngine;

namespace Rust.Ai
{
	public class TimeSinceLastMoveThreshold : BaseScorer
	{
		[ApexSerialization]
		public float minThreshold;

		[ApexSerialization]
		public float maxThreshold = 1f;

		public TimeSinceLastMoveThreshold()
		{
		}

		public static bool Evaluate(NPCHumanContext c, float minThreshold, float maxThreshold)
		{
			if (Mathf.Approximately(c.Human.TimeLastMoved, 0f))
			{
				return true;
			}
			float timeLastMoved = Time.realtimeSinceStartup - c.Human.TimeLastMoved;
			if (c.GetFact(NPCPlayerApex.Facts.IsMoving) > 0 || timeLastMoved < minThreshold)
			{
				return false;
			}
			if (timeLastMoved >= maxThreshold)
			{
				return true;
			}
			float single = maxThreshold - minThreshold;
			return UnityEngine.Random.@value < (maxThreshold - timeLastMoved) / single;
		}

		public override float GetScore(BaseContext c)
		{
			object obj;
			if (TimeSinceLastMoveThreshold.Evaluate(c as NPCHumanContext, this.minThreshold, this.maxThreshold))
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