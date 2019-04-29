using Apex.AI;
using System;

namespace Rust.Ai
{
	public class ValidLineOfSight : OptionScorerBase<BaseEntity>
	{
		public ValidLineOfSight()
		{
		}

		public override float Score(IAIContext context, BaseEntity option)
		{
			EntityTargetContext entityTargetContext = context as EntityTargetContext;
			if (entityTargetContext != null)
			{
				option.IsVisible(entityTargetContext.Self.Entity.CenterPoint(), entityTargetContext.Self.GetStats.CloseRange);
			}
			return 0f;
		}
	}
}