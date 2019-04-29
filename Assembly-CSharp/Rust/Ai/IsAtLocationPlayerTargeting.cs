using Apex.AI;
using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public class IsAtLocationPlayerTargeting : ContextualScorerBase<PlayerTargetContext>
	{
		[ApexSerialization]
		public AiLocationSpawner.SquadSpawnerLocation Location;

		public IsAtLocationPlayerTargeting()
		{
		}

		public override float Score(PlayerTargetContext c)
		{
			if (!IsAtLocationPlayerTargeting.Test(c, this.Location))
			{
				return 0f;
			}
			return this.score;
		}

		public static bool Test(PlayerTargetContext c, AiLocationSpawner.SquadSpawnerLocation location)
		{
			NPCPlayerApex self = c.Self as NPCPlayerApex;
			if (self == null)
			{
				return false;
			}
			if (self.AiContext.AiLocationManager == null)
			{
				return false;
			}
			return self.AiContext.AiLocationManager.LocationType == location;
		}
	}
}