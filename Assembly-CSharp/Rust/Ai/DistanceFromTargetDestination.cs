using Apex.Serialization;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai
{
	public sealed class DistanceFromTargetDestination : BaseScorer
	{
		[ApexSerialization(defaultValue=10f)]
		public float MaxDistance = 10f;

		public DistanceFromTargetDestination()
		{
		}

		public override float GetScore(BaseContext c)
		{
			if (!c.AIAgent.IsNavRunning())
			{
				return 1f;
			}
			return Vector3.Distance(c.Entity.ServerPosition, c.AIAgent.GetNavAgent.destination) / this.MaxDistance;
		}
	}
}