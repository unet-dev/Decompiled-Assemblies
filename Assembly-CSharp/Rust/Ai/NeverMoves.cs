using System;

namespace Rust.Ai
{
	public class NeverMoves : BaseScorer
	{
		public NeverMoves()
		{
		}

		public override float GetScore(BaseContext ctx)
		{
			NPCHumanContext nPCHumanContext = ctx as NPCHumanContext;
			if (nPCHumanContext == null)
			{
				return 0f;
			}
			if (!NeverMoves.Test(nPCHumanContext))
			{
				return 0f;
			}
			return 1f;
		}

		public static bool Test(NPCHumanContext c)
		{
			return c.Human.NeverMove;
		}
	}
}