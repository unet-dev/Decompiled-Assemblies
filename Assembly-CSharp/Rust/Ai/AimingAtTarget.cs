using Apex.Serialization;
using System;
using UnityEngine;

namespace Rust.Ai
{
	public sealed class AimingAtTarget : BaseScorer
	{
		[ApexSerialization]
		public float arc;

		[ApexSerialization]
		public bool PerfectKnowledge;

		public AimingAtTarget()
		{
		}

		public override float GetScore(BaseContext c)
		{
			if (c.AIAgent.AttackTarget == null)
			{
				return 0f;
			}
			if (!this.PerfectKnowledge)
			{
				if (Vector3.Dot(c.AIAgent.CurrentAimAngles, (c.AIAgent.AttackTargetMemory.Position - c.AIAgent.AttackPosition).normalized) < this.arc)
				{
					return 0f;
				}
				return 1f;
			}
			if (Vector3.Dot(c.AIAgent.CurrentAimAngles, (c.AIAgent.AttackTarget.transform.position - c.AIAgent.AttackPosition).normalized) < this.arc)
			{
				return 0f;
			}
			return 1f;
		}
	}
}