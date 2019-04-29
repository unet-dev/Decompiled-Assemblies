using Apex.Serialization;
using System;
using UnityEngine;

namespace Rust.Ai
{
	public sealed class HasSeenThreatsLately : BaseScorer
	{
		[ApexSerialization]
		public float WithinSeconds = 10f;

		public HasSeenThreatsLately()
		{
		}

		public override float GetScore(BaseContext c)
		{
			if (c.AIAgent.AttackTargetMemory.Timestamp > 0f && Time.realtimeSinceStartup - c.AIAgent.AttackTargetMemory.Timestamp <= this.WithinSeconds)
			{
				return 1f;
			}
			return 0f;
		}
	}
}