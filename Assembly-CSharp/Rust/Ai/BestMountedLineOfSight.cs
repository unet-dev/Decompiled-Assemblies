using Apex.AI;
using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public class BestMountedLineOfSight : OptionScorerBase<BasePlayer>
	{
		[ApexSerialization]
		private float score = 10f;

		public BestMountedLineOfSight()
		{
		}

		public static byte Evaluate(NPCPlayerApex self, BasePlayer option)
		{
			object obj;
			if (self.IsVisibleMounted(option))
			{
				obj = 1;
			}
			else
			{
				obj = null;
			}
			return (byte)obj;
		}

		public override float Score(IAIContext context, BasePlayer option)
		{
			PlayerTargetContext playerTargetContext = context as PlayerTargetContext;
			if (playerTargetContext != null)
			{
				NPCPlayerApex self = playerTargetContext.Self as NPCPlayerApex;
				if (self)
				{
					byte num = BestMountedLineOfSight.Evaluate(self, option);
					playerTargetContext.LineOfSight[playerTargetContext.CurrentOptionsIndex] = num;
					return (float)num * this.score;
				}
			}
			playerTargetContext.LineOfSight[playerTargetContext.CurrentOptionsIndex] = 0;
			return 0f;
		}
	}
}