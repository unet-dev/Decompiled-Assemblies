using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public class IsAtLocation : BaseScorer
	{
		[ApexSerialization]
		public AiLocationSpawner.SquadSpawnerLocation Location;

		public IsAtLocation()
		{
		}

		public override float GetScore(BaseContext ctx)
		{
			NPCHumanContext nPCHumanContext = ctx as NPCHumanContext;
			if (nPCHumanContext == null)
			{
				return 0f;
			}
			if (!IsAtLocation.Test(nPCHumanContext, this.Location))
			{
				return 0f;
			}
			return 1f;
		}

		public static bool Test(NPCHumanContext c, AiLocationSpawner.SquadSpawnerLocation location)
		{
			if (c.AiLocationManager == null)
			{
				return false;
			}
			return c.AiLocationManager.LocationType == location;
		}
	}
}