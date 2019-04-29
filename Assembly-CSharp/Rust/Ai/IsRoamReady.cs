using System;

namespace Rust.Ai
{
	public class IsRoamReady : BaseScorer
	{
		public IsRoamReady()
		{
		}

		public static bool Evaluate(BaseContext c)
		{
			if (c is NPCHumanContext)
			{
				return c.GetFact(NPCPlayerApex.Facts.IsRoamReady) > 0;
			}
			return c.GetFact(BaseNpc.Facts.IsRoamReady) > 0;
		}

		public override float GetScore(BaseContext c)
		{
			object obj;
			if (IsRoamReady.Evaluate(c))
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