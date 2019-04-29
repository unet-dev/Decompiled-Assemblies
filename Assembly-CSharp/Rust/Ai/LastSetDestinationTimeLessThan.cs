using Apex.Serialization;
using System;
using UnityEngine;

namespace Rust.Ai
{
	public class LastSetDestinationTimeLessThan : BaseScorer
	{
		[ApexSerialization]
		private float Timeout = 5f;

		public LastSetDestinationTimeLessThan()
		{
		}

		public override float GetScore(BaseContext c)
		{
			BaseNpc aIAgent = c.AIAgent as BaseNpc;
			if (aIAgent != null && (Mathf.Approximately(aIAgent.LastSetDestinationTime, 0f) || aIAgent.SecondsSinceLastSetDestination < this.Timeout))
			{
				return 1f;
			}
			return 0f;
		}
	}
}