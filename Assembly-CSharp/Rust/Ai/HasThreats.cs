using System;
using System.Collections.Generic;

namespace Rust.Ai
{
	public sealed class HasThreats : BaseScorer
	{
		public HasThreats()
		{
		}

		public override float GetScore(BaseContext c)
		{
			float single = 0f;
			for (int i = 0; i < c.Memory.All.Count; i++)
			{
				Memory.SeenInfo item = c.Memory.All[i];
				if (item.Entity != null)
				{
					single += c.AIAgent.FearLevel(item.Entity);
				}
			}
			return single;
		}
	}
}