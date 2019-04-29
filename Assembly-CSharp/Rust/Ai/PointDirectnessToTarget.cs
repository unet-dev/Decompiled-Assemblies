using Apex.AI;
using Apex.Serialization;
using System;
using UnityEngine;

namespace Rust.Ai
{
	public class PointDirectnessToTarget : WeightedScorerBase<Vector3>
	{
		[ApexSerialization]
		[FriendlyName("Use Perfect Position Information", "Should we apply perfect knowledge about the attack target's whereabouts, or the last memorized position.")]
		private bool UsePerfectInfo;

		public PointDirectnessToTarget()
		{
		}

		public override float GetScore(BaseContext c, Vector3 point)
		{
			Vector3 vector3;
			vector3 = (!this.UsePerfectInfo ? c.AIAgent.AttackTargetMemory.Position : c.AIAgent.AttackTarget.ServerPosition);
			float single = Vector3.Distance(c.Position, vector3);
			float single1 = Vector3.Distance(point, vector3);
			float single2 = Vector3.Distance(c.Position, point);
			return (single - single1) / single2;
		}
	}
}