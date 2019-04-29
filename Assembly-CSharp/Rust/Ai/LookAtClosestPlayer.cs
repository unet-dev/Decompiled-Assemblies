using System;

namespace Rust.Ai
{
	public class LookAtClosestPlayer : BaseAction
	{
		public LookAtClosestPlayer()
		{
		}

		public static void Do(NPCHumanContext c)
		{
			c.Human.LookAtEyes = c.ClosestPlayer.eyes;
		}

		public override void DoExecute(BaseContext context)
		{
			NPCHumanContext nPCHumanContext = context as NPCHumanContext;
			if (nPCHumanContext != null)
			{
				LookAtClosestPlayer.Do(nPCHumanContext);
			}
		}
	}
}