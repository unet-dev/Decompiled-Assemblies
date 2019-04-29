using Apex.AI;
using System;

namespace Rust.Ai
{
	public class IsPathDistanceBetweenHideoutAndLKPValid : OptionScorerBase<CoverPoint>
	{
		public IsPathDistanceBetweenHideoutAndLKPValid()
		{
		}

		public static bool Evaluate(CoverContext c, CoverPoint option)
		{
			if (c == null || c.Self.AttackTarget == null)
			{
				return false;
			}
			NPCPlayerApex entity = c.Self.Entity as NPCPlayerApex;
			if (entity == null)
			{
				return false;
			}
			return entity.PathDistanceIsValid(option.Position, c.DangerPoint, true);
		}

		public override float Score(IAIContext context, CoverPoint option)
		{
			if (!IsPathDistanceBetweenHideoutAndLKPValid.Evaluate(context as CoverContext, option))
			{
				return 0f;
			}
			return 1f;
		}
	}
}