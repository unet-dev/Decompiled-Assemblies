using System;

namespace Rust.Ai
{
	public class IsHumanRoamReady : BaseScorer
	{
		public IsHumanRoamReady()
		{
		}

		public static bool Evaluate(NPCHumanContext c)
		{
			return c.GetFact(NPCPlayerApex.Facts.IsRoamReady) > 0;
		}

		public override float GetScore(BaseContext c)
		{
			object obj;
			if (IsHumanRoamReady.Evaluate(c as NPCHumanContext))
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