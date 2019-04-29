using Apex.AI;
using System;
using UnityEngine;

namespace Rust.Ai
{
	public class ValidDistance : OptionScorerBase<BaseEntity>
	{
		public ValidDistance()
		{
		}

		public override float Score(IAIContext context, BaseEntity option)
		{
			EntityTargetContext entityTargetContext = context as EntityTargetContext;
			if (entityTargetContext == null)
			{
				return 0f;
			}
			if ((entityTargetContext.Self.Entity.ServerPosition - option.ServerPosition).sqrMagnitude > entityTargetContext.Self.GetStats.CloseRange * entityTargetContext.Self.GetStats.CloseRange)
			{
				return 0f;
			}
			return 1f;
		}
	}
}