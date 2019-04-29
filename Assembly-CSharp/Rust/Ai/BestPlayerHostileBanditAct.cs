using Apex.AI;
using Apex.Serialization;
using System;
using UnityEngine;

namespace Rust.Ai
{
	public class BestPlayerHostileBanditAct : OptionScorerBase<BasePlayer>
	{
		[ApexSerialization]
		private float score = 10f;

		[ApexSerialization]
		public float Timeout = 10f;

		public BestPlayerHostileBanditAct()
		{
		}

		public override float Score(IAIContext context, BasePlayer option)
		{
			PlayerTargetContext playerTargetContext = context as PlayerTargetContext;
			if (playerTargetContext != null)
			{
				Scientist self = playerTargetContext.Self as Scientist;
				if (self)
				{
					Memory.ExtendedInfo extendedInfo = self.AiContext.Memory.GetExtendedInfo(option);
					if (extendedInfo.Entity == null)
					{
						if (!self.HostilityConsideration(option))
						{
							return 0f;
						}
						return this.score;
					}
					if (Time.time < extendedInfo.LastHurtUsTime + this.Timeout)
					{
						return this.score;
					}
					if (!self.HostilityConsideration(option))
					{
						return 0f;
					}
					return this.score;
				}
			}
			return 0f;
		}
	}
}