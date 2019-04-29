using Apex.Serialization;
using System;
using UnityEngine;

namespace Rust.Ai
{
	public class AtDestinationForRandom : BaseScorer
	{
		[ApexSerialization]
		public float MinDuration = 2.5f;

		[ApexSerialization]
		public float MaxDuration = 5f;

		public AtDestinationForRandom()
		{
		}

		public override float GetScore(BaseContext c)
		{
			if (c.AIAgent.TimeAtDestination < UnityEngine.Random.Range(this.MinDuration, this.MaxDuration))
			{
				return 0f;
			}
			return 1f;
		}
	}
}