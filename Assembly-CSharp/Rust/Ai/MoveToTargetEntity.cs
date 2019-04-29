using System;

namespace Rust.Ai
{
	public class MoveToTargetEntity : BaseAction
	{
		public MoveToTargetEntity()
		{
		}

		public override void DoExecute(BaseContext c)
		{
			if (c.AIAgent.AttackTarget == null)
			{
				return;
			}
			c.AIAgent.UpdateDestination(c.AIAgent.AttackTargetMemory.Position);
		}
	}
}