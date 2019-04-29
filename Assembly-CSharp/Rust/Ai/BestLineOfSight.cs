using Apex.AI;
using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public class BestLineOfSight : OptionScorerBase<BasePlayer>
	{
		[ApexSerialization]
		private float score = 10f;

		public BestLineOfSight()
		{
		}

		public static byte Evaluate(NPCPlayerApex self, BasePlayer option, out int standing, out int crouched)
		{
			standing = (self.IsVisibleStanding(option) ? 1 : 0);
			crouched = (self.IsVisibleCrouched(option) ? 1 : 0);
			return (byte)(standing + crouched * 2);
		}

		public override float Score(IAIContext context, BasePlayer option)
		{
			int num;
			int num1;
			PlayerTargetContext playerTargetContext = context as PlayerTargetContext;
			if (playerTargetContext != null)
			{
				NPCPlayerApex self = playerTargetContext.Self as NPCPlayerApex;
				if (self)
				{
					playerTargetContext.LineOfSight[playerTargetContext.CurrentOptionsIndex] = BestLineOfSight.Evaluate(self, option, out num, out num1);
					return (float)(num + num1) * 0.5f * this.score;
				}
			}
			playerTargetContext.LineOfSight[playerTargetContext.CurrentOptionsIndex] = 0;
			return 0f;
		}
	}
}