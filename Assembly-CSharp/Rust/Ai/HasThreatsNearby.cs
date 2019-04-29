using Apex.Serialization;
using System;
using System.Collections.Generic;

namespace Rust.Ai
{
	public sealed class HasThreatsNearby : BaseScorer
	{
		[ApexSerialization]
		public float range = 20f;

		public HasThreatsNearby()
		{
		}

		public override float GetScore(BaseContext c)
		{
			float single = 0f;
			for (int i = 0; i < c.Memory.All.Count; i++)
			{
				Memory.SeenInfo item = c.Memory.All[i];
				if (!(item.Entity == null) && c.Entity.Distance(item.Entity) <= this.range)
				{
					single += c.AIAgent.FearLevel(item.Entity);
				}
			}
			return single;
		}
	}
}