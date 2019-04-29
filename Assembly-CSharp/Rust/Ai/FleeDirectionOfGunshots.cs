using Apex.Serialization;
using System;
using UnityEngine;

namespace Rust.Ai
{
	public sealed class FleeDirectionOfGunshots : WeightedScorerBase<Vector3>
	{
		[ApexSerialization]
		public float WithinSeconds = 10f;

		[ApexSerialization]
		public float Arc = -0.2f;

		public FleeDirectionOfGunshots()
		{
		}

		public override float GetScore(BaseContext c, Vector3 option)
		{
			BaseNpc aIAgent = c.AIAgent as BaseNpc;
			if (aIAgent == null)
			{
				return 0f;
			}
			if (float.IsInfinity(aIAgent.SecondsSinceLastHeardGunshot) || float.IsNaN(aIAgent.SecondsSinceLastHeardGunshot))
			{
				return 0f;
			}
			if ((this.WithinSeconds - aIAgent.SecondsSinceLastHeardGunshot) / this.WithinSeconds <= 0f)
			{
				return 0f;
			}
			Vector3 vector3 = option - aIAgent.transform.localPosition;
			float single = Vector3.Dot(aIAgent.LastHeardGunshotDirection, vector3);
			if (this.Arc <= single)
			{
				return 0f;
			}
			return 1f;
		}
	}
}