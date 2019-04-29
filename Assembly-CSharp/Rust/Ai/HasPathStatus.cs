using Apex.Serialization;
using System;
using UnityEngine.AI;

namespace Rust.Ai
{
	public class HasPathStatus : BaseScorer
	{
		[ApexSerialization]
		private NavMeshPathStatus Status;

		public HasPathStatus()
		{
		}

		public override float GetScore(BaseContext c)
		{
			if (!c.AIAgent.IsNavRunning())
			{
				return 0f;
			}
			if (c.AIAgent.GetNavAgent.pathStatus != this.Status)
			{
				return 0f;
			}
			return 1f;
		}
	}
}