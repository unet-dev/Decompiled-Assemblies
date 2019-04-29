using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public sealed class BehaviorDuration : BaseScorer
	{
		[ApexSerialization]
		public BaseNpc.Behaviour Behaviour;

		[ApexSerialization]
		public float duration;

		public BehaviorDuration()
		{
		}

		public override float GetScore(BaseContext c)
		{
			object obj;
			if (c.AIAgent.CurrentBehaviour != this.Behaviour || c.AIAgent.currentBehaviorDuration < this.duration)
			{
				obj = null;
			}
			else
			{
				obj = 1;
			}
			return (float)obj;
		}
	}
}