using Apex.AI;
using System;

namespace Rust.Ai
{
	public class HasAlreadyCheckedHideoutPointScorer : OptionScorerBase<CoverPoint>
	{
		public HasAlreadyCheckedHideoutPointScorer()
		{
		}

		public static float Evaluate(CoverContext c, CoverPoint option)
		{
			if (c != null)
			{
				NPCPlayerApex entity = c.Self.Entity as NPCPlayerApex;
				if (entity != null && !entity.AiContext.HasCheckedHideout(option))
				{
					return 1f;
				}
			}
			return 0f;
		}

		public override float Score(IAIContext context, CoverPoint option)
		{
			return HasAlreadyCheckedHideoutPointScorer.Evaluate(context as CoverContext, option);
		}
	}
}