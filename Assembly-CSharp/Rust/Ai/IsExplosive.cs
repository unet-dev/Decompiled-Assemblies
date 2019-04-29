using Apex.AI;
using Rust;
using System;
using System.Collections.Generic;

namespace Rust.Ai
{
	public class IsExplosive : OptionScorerBase<BaseEntity>
	{
		public IsExplosive()
		{
		}

		public override float Score(IAIContext context, BaseEntity option)
		{
			TimedExplosive timedExplosive = option as TimedExplosive;
			if (!timedExplosive)
			{
				return 0f;
			}
			float single = 0f;
			foreach (DamageTypeEntry damageType in timedExplosive.damageTypes)
			{
				single += damageType.amount;
			}
			if (single <= 0f)
			{
				return 0f;
			}
			return 1f;
		}
	}
}