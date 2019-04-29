using System;

namespace Rust.Ai
{
	public class IsPathDistanceToTargetValid : BaseScorer
	{
		public IsPathDistanceToTargetValid()
		{
		}

		public static bool Evaluate(NPCHumanContext c)
		{
			if (c == null || c.Human.AttackTarget == null)
			{
				return false;
			}
			return c.Human.PathDistanceIsValid(c.Human.ServerPosition, c.Human.AttackTarget.ServerPosition, false);
		}

		public override float GetScore(BaseContext c)
		{
			object obj;
			if (IsPathDistanceToTargetValid.Evaluate(c as NPCHumanContext))
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