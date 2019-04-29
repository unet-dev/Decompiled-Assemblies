using System;

namespace Rust.Ai
{
	public sealed class StartAttack : BaseAction
	{
		public StartAttack()
		{
		}

		public override void DoExecute(BaseContext c)
		{
			c.AIAgent.StartAttack();
		}
	}
}