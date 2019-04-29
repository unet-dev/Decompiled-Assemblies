using System;

namespace Rust.Ai
{
	public sealed class IsSleeping : BaseScorer
	{
		public IsSleeping()
		{
		}

		public override float GetScore(BaseContext c)
		{
			object obj;
			if (c.AIAgent.CurrentBehaviour == BaseNpc.Behaviour.Sleep)
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