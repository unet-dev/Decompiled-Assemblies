using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public sealed class TestBehaviour : BaseScorer
	{
		[ApexSerialization]
		public BaseNpc.Behaviour Behaviour;

		public TestBehaviour()
		{
		}

		public override float GetScore(BaseContext c)
		{
			object obj;
			if (c.AIAgent.CurrentBehaviour == this.Behaviour)
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