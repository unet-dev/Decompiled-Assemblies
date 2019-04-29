using System;

namespace Rust.Ai
{
	public class ShouldReturnToSpawnPoint : BaseScorer
	{
		public ShouldReturnToSpawnPoint()
		{
		}

		public override float GetScore(BaseContext ctx)
		{
			object obj;
			NPCHumanContext nPCHumanContext = ctx as NPCHumanContext;
			if (nPCHumanContext == null || nPCHumanContext.GetFact(NPCPlayerApex.Facts.RangeToSpawnLocation) < (byte)nPCHumanContext.Human.GetStats.MaxRangeToSpawnLoc)
			{
				return 0f;
			}
			if (float.IsNaN(nPCHumanContext.Human.SecondsSinceLastInRangeOfSpawnPosition) || float.IsNegativeInfinity(nPCHumanContext.Human.SecondsSinceLastInRangeOfSpawnPosition) || float.IsInfinity(nPCHumanContext.Human.SecondsSinceLastInRangeOfSpawnPosition))
			{
				return 0f;
			}
			if (nPCHumanContext.Human.SecondsSinceLastInRangeOfSpawnPosition >= nPCHumanContext.Human.GetStats.OutOfRangeOfSpawnPointTimeout)
			{
				obj = 1;
			}
			else
			{
				obj = null;
			}
			return (float)obj;
		}
	}
}