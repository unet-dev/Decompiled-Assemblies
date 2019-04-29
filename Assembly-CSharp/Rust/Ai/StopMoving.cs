using System;

namespace Rust.Ai
{
	public class StopMoving : BaseAction
	{
		public StopMoving()
		{
		}

		public override void DoExecute(BaseContext c)
		{
			c.AIAgent.StopMoving();
		}
	}
}